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
    /// Lógica de interacción para DetallesClientePagina.xaml
    /// </summary>
    public partial class DetallesClientePagina : Page
    {
        private Dictionary<string, Button> _botonesTipoArchivo = new Dictionary<string, Button>();

        private Cliente _clienteDetalle;
        public DetallesClientePagina(Cliente cliente)
        {
            InitializeComponent();
            _botonesTipoArchivo.Add("Identificación Oficial", btnDescargarIdentificacion);
            _botonesTipoArchivo.Add("Comprobante Domicilio", btnDescargarComprobanteDomicilio);
            _botonesTipoArchivo.Add("Comprobante Ingreso", btnDescargarComprobanteIngreso);
            _botonesTipoArchivo.Add("Comprobante Trabajo", btnDescargarComprobanteTrabajo);
            _botonesTipoArchivo.Add("Referencia Cliente 1", btnDescargarReferenciaCliente1);
            _botonesTipoArchivo.Add("Referencia Cliente 2", btnDescargarReferenciaCliente2);
            CargarDetallesCliente(cliente);
        }

        private void CargarDetallesCliente(Cliente cliente)
        {
            RecuperarDetallesCliente(cliente.rfc);
        }

        private void RecuperarDetallesCliente(string rfc)
        {
            _clienteDetalle = new Cliente();
            Codigo codigo;
            try
            {
                Cliente clienteRecuperado;
                ClienteClient proxy = new ClienteClient();
                (clienteRecuperado, codigo) = proxy.RecuperarTodosDetallesCliente(rfc);

                if (clienteRecuperado != null)
                {
                    _clienteDetalle = clienteRecuperado;
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
                    CargarDetallesCliente();
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
            DeshabilitarBotones();
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje ventana = new VentanaMensaje("Error. No se pudo conectar con la base de datos.Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventana.Mostrar();
            DeshabilitarBotones();
        }

        private void CargarDetallesCliente()
        {
            txtBoxRFCCliente.Text = _clienteDetalle.rfc;
            txtBoxNombreCliente.Text = _clienteDetalle.nombres;
            txtBoxApellidoCliente.Text = _clienteDetalle.apellidos;
            txtBoxCorreoCliente.Text = _clienteDetalle.correoElectronico;
            txtBoxDireccionCliente.Text = _clienteDetalle.direccion;

            txtBlockEstadoCliente.Text = (_clienteDetalle.esDeudor) ? "Es deudor" : "No es deudor";
            llpEstadoCliente.Fill = _clienteDetalle.esDeudor ?
                (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24") :
                (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F");

            txtBoxTelefonoCasaCliente.Text = _clienteDetalle.Telefono[0].numeroTelefonico;
            txtBoxTelefonoPersonalCliente.Text = _clienteDetalle.Telefono[1].numeroTelefonico;
            txtBoxCuentaCobroCliente.Text = _clienteDetalle.cuentaCobro;
            txtBoxCuentaDepositoCliente.Text = _clienteDetalle.cuentaDeposito;

            txtBoxNombreReferenciaTrabajo.Text = _clienteDetalle.ReferenciaTrabajo.nombre;
            txtBoxTelefonoReferenciaTrabajo.Text = _clienteDetalle.ReferenciaTrabajo.telefono;
            txtBoxDireccionReferenciaTrabajo.Text = _clienteDetalle.ReferenciaTrabajo.direccion;

            txtBoxNombreReferenciaCliente1.Text = _clienteDetalle.ReferenciaCliente[0].nombres;
            txtBoxDescripcionReferenciaCliente1.Text = _clienteDetalle.ReferenciaCliente[0].descripcion;
            txtBoxTelefonoReferenciaCliente1.Text = _clienteDetalle.ReferenciaCliente[0].telefono;

            txtBoxNombreReferenciaCliente2.Text = _clienteDetalle.ReferenciaCliente[1].nombres;
            txtBoxDescripcionReferenciaCliente2.Text = _clienteDetalle.ReferenciaCliente[1].descripcion;
            txtBoxTelefonoReferenciaCliente2.Text = _clienteDetalle.ReferenciaCliente[1].telefono;

            dtGridHistorialCreditos.ItemsSource = _clienteDetalle.Credito;

            for (int i = 0; i < _clienteDetalle.Documento.Length; i++)
            {
                string tipo = _clienteDetalle.Documento[i].TipoDocumento.descripcion;
                Button btnDocumento = _botonesTipoArchivo[tipo];
                btnDocumento.Content = _clienteDetalle.Documento[i].nombre;
            }
           
        }

        private void ClicDescargarDocumento(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            if (btn.Content != null)
            {
                string nombreDocumento = btn.Content.ToString();
                string tipoDocumento = _botonesTipoArchivo.FirstOrDefault(boton => boton.Value == btn).Key;
                DescargarDocumento(nombreDocumento, tipoDocumento);
            }
        }

        private void DescargarDocumento(string nombreDocumento, string tipo)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ruta = dialog.SelectedPath + "/" + nombreDocumento;
                Documento archivo = _clienteDetalle.Documento.Where(documento => documento.TipoDocumento.descripcion.Equals(tipo)).FirstOrDefault();

                File.WriteAllBytes(ruta, archivo.archivo);
                MostrarVentanaExitoDescarga(nombreDocumento);

            }
        }

        private void MostrarVentanaExitoDescarga(string documento)
        {
            VentanaMensaje mensaje = new VentanaMensaje("Se ha descargado el documento '" + documento + "' exitosamente", Mensaje.EXITO);
            mensaje.Mostrar();
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            ListaClientesPagina lista = new ListaClientesPagina();
            MainWindow main = (MainWindow)Window.GetWindow(this);
            main.CambiarPagina(lista);
        }

        private void DeshabilitarBotones()
        {
            btnDescargarIdentificacion.IsEnabled = false;
            btnDescargarComprobanteDomicilio.IsEnabled = false;
            btnDescargarComprobanteIngreso.IsEnabled = false;
            btnDescargarComprobanteTrabajo.IsEnabled = false;
            btnDescargarReferenciaCliente1.IsEnabled = false;
            btnDescargarReferenciaCliente2.IsEnabled = false;
        }
    }
}
