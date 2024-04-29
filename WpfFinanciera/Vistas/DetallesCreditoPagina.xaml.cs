using Microsoft.Win32;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfFinanciera.ServicioFinancieraIndependiente;
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para DetallesCreditoPagina.xaml
    /// </summary>
    public partial class DetallesCreditoPagina : Page
    {
        private Credito _credito;
        private byte[] _dictamen;

        public DetallesCreditoPagina(Credito credito)
        {
            InitializeComponent();
            CargarDetallesCredito(credito);
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            ListaCreditosPagina lista = new ListaCreditosPagina();
            MainWindow main = (MainWindow)Window.GetWindow(this);
            main.CambiarPagina(lista);
        }

        private void CargarDetallesCredito(Credito credito)
        {
            RecuperarDetallesCredito(credito.folioCredito);
        }

        private void RecuperarDetallesCredito(int folio)
        {
            _credito = new Credito();
            Codigo codigo;
            try
            {
                Credito creditoRecuperado;
                CreditoClient proxy = new CreditoClient();
                (creditoRecuperado, codigo) = proxy.RecuperarTodosDetallesCredito(folio);

                if (creditoRecuperado != null)
                {
                    _credito = creditoRecuperado;
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
                    CargarDetallesCredito();
                    if (_credito.Dictamen == null)
                    {
                        DeshabilitarBotones();
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
        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje ventana = new VentanaMensaje("Error. No se pudo conectar con el servidor.Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventana.Mostrar();
            DeshabilitarBotonesError();
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje ventana = new VentanaMensaje("Error. No se pudo conectar con la base de datos.Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventana.Mostrar();
            DeshabilitarBotonesError();
        }


        private void CargarDetallesCredito()
        {
            txtBlockRfc.Text = _credito.Cliente.rfc;
            txtBlockNombre.Text = _credito.Cliente.nombres + " " + _credito.Cliente.apellidos;

            txtBlockEstadoCondicionCredito.Text = (_credito.CondicionCredito.estaActiva) ? "Activa" : "No activa";
            llpCondicionCreditoEstado.Fill = _credito.CondicionCredito.estaActiva ?
                (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F") :
                (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24");

            txtBlockIdentificadorCondicion.Text = _credito.CondicionCredito.identificador;
            txtBlockDescripcionCondicion.Text = _credito.CondicionCredito.descripcion;
            txtBlockPlazoMesesCondicion.Text = _credito.CondicionCredito.plazoMeses.ToString();
            rnTasaInteres.Text = _credito.CondicionCredito.tasaInteres.ToString();
            txtBlockAplicaIva.Text = (_credito.CondicionCredito.tieneIVA) ? "Aplica" : "No aplica";
            llpAplicaIva.Fill = _credito.CondicionCredito.tieneIVA ?
                (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F") :
                (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24");

            txtBlockFolio.Text = _credito.folioCredito.ToString();
            txtBlockFecha.Text = _credito.fechaSolicitud.ToString("dd/MM/yyyy HH:mm");
            txtBlockEstatusCodigo.Text = _credito.EstatusCredito.nombre;
            rnMonto.Text = _credito.monto.ToString("#,##0");
            rnSaldoPendiente.Text = _credito.saldoPendiente.ToString("#,##0"); ;
            rnDeudaExtra.Text = _credito.deudaExtra.ToString("#,##0");

            if (_credito.Dictamen != null)
            {
                txtBlockEstadoDictamen.Text = (_credito.Dictamen[0].estaAprobado) ? "Activo" : "No activo";
                llpEstadoDictamen.Fill = _credito.Dictamen[0].estaAprobado ?
                    (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F") :
                    (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24");
                txtBlockFechaDictamen.Text = _credito.Dictamen[0].fechaHora.ToString("dd/MM/yyyy HH:mm");
                txtBlockObservacionesDictamen.Text = _credito.Dictamen[0].observaciones.ToString();
                txtBlockUsuarioDictamen.Text = _credito.Dictamen[0].Usuario.nombres + " " + _credito.Dictamen[0].Usuario.apellidos;

                if (!_credito.Dictamen[0].estaAprobado)
                {
                    btnConsultarTablaPagos.IsEnabled = false;
                }
            }
        }

        private void DeshabilitarBotones()
        {
            btnConsultarTablaPagos.IsEnabled = false;
            btnGenerarDocumento.IsEnabled = false;
            llpEstadoDictamen.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#939393");
        }

        private void ClicGenerarDocumento(object sender, RoutedEventArgs e)
        {
            GenerarDocumento();
        } 

        private void GenerarDocumento()
        {
            _dictamen = GeneracionDictamen.GenerarDictamen(_credito);
            MostrarDescargaDocumento();
        }

        private void MostrarDescargaDocumento()
        {
            btnGenerarDocumento.Content = "DICTAMEN_CREDITO_" + _credito.folioCredito + ".pdf";
            btnGenerarDocumento.Style = (Style)FindResource("estiloBtnArchivoAdjuntado");
            btnGenerarDocumento.Click -= ClicGenerarDocumento;
            btnGenerarDocumento.Click += ClicDescargarDocumento;
        }

        private void ClicDescargarDocumento(object sender, RoutedEventArgs e)
        {
            DescargarDocumento();
        }

        private void DescargarDocumento()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string ruta = dialog.SelectedPath + "/" + btnGenerarDocumento.Content;
                File.WriteAllBytes(ruta, _dictamen);
                MostrarVentanaExitoDescarga();

            }
        }

        private void MostrarVentanaExitoDescarga()
        {
            VentanaMensaje ventana = new VentanaMensaje("Se ha descargado el documento del dictamen exitosamente", Mensaje.EXITO);
            ventana.Mostrar();
        }

        private void DeshabilitarBotonesError()
        {
            btnConsultarTablaPagos.IsEnabled = false;
            btnGenerarDocumento.IsEnabled = false;
            llpAplicaIva.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#939393");
            llpEstadoDictamen.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#939393");
            llpCondicionCreditoEstado.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#939393");
        }
    }
}
