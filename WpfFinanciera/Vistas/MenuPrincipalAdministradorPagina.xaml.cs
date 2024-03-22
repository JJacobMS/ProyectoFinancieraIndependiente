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
    /// Lógica de interacción para MenuPrincipalAdministradorPagina.xaml
    /// </summary>
    public partial class MenuPrincipalAdministradorPagina : Page
    {
        public MenuPrincipalAdministradorPagina()
        {
            InitializeComponent();
            txtBlockNombre.Text = "Bienvenido " + UsuarioSingleton.ObtenerUsuario().nombres;
        }

        private void ClicConsultarUsuarios(object sender, RoutedEventArgs e)
        {

        }

        private void ClicRegistrarNuevoUsuario(object sender, RoutedEventArgs e)
        {

        }

        private void ClicConsultarCondicionesCredito(object sender, RoutedEventArgs e)
        {

        }

        private void ClicRegistrarCondicionCredito(object sender, RoutedEventArgs e)
        {
            FormularioCondicionCreditoPagina paginaFormularioCondicion = new FormularioCondicionCreditoPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(paginaFormularioCondicion);
        }

        private void ClicConsultarChecklist(object sender, RoutedEventArgs e)
        {

        }

        private void ClicRegistrarNuevaChecklist(object sender, RoutedEventArgs e)
        {
            FormularioChecklistPagina paginaFormularioChecklist = new FormularioChecklistPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(paginaFormularioChecklist);
        }

        private void ClicCalculoEficienciasMensaules(object sender, RoutedEventArgs e)
        {

        }

        private void ClicConsultarPoliticas(object sender, RoutedEventArgs e)
        {

        }

        private void ClicRegistrarNuevaPolitica(object sender, RoutedEventArgs e)
        {
            FormularioPoliticaOtorgamientoPagina paginaFormularioPolitica = new FormularioPoliticaOtorgamientoPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(paginaFormularioPolitica);
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
