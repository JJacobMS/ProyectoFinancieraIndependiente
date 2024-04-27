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
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para MenuPrincipalAsesorCreditoPagina.xaml
    /// </summary>
    public partial class MenuPrincipalAsesorCreditoPagina : Page
    {
        public MenuPrincipalAsesorCreditoPagina()
        {
            InitializeComponent();
            txtBlockNombre.Text = "Bienvenido " + UsuarioSingleton.ObtenerUsuario().nombres;
        }

        private void ClicConsultarClientes(object sender, RoutedEventArgs e)
        {
            BusquedaRFCCliente paginaBusqueda = new BusquedaRFCCliente();
            MainWindow ventana = (MainWindow)Window.GetWindow(this);
            ventana.CambiarPagina(paginaBusqueda);
        }

        private void ClicRegistrarCliente(object sender, RoutedEventArgs e)
        {
            FormularioClientePagina paginaFormularioCliente = new FormularioClientePagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(paginaFormularioCliente);
        }

        private void ClicConsultarCreditos(object sender, RoutedEventArgs e)
        {
            ListaCreditosPagina lista = new ListaCreditosPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(lista);
        }

        private void ClicConsultarCalculoEficiencias(object sender, RoutedEventArgs e)
        {

        }

        private void ClicSalirInicioSesion(object sender, RoutedEventArgs e)
        {
            UsuarioSingleton.SettearUsuario(null);
            InicioSesionPagina paginaInicio = new InicioSesionPagina();
            MainWindow ventana = (MainWindow)Window.GetWindow(this);
            ventana.CambiarPagina(paginaInicio);
        }

        private void ClicRegistrarCobro(object sender, RoutedEventArgs e)
        {

        }
    }
}
