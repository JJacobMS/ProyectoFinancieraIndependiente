using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using static QuestPDF.Helpers.Colors;
using static WpfFinanciera.Vistas.VerEficienciasPagina;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para VerEficienciasPagina.xaml
    /// </summary>
    public partial class VerEficienciasPagina : Page
    {
        private List<CategoriasGraficoPastel> Categorias { get; set; }
        private List<CreditoEficiencias> _creditosEficiencias;
        private Cobro[] _arrayCobros;
        private DateTime _fechaAntigua;
        private CheckBox _chkBoxSeleccionada;
        private string _mesActual;
        private Cobro[] _cobros;
        private DateTime _fechaActual;
        private double _montoTotalCobros = 0;
        private double _cobrosRealizados = 0;
        private double _montoEsperado = 0;
        private double _contadorCobros = 0;
        private List<CreditoEficiencias> _listaDeCreditos;
        public VerEficienciasPagina()
        {
            InitializeComponent();
            _fechaActual = DateTime.Now;
            RecuperarMeses();
            chkBoxMesVigente.Checked += CheckedMostrarMesVigente;
            chkBoxMesVigente.Unchecked += CheckedMostrarMesVigente;
        }

        private void RecuperarMeses()
        {
            Codigo codigo = new Codigo();
            bool creditos = false;
            try
            {
                CobroClient proxy = new CobroClient();
                (codigo, _fechaAntigua, creditos) = proxy.RecuperarMeses();
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            switch (codigo)
            {
                case Codigo.EXITO:
                    if (creditos == false)
                    {
                        MostrarVentanaCreditosNoExiste();
                    }
                    else
                    {
                        CargarMeses();
                        RecuperarCobros();
                    }
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }
        
        private void CargarMeses()
        {
            List<DateTime> listaMeses = new List<DateTime>();

            DateTime primerDiaDelMes = new DateTime(_fechaAntigua.Year, _fechaAntigua.Month, 1);

            DateTime primerDiaDelMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            for (DateTime fechaIterada = primerDiaDelMes; fechaIterada <= primerDiaDelMesActual; fechaIterada = fechaIterada.AddMonths(1))
            {
                listaMeses.Add(fechaIterada);
            }

            List<FechaTablaPagos> fechasPrevias = new List<FechaTablaPagos>();
            for (int i = 0; i < listaMeses.Count; i++)
            {
                string mes = listaMeses[i].ToString("MMMM");
                FechaTablaPagos fechaPrevia = new FechaTablaPagos
                {
                    PosicionRango = i,
                    Dia = listaMeses[i].ToString("dd"),
                    Mes = char.ToUpper(mes[0]) + mes.Substring(1),
                    Anio = listaMeses[i].ToString("yyyy")
                };
                fechasPrevias.Add(fechaPrevia);
            }

            List<FechaTablaPagos> fechasActuales = new List<FechaTablaPagos>();
            _mesActual = primerDiaDelMesActual.ToString("MMMM");
            FechaTablaPagos fechaActual = new FechaTablaPagos
            {
                PosicionRango = 1,
                Mes = char.ToUpper(_mesActual[0]) + _mesActual.Substring(1),
                Anio = primerDiaDelMesActual.ToString("yyyy")
            };
            fechasActuales.Add(fechaActual);

            lstBoxMesesPrevios.ItemsSource = fechasPrevias;
            lstBoxMesVigente.ItemsSource = fechasActuales;

        }

        private void RecuperarCobros()
        {
            lstBoxCreditos.ItemsSource = null;
            lstBoxCreditos.Items.Clear();
            _arrayCobros = new Cobro[] { };
            Codigo codigo = new Codigo();
            _cobros = new Cobro[] { };
            try
            {
                CobroClient proxy = new CobroClient();
                (codigo, _cobros) = proxy.RecuperarCobrosPorFecha(_fechaActual);
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            switch (codigo)
            {
                case Codigo.EXITO:
                    if (_arrayCobros == null)
                    {
                        MostrarVentanaCobrosNoExisten();
                    }
                    else
                    {
                        CargarCobros();
                    }
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }

        private void FiltrarCampos()
        {
            lstBoxCreditos.ItemsSource = null;
            lstBoxCreditos.Items.Clear();
            int folioBusqueda;
            int.TryParse(txtBoxBarraBuscar.Text, out folioBusqueda);
            if (_creditosEficiencias != null && _creditosEficiencias.Count() > 0 && txtBoxBarraBuscar.Text!="")
            {
                txtBoxBarraBuscar.Text = "";
                CreditoEficiencias[] politicasFiltradasNombre = _creditosEficiencias.Where(politica => politica.folio == folioBusqueda).ToArray();
                List<CreditoEficiencias> politicasUnicas = politicasFiltradasNombre.ToList();
                lstBoxCreditos.ItemsSource = politicasUnicas;
            }
            else if (_creditosEficiencias != null && _creditosEficiencias.Count() > 0)
            {
                lstBoxCreditos.ItemsSource = _listaDeCreditos;
            }
        }

        private void MostrarVentanaCobrosNoExisten() 
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No existen cobros en este mes, Inténtelo más tarde", Mensaje.ERROR);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaCreditosNoExiste()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error.No existen créditos actualmente, Inténtelo más tarde", Mensaje.ERROR);
            mensajeError.Mostrar();
        }

        private void CargarCobros()
        {

            Dictionary<int, CreditoEficiencias> eficienciasDict = new Dictionary<int, CreditoEficiencias>();
            if (_cobros != null)
            {
                for (int i = 0; i < _cobros.Length; i++)
                {
                    var cobro = _cobros[i];
                    if (eficienciasDict.ContainsKey(cobro.Credito_folioCredito))
                    {
                        eficienciasDict[cobro.Credito_folioCredito].montoTotalCobros += cobro.importe;
                        eficienciasDict[cobro.Credito_folioCredito].cobrosRealizados++;
                    }
                    else
                    {
                        double monto = 0;
                        try
                        {
                            CreditoClient proxy = new CreditoClient();
                            monto = proxy.CalcularMontoEsperado(cobro.Credito_folioCredito);
                        }
                        catch (CommunicationException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        catch (TimeoutException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        var eficiencia = new CreditoEficiencias
                        {
                            folio = cobro.Credito_folioCredito,
                            montoTotalCobros = cobro.importe,
                            cobrosRealizados = 1,
                            montoEsperado = monto
                        };
                        eficienciasDict.Add(eficiencia.folio, eficiencia);
                    }
                }
            }
            _montoTotalCobros = 0;
            _cobrosRealizados = 0;
            _montoEsperado = 0;
            _contadorCobros = 0;
            _creditosEficiencias = new List<CreditoEficiencias>() { };
            foreach (var eficiencia in eficienciasDict.Values)
            {
                _montoTotalCobros = _montoTotalCobros + eficiencia.montoTotalCobros;
                _cobrosRealizados = _cobrosRealizados + eficiencia.cobrosRealizados;
                _montoEsperado = _montoEsperado + eficiencia.montoEsperado;
                _contadorCobros = _contadorCobros + 1;
                Console.WriteLine($"Folio: {eficiencia.folio}, Monto total de cobros: {eficiencia.montoTotalCobros}, Cobros realizados: {eficiencia.cobrosRealizados}, Monto esperado: {eficiencia.montoEsperado}");
                _creditosEficiencias.Add(eficiencia);
            }

            if (_creditosEficiencias != null && _creditosEficiencias.Count > 0)
            {
                _listaDeCreditos = new List<CreditoEficiencias>() { };
                lstBoxCreditos.ItemsSource = _listaDeCreditos;
                _listaDeCreditos = new List<CreditoEficiencias>();
                foreach (CreditoEficiencias creditosEficiencias in _creditosEficiencias)
                {
                    _listaDeCreditos.Add(creditosEficiencias);
                }
                lstBoxCreditos.ItemsSource = _listaDeCreditos;
                this.DataContext = this;
                txtBlockMontoTotal.Text = "$" + _montoTotalCobros;
                txtBlockMontoEsperado.Text = "$" + _montoEsperado;
                txtBlockCobrosTotales.Text = "" + _cobrosRealizados;
                CargarGrafico();
            }
        }

        private void CargarGrafico()
        {
            mainCanvas.Children.Clear();
            double porcentajeEficiencia = 0;
            try
            {
                porcentajeEficiencia = (_montoTotalCobros / _montoEsperado) * 100;
                if (porcentajeEficiencia >= 100)
                {
                    porcentajeEficiencia = 100;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                porcentajeEficiencia = 100;
            }
            porcentajeEficiencia = Math.Round(porcentajeEficiencia, 2);
            Categorias = new List<CategoriasGraficoPastel>()
            {
                new CategoriasGraficoPastel
                {
                    Titulo = "Porcentaje de Eficiencia",
                    Porcentaje = porcentajeEficiencia,
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#146081")),
                },

                new CategoriasGraficoPastel
                {
                    Titulo = "Porcentaje de Falta de Cobro",
                    Porcentaje = Math.Round(100 - porcentajeEficiencia, 2),
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E77032")),
                },
            };
            float ancho = 190, largo = 76, centroX = ancho / 2, centroY = largo / 2, radio = ancho / 2;
            mainCanvas.Width = ancho;
            mainCanvas.Height = largo;

            detailsItemsControl.ItemsSource = Categorias;
            double angulo = 0, anguloPrevio = 0;
            foreach (var categoria in Categorias)
            {
                if (categoria.Porcentaje < 100)
                {
                    double linea1X = (radio * Math.Cos(angulo * Math.PI / 180)) + centroX;
                    double linea1Y = (radio * Math.Sin(angulo * Math.PI / 180)) + centroY;

                    angulo = categoria.Porcentaje * (float)360 / 100 + anguloPrevio;
                    double arcoX = (radio * Math.Cos(angulo * Math.PI / 180)) + centroX;
                    double arcoY = (radio * Math.Sin(angulo * Math.PI / 180)) + centroY;

                    var lineaSegmento1 = new LineSegment(new Point(linea1X, linea1Y), false);
                    double arcWidth = radio, arcHeight = radio;
                    bool isLargeArc = categoria.Porcentaje > 50;
                    var segmento = new ArcSegment()
                    {
                        Size = new Size(arcWidth, arcHeight),
                        Point = new Point(arcoX, arcoY),
                        SweepDirection = SweepDirection.Clockwise,
                        IsLargeArc = isLargeArc,
                    };
                    var line2Segment = new LineSegment(new Point(centroX, centroY), false);

                    var figura = new PathFigure(
                        new Point(centroX, centroY),
                        new List<PathSegment>()
                        {
                            lineaSegmento1,
                            segmento,
                            line2Segment,
                        },
                        true);

                    var figuras = new List<PathFigure>() { figura, };
                    var geometria = new PathGeometry(figuras);
                    var path = new System.Windows.Shapes.Path()
                    {
                        Fill = categoria.Color,
                        Data = geometria,
                    };
                    mainCanvas.Children.Add(path);

                    anguloPrevio = angulo;


                    var linea1 = new Line()
                    {
                        X1 = centroX,
                        Y1 = centroY,
                        X2 = lineaSegmento1.Point.X,
                        Y2 = lineaSegmento1.Point.Y,
                        Stroke = Brushes.White,
                        StrokeThickness = 1,
                    };
                    var linea2 = new Line()
                    {
                        X1 = centroX,
                        Y1 = centroY,
                        X2 = segmento.Point.X,
                        Y2 = segmento.Point.Y,
                        Stroke = Brushes.White,
                        StrokeThickness = 1,
                    };

                    mainCanvas.Children.Add(linea1);
                    mainCanvas.Children.Add(linea2);
                }
                else
                {
                    // Si el porcentaje es 100%, dibujar un círculo completo
                    var geometria = new EllipseGeometry(new Point(centroX, centroY), radio, radio);
                    var path = new System.Windows.Shapes.Path()
                    {
                        Fill = categoria.Color,
                        Data = geometria,
                    };
                    mainCanvas.Children.Add(path);
                }
            }
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            EliminarCobros();
            NavigationService.GoBack();
        }

        private void EliminarCobros()
        {
            try
            {
                CobroClient proxy = new CobroClient();
                proxy.EliminarCobrosVacios();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void CheckedMostrarMesPrevio(object sender, RoutedEventArgs e)
        {
            CheckBox chkBoxNueva = sender as CheckBox;
            ManejarCajaSeleccion(chkBoxNueva);
            FechaTablaPagos fechaSeleccionada = new FechaTablaPagos();
            fechaSeleccionada = chkBoxNueva.DataContext as FechaTablaPagos;

            int numeroMes = DateTime.ParseExact(fechaSeleccionada.Mes, "MMMM", System.Globalization.CultureInfo.CurrentCulture).Month;

            _fechaActual = new DateTime(int.Parse(fechaSeleccionada.Anio), numeroMes, 1);

            RecuperarCobros();

        }

        private void CheckedMostrarMesVigente(object sender, RoutedEventArgs e)
        {
            CheckBox chkBoxNueva = sender as CheckBox;
            ManejarCajaSeleccion(chkBoxNueva);
            _fechaActual = DateTime.Now;
            RecuperarCobros();
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

        public class CreditoEficiencias
        {
            public CreditoEficiencias()
            {
            }

            public int folio { get; set; }
            public double cobrosRealizados { get; set; }
            public double montoEsperado { get; set; }
            public double montoTotalCobros { get; set; }

        }

        private void ClicBuscarCredito(object sender, RoutedEventArgs e)
        {
            FiltrarCampos();
        }
    }
}
