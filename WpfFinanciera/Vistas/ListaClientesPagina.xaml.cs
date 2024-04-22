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
    /// Lógica de interacción para ListaClientesPagina.xaml
    /// </summary>
    public partial class ListaClientesPagina : Page
    {
        private List<Cliente> _clientes;

        public ListaClientesPagina()
        {
            InitializeComponent();
            CargarPaginaLista();
        }

        private void CargarPaginaLista()
        {
            RecuperarClientes();
        }

        private void RecuperarClientes()
        {
            _clientes = new List<Cliente>();
            Codigo codigo;
            try
            {
                Cliente[] clientesRecuperados;
                ClienteClient proxy = new ClienteClient();
                (clientesRecuperados, codigo) = proxy.RecuperarClientesRegistrados();
                if (clientesRecuperados != null)
                {
                    _clientes = clientesRecuperados.ToList();
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
                    if (_clientes.Count == 0)
                    {
                        MostrarNoHayClientesRegistrados();
                    }
                    else
                    {
                        CargarListaClientes(_clientes);
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
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje ventana = new VentanaMensaje("Error. No se pudo conectar con la base de datos.Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventana.Mostrar();
        }


        private void MostrarNoHayClientesRegistrados()
        {
            lstBoxClientes.Style = (Style)FindResource("estiloLstBoxClientesSinRegistros");
            btnBuscar.IsEnabled = false;
        }

        private void CargarListaClientes(List<Cliente> clientes)
        {
            lstBoxClientes.ItemsSource = clientes;
        }

        private void ClicBuscar(object sender, RoutedEventArgs e)
        {
            lstBoxClientes.Style = null;
            List<Cliente> clientesFiltrados = FiltrarClientes();

            if (clientesFiltrados.Count == 0)
            {
                MostrarClientesNoEncontrados();
            }
            else
            {
                CargarListaClientes(clientesFiltrados);
            }
        }

        private List<Cliente> FiltrarClientes()
        {
            List<Cliente> clientesFiltrados = _clientes;
            string busqueda = txtBoxBarraBuscar.Text.Trim();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                clientesFiltrados = _clientes.Where(
                   cliente => cliente.rfc.Contains(busqueda) || (cliente.nombres + " " + cliente.apellidos).Contains(busqueda)).ToList();
            }

            return clientesFiltrados;
        }

        private void MostrarClientesNoEncontrados()
        {
            lstBoxClientes.Style = (Style)FindResource("estiloLstBoxClientesSinBusqueda");
        }

        private void ClicVerMas(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Cliente cliente = btn.CommandParameter as Cliente;
            DetallesClientePagina clienteDetalles = new DetallesClientePagina(cliente);
            MainWindow ventanaPrincipal = (MainWindow) Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(clienteDetalles);
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MenuPrincipalAnalistaCreditoPagina menu = new MenuPrincipalAnalistaCreditoPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(menu);
        }
    }
}
