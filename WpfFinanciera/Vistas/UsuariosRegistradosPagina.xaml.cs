using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para UsuariosRegistradosPagina.xaml
    /// </summary>
    public partial class UsuariosRegistradosPagina : Page
    {
        Usuario[] _listaUsuariosRegistrados;

        public UsuariosRegistradosPagina()
        {
            InitializeComponent();
        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            ObtenerUsuariosRegistrados();
        }

        private void ObtenerUsuariosRegistrados()
        {
            Codigo codigoRespuesta = new Codigo();

            try
            {
                UsuarioClient proxy = new UsuarioClient();
                var respuesta = proxy.ObtenerUsuariosRegistrados();
                var (codigo, usuarios) = respuesta;

                codigoRespuesta = codigo;
                _listaUsuariosRegistrados = usuarios;
            }
            catch (CommunicationException ex)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }

            switch (codigoRespuesta)
            {
                case Codigo.EXITO:
                    lstBoxUsuarios.ItemsSource = _listaUsuariosRegistrados;
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
            }
        }

        private void MostrarVentanaErrorBD()
        {
            VentanaMensaje errorServidor = new VentanaMensaje(
                "Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            errorServidor.Mostrar();
        }

        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje errorBaseDatos = new VentanaMensaje(
                "Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            errorBaseDatos.Mostrar();
        }

        private void ClicBuscar(object sender, RoutedEventArgs e)
        {
            string busqueda = txtBoxBusquedaUsuario.Text.Trim().ToLower();

            List<Usuario> listaFiltrada = FiltrarLista(busqueda);

            lstBoxUsuarios.ItemsSource = listaFiltrada;

            Console.WriteLine(listaFiltrada.Count);

            if(listaFiltrada.Count == 0)
            {
                MostrarMensajeUsuariosNoEncontrados();
            }
            else
            {
                brdMensajeNoUsuarios.Visibility = Visibility.Collapsed;
                lstBoxUsuarios.Visibility = Visibility.Visible;
            }
        }

        private void MostrarMensajeUsuariosNoEncontrados()
        {
            brdMensajeNoUsuarios.Visibility = Visibility.Visible;
            lstBoxUsuarios.Visibility = Visibility.Collapsed;
        }

        private List<Usuario> FiltrarLista(string busqueda)
        {
            List<Usuario> listaFiltrada = new List<Usuario>();

            if(_listaUsuariosRegistrados != null && !string.IsNullOrEmpty(busqueda))
            {
                foreach (var usuario in _listaUsuariosRegistrados)
                {
                    if (usuario.nombres.ToLower().Contains(busqueda) || usuario.correoElectronico.ToLower().Contains(busqueda))
                    {
                        listaFiltrada.Add(usuario);
                    }
                }
            }
            else
            {
                listaFiltrada = new List<Usuario>(_listaUsuariosRegistrados); 
            }

            return listaFiltrada;
        }

        private void ClicRegistro(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ClicRegresar(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
