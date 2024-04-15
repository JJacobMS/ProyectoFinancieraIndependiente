using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para ListaClientesPagina.xaml
    /// </summary>
    public partial class ListaClientesPagina : Page
    {
        private List<Cliente> clientes;
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
            List<Cliente> clientes = new List<Cliente>
            {
                new Cliente{nombres = "Sulem A", apellidos = "Martinez Aguilar aaaaaa", rfc = "Q83891bvnab3"},
                new Cliente{nombres = "Jamzin A", apellidos = "Martinez Aguilar aaaaaa", rfc = "R7948fidnsjnj"},
                new Cliente{nombres = "Jacobo A", apellidos = "Monyiel ifivsofv aaaaaa", rfc = "Sbvjoda82990"}
            };

            this.clientes = clientes;

            if (clientes.Count == 0)
            {
                MostrarNoHayClientesRegistrados();
            }
            else
            {
                CargarListaClientes(clientes);
            }
                        
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
            List<Cliente> clientesFiltrados = clientes;
            string busqueda = txtBoxBarraBuscar.Text.Trim();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                clientesFiltrados = clientes.Where(
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
            DetallesClientePagina clienteDetalles = new DetallesClientePagina();
            MainWindow ventanaPrincipal = (MainWindow) Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(clienteDetalles);
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {

        }
    }
}
