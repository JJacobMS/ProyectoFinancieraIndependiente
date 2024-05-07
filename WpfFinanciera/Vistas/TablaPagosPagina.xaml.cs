using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfFinanciera.ServicioFinancieraIndependiente;
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para TablaPagosPagina.xaml
    /// </summary>
    public partial class TablaPagosPagina : Page
    {
        private Credito _credito;
        private List<Cobro> _cobros;
        private List<(DateTime, DateTime)> _rangosMeses;
        List<(DateTime, DateTime)> _fechasPrevias;
        List<(DateTime, DateTime)> _fechaMesVigente;
        List<(DateTime, DateTime)> _fechasProximas;
        private List<FilaCobro> _filasCobros;
        private int _rangoMesActual;
        private byte[] _csvBytes;

        private CheckBox _chkBoxSeleccionada;

        public TablaPagosPagina(Credito credito)
        {
            InitializeComponent();
            _rangoMesActual = -1;
            _credito = credito;
            CargarPaginaPagos(credito);
            RecuperarCobros();
        }

        private void CargarPaginaPagos(Credito credito)
        {
            rnFolio.Text = credito.folioCredito.ToString();
            rnRFCCliente.Text = credito.Cliente.rfc;
            rnMonto.Text = credito.monto.ToString("F2");
            rnSaldoPendiente.Text = credito.saldoPendiente.ToString("F2");
            rnDeudaExtra.Text = credito.deudaExtra.ToString("F2");
            rnPlazoMeses.Text = credito.CondicionCredito.plazoMeses.ToString();
            rnTasaInteres.Text = credito.CondicionCredito.tasaInteres.ToString();
            llpAplicaIva.Fill = credito.CondicionCredito.tieneIVA ?
                (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F") :
                (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24");
        }

        private void RecuperarCobros()
        {
            Codigo codigo;
            List<Cobro> cobros = new List<Cobro>();

            try
            {
                Cobro[] cobrosRecuperados;
                CobroClient proxy = new CobroClient();
                (cobrosRecuperados, codigo) = proxy.RecuperarCobros(_credito.folioCredito);
                if (cobrosRecuperados != null)
                {
                    cobros = cobrosRecuperados.ToList();
                }
                
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    _cobros = cobros;
                    rnCobrosTotales.Text = _cobros.Count.ToString();
                    CalcularRangosMeses();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
            
        }

        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
            btnGenerarDocumento.IsEnabled = false;
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
            btnGenerarDocumento.IsEnabled = false;
        }

        private void CalcularRangosMeses()
        {
            DateTime fechaHoy = DateTime.Now;
            fechaHoy = fechaHoy.Date;

            DateTime fechaInicialAprobacionCredito = _credito.Dictamen[0].fechaHora;
            int numeroMesesTotales = _credito.CondicionCredito.plazoMeses;
            List<(DateTime, DateTime)> rangos = new List<(DateTime, DateTime)>();

            _fechasPrevias = new List<(DateTime, DateTime)>();
            _fechaMesVigente = new List<(DateTime, DateTime)>();
            _fechasProximas = new List<(DateTime, DateTime)>();

            DateTime rangoInferior = fechaInicialAprobacionCredito;
            DateTime rangoSuperior = fechaInicialAprobacionCredito.AddDays(30);

            for (int i = 0; i < numeroMesesTotales; i++)
            {
                if (fechaHoy > rangoInferior && fechaHoy <= rangoSuperior)
                {
                    _rangoMesActual = i;
                    _fechaMesVigente.Add((rangoInferior, rangoSuperior));
                }
                else if (fechaHoy > rangoSuperior)
                {
                    _fechasPrevias.Add((rangoInferior, rangoSuperior));
                }
                else
                {
                    _fechasProximas.Add((rangoInferior, rangoSuperior));
                }
                rangos.Add((rangoInferior, rangoSuperior));
                rangoInferior = rangoInferior.AddDays(30);
                rangoSuperior = rangoSuperior.AddDays(30);

            }

            _rangosMeses = rangos;
            MostrarRangosMeses();
            GenerarFilasTablaPagos();

            if (_rangoMesActual == -1)
            {
                MostrarCreditoFinalizado();
            }
            else
            {
                chkBoxMesVigente.IsChecked = true;
                chkBoxMesVigente.IsEnabled = false;
                _chkBoxSeleccionada = chkBoxMesVigente;
                string mes = _fechaMesVigente[0].Item1.ToString("MMMM");
                FechaTablaPagos fecha = new FechaTablaPagos
                {
                    PosicionRango = 0,
                    Dia = _fechaMesVigente[0].Item1.ToString("dd"),
                    Mes = char.ToUpper(mes[0]) + mes.Substring(1),
                    Anio = _fechaMesVigente[0].Item1.ToString("yyyy")
                };

                FiltrarCobrosPorMes(fecha, _fechaMesVigente[0], "vigente");
                chkBoxMesVigente.Checked += CheckedMostrarMesVigente;
                chkBoxMesVigente.Unchecked += CheckedMostrarMesVigente;
            }
        }

        private void MostrarRangosMeses()
        {
            List<FechaTablaPagos> fechaVigente = new List<FechaTablaPagos>();
            for (int i = 0; i < _fechaMesVigente.Count; i++)
            {
                string mes = _fechaMesVigente[i].Item1.ToString("MMMM");
                FechaTablaPagos fecha = new FechaTablaPagos
                {
                    PosicionRango = i,
                    Dia = _fechaMesVigente[i].Item1.ToString("dd"),
                    Mes = char.ToUpper(mes[0]) + mes.Substring(1),
                    Anio = _fechaMesVigente[i].Item1.ToString("yyyy")
                };
                fechaVigente.Add(fecha);
            }
            lstBoxMesVigente.ItemsSource = fechaVigente;

            List<FechaTablaPagos> fechasPrevias = new List<FechaTablaPagos>();
            for (int i = 0; i < _fechasPrevias.Count; i++)
            {
                string mes = _fechasPrevias[i].Item1.ToString("MMMM");
                FechaTablaPagos fechaPrevia = new FechaTablaPagos
                {
                    PosicionRango = i,
                    Dia = _fechasPrevias[i].Item1.ToString("dd"),
                    Mes = char.ToUpper(mes[0]) + mes.Substring(1),
                    Anio = _fechasPrevias[i].Item1.ToString("yyyy")
                };
                fechasPrevias.Add(fechaPrevia);
            }
            lstBoxMesesPrevios.ItemsSource = fechasPrevias;

            List<FechaTablaPagos> fechasProximas = new List<FechaTablaPagos>();
            for (int i=0; i<_fechasProximas.Count; i++ )
            {
                string mes = _fechasProximas[i].Item1.ToString("MMMM");
                FechaTablaPagos fechaProxima = new FechaTablaPagos
                {
                    PosicionRango = i,
                    Dia = _fechasProximas[i].Item1.ToString("dd"),
                    Mes = char.ToUpper(mes[0]) + mes.Substring(1),
                    Anio = _fechasProximas[i].Item1.ToString("yyyy")
                };
                fechasProximas.Add(fechaProxima);
            }
            lstBoxMesesProximos.ItemsSource = fechasProximas;

        }

        private void GenerarFilasTablaPagos()
        {
            List<FilaCobro> filas = new List<FilaCobro>();
            DateTime fechaHoy = DateTime.Now;

            double creditoInteres = _credito.monto * ((double)_credito.CondicionCredito.tasaInteres / 100);
            double iva = _credito.CondicionCredito.tieneIVA ? creditoInteres * .16 : 0;
            double totalAPagar = _credito.monto + creditoInteres + iva;
            double restanteAPagar = totalAPagar;

            double pagoMesFijo = (double)totalAPagar / _credito.CondicionCredito.plazoMeses;

            double deudaExtra = 0;
            double faltaPagarEnMesesPrevios = 0;
            double pagadoExtraEnMesesPrevios = 0;

            for (int i = 0; i < _rangosMeses.Count; i++)
            {
                string tipo = ClasificarTipo(fechaHoy, _rangosMeses[i].Item1, _rangosMeses[i].Item2);

                double totalDelMesActual = pagoMesFijo + faltaPagarEnMesesPrevios - pagadoExtraEnMesesPrevios;
                double restanteAPagarPorMes = totalDelMesActual;

                faltaPagarEnMesesPrevios = 0;
                pagadoExtraEnMesesPrevios = 0;

                FilaCobro fila = new FilaCobro 
                {
                    RangoPrimero = _rangosMeses[i].Item1,
                    CobroRepresentado = null, TipoMes = tipo, Dia = _rangosMeses[i].Item1.Day.ToString(),
                    Mes = char.ToUpper(_rangosMeses[i].Item1.ToString("MMMM")[0]) + _rangosMeses[i].Item1.ToString("MMMM").Substring(1),
                    Anio = _rangosMeses[i].Item1.ToString("yyyy"), TotalPagar = totalAPagar,
                    RestanteAPagar = restanteAPagar, RestanteAPagarPorMes = restanteAPagarPorMes, TotalPagarMes = totalDelMesActual, DeudaExtra = deudaExtra
                };

                filas.Add(fila);
                List<Cobro> cobrosDentroRango = _cobros.Where((cobro, posicion) => 
                    (cobro.fecha >= _rangosMeses[i].Item1 && cobro.fecha < _rangosMeses[i].Item2) 
                      ||  (posicion == (_cobros.Count -1) && cobro.fecha == _rangosMeses[i].Item2)).ToList();

                if (cobrosDentroRango.Count > 0)
                {
                    foreach (var cobro in cobrosDentroRango)
                    {
                        restanteAPagar -= (double)cobro.importe;
                        restanteAPagarPorMes -= (double)cobro.importe;

                        FilaCobro filaConCobro = new FilaCobro 
                        {
                            RangoPrimero = _rangosMeses[i].Item1,
                            CobroRepresentado = cobro, TipoMes = tipo, Dia = _rangosMeses[i].Item1.Day.ToString(),
                            Mes = char.ToUpper(_rangosMeses[i].Item1.ToString("MMMM")[0]) + _rangosMeses[i].Item1.ToString("MMMM").Substring(1),
                            Anio = _rangosMeses[i].Item1.ToString("yyyy"), TotalPagar = totalAPagar,
                            RestanteAPagar = restanteAPagar, RestanteAPagarPorMes = restanteAPagarPorMes, TotalPagarMes = totalDelMesActual, DeudaExtra = deudaExtra,
                            Folio = cobro.idCobro.ToString(), Fecha = cobro.fecha.ToString("dd-MM-yyyy"),
                            Importe = "$" + cobro.importe.ToString()
                        };

                        filas.Add(filaConCobro);
                    }
                }

                if (restanteAPagarPorMes > 0 && tipo == "Previo")
                {
                    double deudaMesActual = (double)_credito.monto / _credito.CondicionCredito.plazoMeses * .2;
                    deudaExtra += deudaMesActual;
                    totalAPagar += deudaExtra;
                    restanteAPagar += deudaExtra;
                    faltaPagarEnMesesPrevios += restanteAPagarPorMes + deudaMesActual;
                }

                if (restanteAPagarPorMes < 0)
                {
                    pagadoExtraEnMesesPrevios -= restanteAPagarPorMes;
                }

                if (cobrosDentroRango.Count == 0 && tipo != "Próximo")
                {
                    FilaCobro filaProxima = new FilaCobro 
                    {
                        RangoPrimero = _rangosMeses[i].Item1,
                        CobroRepresentado = null, TipoMes = tipo, Dia = _rangosMeses[i].Item1.Day.ToString(),
                        Mes = char.ToUpper(_rangosMeses[i].Item1.ToString("MMMM")[0]) + _rangosMeses[i].Item1.ToString("MMMM").Substring(1),
                        Anio = _rangosMeses[i].Item1.ToString("yyyy"), TotalPagar = totalAPagar,
                        RestanteAPagar = restanteAPagar, RestanteAPagarPorMes = restanteAPagarPorMes, TotalPagarMes = totalDelMesActual, DeudaExtra = deudaExtra
                    };

                    filas.Add(filaProxima);                
                }
            }
            _filasCobros = filas;
        }

        private string ClasificarTipo(DateTime fechaHoy, DateTime rangoInferior, DateTime rangoSuperior)
        {
            if (fechaHoy > rangoInferior && fechaHoy <= rangoSuperior)
            {
                return "Vigente";
            }
            else if (fechaHoy > rangoSuperior)
            {
                return "Previo";
            }
            else
            {
                return "Próximo";
            }
        }

        private void MostrarCreditoFinalizado()
        {
            chkBocMesesProximos.Style = (Style)FindResource("estiloChcBoxMesSeleccionNoHabilitada");
            chkBocMesesProximos.IsEnabled = false;
            chkBoxMesVigente.Style = (Style)FindResource("estiloChcBoxMesSeleccionNoHabilitada");
            chkBoxMesVigente.IsEnabled = false;

            chkBoxMesVigente.IsChecked = false;
            lstBoxPagosProximos.Visibility = Visibility.Collapsed;
            brdDetallesMes.Visibility = Visibility.Collapsed;
            dtGridCobros.Visibility = Visibility.Collapsed;

            rnFechaFinalizada.Text = _rangosMeses.Last().Item2.ToString("dd-MM-yyyy");
            brdCreditoFinalizado.Visibility = Visibility.Visible;
        }

        private void CheckedMostrarMesVigente(object sender, RoutedEventArgs e)
        {
            CheckBox chkBoxNueva = sender as CheckBox;
            ManejarCajaSeleccion(chkBoxNueva);

            string mes = _fechaMesVigente[0].Item1.ToString("MMMM");
            FechaTablaPagos fecha = new FechaTablaPagos
            {
                PosicionRango = 0,
                Dia = _fechaMesVigente[0].Item1.ToString("dd"),
                Mes = char.ToUpper(mes[0]) + mes.Substring(1),
                Anio = _fechaMesVigente[0].Item1.ToString("yyyy")
            };

            FiltrarCobrosPorMes(fecha, _fechaMesVigente[0], "vigente");

        }
        private void CheckedMostrarMesPrevio(object sender, RoutedEventArgs e)
        {
            CheckBox chkBoxNueva = sender as CheckBox;
            ManejarCajaSeleccion(chkBoxNueva);

            FechaTablaPagos fecha = chkBoxNueva.DataContext as FechaTablaPagos;
            FiltrarCobrosPorMes(fecha, _fechasPrevias[fecha.PosicionRango], "previo");
        }

        private void ManejarCajaSeleccion(CheckBox chkBoxNueva)
        {
            if (chkBoxNueva.IsChecked == false)
            {
                return;
            }

            if (_chkBoxSeleccionada != null && chkBoxNueva != _chkBoxSeleccionada)
            {
                _chkBoxSeleccionada.IsChecked = false;
                _chkBoxSeleccionada.IsEnabled = true;
            }

            _chkBoxSeleccionada = chkBoxNueva;
            _chkBoxSeleccionada.IsEnabled = false;
        }

        private void MostrarNoHayCobrosRegistrados(string mes)
        {
            dtGridCobros.ItemsSource = null;
            dtGridCobros.Items.Refresh();
            brdSinCobrosRegistrados.Visibility = Visibility.Visible;
            rnMesSinCobros.Text = mes;
        }

        private void FiltrarCobrosPorMes(FechaTablaPagos fecha, (DateTime, DateTime) rangoMes, string tipoMes)
        {
            if (fecha != null)
            {
                brdCreditoFinalizado.Visibility = Visibility.Collapsed;
                brdSinCobrosRegistrados.Visibility = Visibility.Collapsed;
                dtGridCobros.Visibility = Visibility.Visible;
                brdDetallesMes.Visibility = Visibility.Visible;
                lstBoxPagosProximos.Visibility = Visibility.Collapsed;

                txtBlockDiaMesActual.Text = fecha.Dia + " " + fecha.Mes;
                txtBlockAnioActual.Text = fecha.Anio;
                rnTipoMesActual.Text = tipoMes;

                foreach (var filaCobro in _filasCobros)
                {
                    if (filaCobro.RangoPrimero == rangoMes.Item1)
                    {
                        rnTotalPagarPorMes.Text = String.Format("{0:0.00}", filaCobro.TotalPagarMes);
                        rnRestanteAPagarPorMes.Text = String.Format("{0:0.00}", filaCobro.RestanteAPagarPorMes);
                        rnDeudaAcumuladaPorMes.Text = String.Format("{0:0.00}", filaCobro.DeudaExtra);
                        break;
                    }
                }

                List<FilaCobro> filasMesSeleccionado = FiltrarFilasCobrosPorMes(rangoMes);

                if (filasMesSeleccionado.Count == 0)
                {
                    MostrarNoHayCobrosRegistrados(fecha.Mes);
                }
                else
                {
                    MostrarCobrosMes(filasMesSeleccionado);
                }
            }
        }
        private List<FilaCobro> FiltrarFilasCobrosPorMes((DateTime, DateTime) rango)
        {
            List<FilaCobro> filas = new List<FilaCobro>();
            foreach (FilaCobro fila in _filasCobros)
            {
                bool esRango = (fila.CobroRepresentado != null && ((fila.CobroRepresentado.fecha >= rango.Item1 && fila.CobroRepresentado.fecha < rango.Item2)
                        || (fila.CobroRepresentado.idCobro == _cobros[_cobros.Count - 1].idCobro && fila.CobroRepresentado.fecha == rango.Item2)));

                if (esRango)
                {
                    filas.Add(fila);
                }
            }
            return filas;
        }

        private void MostrarCobrosMes(List<FilaCobro> filasMesPrevio)
        {
            if (filasMesPrevio.Count > 0)
            {
                rnRestanteAPagarPorMes.Text = String.Format("{0:0.00}", filasMesPrevio[filasMesPrevio.Count - 1].RestanteAPagarPorMes);
            }

            dtGridCobros.ItemsSource = filasMesPrevio;
            dtGridCobros.Items.Refresh();
        }

        private void CheckedMostrarMesesProximos(object sender, RoutedEventArgs e)
        {
            CheckBox chkBoxNueva = sender as CheckBox;
            ManejarCajaSeleccion(chkBoxNueva);

            List<FilaCobro> filasProximos = FiltrarPagosMesesProximos();
            MostrarPagosMesesProximos(filasProximos);
        }

        private List<FilaCobro> FiltrarPagosMesesProximos()
        {
            List<FilaCobro> filasMesesProximos = new List<FilaCobro>();
            foreach (var filaCobro in _filasCobros)
            {
                bool esProxima = _fechasProximas.Exists(rango => filaCobro.RangoPrimero == rango.Item1);
                if (esProxima)
                {
                    filasMesesProximos.Add(filaCobro);
                }
            }
            return filasMesesProximos;
        }

        private void MostrarPagosMesesProximos(List<FilaCobro> filasProximos)
        {
            dtGridCobros.ItemsSource = null;
            dtGridCobros.Items.Refresh();

            dtGridCobros.Visibility = Visibility.Collapsed;
            brdDetallesMes.Visibility = Visibility.Collapsed;

            lstBoxPagosProximos.Visibility = Visibility.Visible;
            lstBoxPagosProximos.ItemsSource = filasProximos;
            lstBoxPagosProximos.Items.Refresh();
        }
        private void ClicGenerarDocumento(object sender, RoutedEventArgs e)
        {
            if (_filasCobros != null && _filasCobros.Count > 0)
            {
                _csvBytes = GeneracionTablaPagos.GenerarTablaPagos(_filasCobros);

                if (_csvBytes == null)
                {
                    
                }
                else
                {
                    btnGenerarDocumento.Click -= ClicGenerarDocumento;
                    btnGenerarDocumento.Content = "TABLA_PAGOS_DEL_CREDITO_" + _credito.folioCredito + "_FECHA_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
                    btnGenerarDocumento.Style = (Style)FindResource("estiloBtnArchivoAdjuntado");
                }
            }
        }

        private void ClicDescargarTablaPagos(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ruta = dialog.SelectedPath + btn.Content;

                File.WriteAllBytes(ruta, _csvBytes);
                MostrarVentanaExitoDescarga();

            }
        }
        private void MostrarVentanaExitoDescarga()
        {
            VentanaMensaje ventana = new VentanaMensaje("Se ha descargado la tabla de pagos exitosamente", Mensaje.EXITO);
            ventana.Mostrar();
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        
    }

    public class FechaTablaPagos
    {
        public int PosicionRango { get; set; }
        public string Dia { get; set; }
        public string Mes { get; set; }
        public string Anio { get; set; }

    }
}
