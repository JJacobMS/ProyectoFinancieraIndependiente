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
    /// Lógica de interacción para MenuPrincipalAnalistaCreditoPagina.xaml
    /// </summary>
    public partial class MenuPrincipalAnalistaCreditoPagina : Page
    {
        public MenuPrincipalAnalistaCreditoPagina()
        {
            InitializeComponent();
            txtBlockNombre.Text = "Bienvenido " + UsuarioSingleton.ObtenerUsuario().nombres;
        }

        private void ClicConsultarSolicitudes(object sender, RoutedEventArgs e)
        {
            SolicitudesCreditoPagina paginaSolicitudCredito = new SolicitudesCreditoPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(paginaSolicitudCredito);
        }

        private void ClicConsultarClientes(object sender, RoutedEventArgs e)
        {
            ListaClientesPagina clientes = new ListaClientesPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(clientes);
        }

        private void ClicConsultarCalculo(object sender, RoutedEventArgs e)
        {

        }

        private void ClicSalirInicioSesion(object sender, RoutedEventArgs e)
        {
            UsuarioSingleton.SettearUsuario(null);
            InicioSesionPagina paginaInicio = new InicioSesionPagina();
            MainWindow ventana = (MainWindow)Window.GetWindow(this);
            ventana.CambiarPagina(paginaInicio);
        }
    }
}
