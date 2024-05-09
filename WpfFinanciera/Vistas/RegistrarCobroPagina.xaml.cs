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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfFinanciera.ServicioFinancieraIndependiente;
using WpfFinanciera.Utilidades;


namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para RegistrarCobroPagina.xaml
    /// </summary>
    public partial class RegistrarCobroPagina : Page
    {
        private bool _sonCamposValidos;
        private bool _esCreditoValido;
        private string _rutaArchivo;
        private string _primeraLinea;
        private int _folio;
        private double _numeroImporte;
        private DateTime _fechaImporte;
        private Credito _credito;
        private string[] _filaArchivo = new string[0];
        public RegistrarCobroPagina()
        {
            InitializeComponent();
            _sonCamposValidos = false;
            _esCreditoValido = false;
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            MenuPrincipalAsesorCreditoPagina menu = new MenuPrincipalAsesorCreditoPagina();
            ventanaPrincipal.CambiarPagina(menu);
        }

        private void ClicAgregarCobroRealizado(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
            AgregarCobro();
        }

        private void LimpiarCampos()
        {
            txtFolioCredito.Text = "";
            txtImporteCredito.Text = "";
            txtFechaImporte.Text = "";
            txtNombreCliente.Text = "";
            txtSaldoPendiente.Text = "";
            _filaArchivo = new string[0];
            _sonCamposValidos = false;
            _esCreditoValido = false;
            _rutaArchivo = "";
            _primeraLinea = "";
            _folio = new int();
            _numeroImporte = new int();
            _credito = new Credito();
       }

        private void AgregarCobro()
        {
            try
            {
                OpenFileDialog abrirBiblioteca = new OpenFileDialog();
                abrirBiblioteca.Filter = $"Archivo csv|*.csv";
                if (abrirBiblioteca.ShowDialog() == DialogResult.OK)
                {
                    _rutaArchivo = abrirBiblioteca.FileName;
                    System.IO.StreamReader archivo = new System.IO.StreamReader(_rutaArchivo);
                    archivo.ReadLine();
                    _primeraLinea = archivo.ReadLine();
                    char separador = ',';
                    int cantidadElementos = _primeraLinea != null ? _primeraLinea.Split(separador).Length : 0;
                    _filaArchivo = new string[4] { "", "", "", "" };
                    for (int i = 0; i < cantidadElementos; i++)
                    {
                        _filaArchivo[i] = _primeraLinea.Split(separador)[i];
                    }
                    _sonCamposValidos = ValidarCampos();
                    RecuperarCredito();
                }
            }
            catch (ArgumentException ex)
            {
                MostrarVentanaErrorArchivo();
                Console.WriteLine(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                MostrarVentanaErrorArchivo();
                Console.WriteLine(ex);
            }
            catch (IOException ex)
            {
                MostrarVentanaErrorArchivo();
                Console.WriteLine(ex);
            }
        }

        private void CargarCobro()
        {
            double.TryParse(_filaArchivo[2], out double importe);
            txtFolioCredito.Text = _filaArchivo[1];
            txtImporteCredito.Text = _filaArchivo[2];
            txtFechaImporte.Text = _filaArchivo[3];
            _esCreditoValido = ValidarFolio();
            if (_esCreditoValido)
            {
                double total = _credito.saldoPendiente - importe;
                txtNombreCliente.Text = _credito.Cliente.nombres + " " + _credito.Cliente.apellidos;
                txtSaldoPendiente.Text = "Saldo pendiente: " + _credito.saldoPendiente + " - " + importe + " = " + total;
            }
        }

        private bool ValidarCampos()
        {
            bool sonCamposValidos = true;

            string lineaCaptura = _filaArchivo[0] ?? "";
            string folio = _filaArchivo[1] ?? "";
            string importe = _filaArchivo[2] ?? "";
            string fechaImporte = _filaArchivo[3] ?? "";
            string razones = "";
            if (int.TryParse(folio, out _folio))
            {
                txtFolioCredito.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            else
            {
                sonCamposValidos = false;
                txtFolioCredito.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                razones += "Folio Credito";
            }
            if (double.TryParse(importe, out _numeroImporte))
            {
                txtImporteCredito.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            else
            {
                sonCamposValidos = false;
                txtImporteCredito.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                razones = (razones.Length > 0) ? razones + ", Importe" : "Importe";
            }
            if (DateTime.TryParse(fechaImporte, out _fechaImporte) && _fechaImporte <= DateTime.Now)
            {
                txtFechaImporte.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            else
            {
                sonCamposValidos = false;
                txtFechaImporte.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                razones = (razones.Length > 0) ? razones + ", Fecha importe" : "Fecha importe";
            }
            if (!sonCamposValidos)
            {
                MostrarVentanaCamposNoValidos(razones);
            }
            return sonCamposValidos;
        }

        private void MostrarVentanaCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaErrorArchivo()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No se puede cargar el archivo .csv, Intentelo mas tarde", Mensaje.ERROR);
            mensajeError.Mostrar();
        }

        private void ClicGuardar(object sender, RoutedEventArgs e)
        {
            if (_sonCamposValidos && _esCreditoValido)
            {
                VentanaMensaje ventana;
                ventana = new VentanaMensaje("¿Desea registrar el Cobro?", Mensaje.CONFIRMACION);
                if (ventana.MostrarConfirmacion())
                {
                    GuardarCobro();
                }
            }
        }

        private void GuardarCobro()
        {
            Codigo codigo = new Codigo();
            bool seActualizo = false;
            try
            {
                Cobro cobro = new Cobro
                {
                    Credito_folioCredito = _folio,
                    importe = _numeroImporte,
                    fecha = _fechaImporte
                };
                CobroClient proxy = new CobroClient();
                codigo = proxy.GuardarCobro(cobro);
                CreditoClient proxyCredito= new CreditoClient();
                (codigo, seActualizo) = proxyCredito.ActualizarCreditoDeudaExtra(_folio);
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
                    MostrarVentanaExito();
                    LimpiarCampos();
                    if (seActualizo)
                    {
                        VentanaMensaje mensajeExito;
                        mensajeExito = new VentanaMensaje("El cobro ha excedido la fecha límite de la tabla de pagos, se agregará deuda moratoria", Mensaje.EXITO);
                        mensajeExito.Mostrar();
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

        private void RecuperarCredito() 
        {
            Codigo codigo = new Codigo();
            _credito = new Credito();
            try
            {
                CreditoClient proxy = new CreditoClient();
                (_credito, codigo) = proxy.RecuperarCredito(_folio);
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
                    CargarCobro();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }

        private bool ValidarFolio()
        {
            bool sonCamposValidos = true;
            if (_credito == null)
            {
                txtFolioCredito.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                txtNombreCliente.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                txtSaldoPendiente.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                VentanaMensaje mensajeError = new VentanaMensaje("Error. No existe un crédito asociado a este folio", Mensaje.ERROR);
                mensajeError.Mostrar();
                sonCamposValidos = false;
            }
            else if (_credito.EstatusCredito_idEstatusCredito != 2)
            {
                txtFolioCredito.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                txtNombreCliente.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                txtSaldoPendiente.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                VentanaMensaje mensajeError = new VentanaMensaje("Error. El crédito asociado a este folio no se encuentra aprobado", Mensaje.ERROR);
                mensajeError.Mostrar();
                sonCamposValidos = false;
            }
            else
            {
                txtFolioCredito.Style = (Style)FindResource("estiloTxtBoxFormulario");
                txtNombreCliente.Style = (Style)FindResource("estiloTxtBoxFormulario");
                txtSaldoPendiente.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            return sonCamposValidos;
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

        private void MostrarVentanaExito()
        {
            VentanaMensaje mensajeExito;
            mensajeExito = new VentanaMensaje("Se ha actualizado el cobro exitosamente", Mensaje.EXITO);
            mensajeExito.Mostrar();
        }
    }
}
