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
    /// Lógica de interacción para InicioSesionPagina.xaml
    /// </summary>
    public partial class InicioSesionPagina : Page
    {
        public InicioSesionPagina()
        {
            InitializeComponent();
        }

        private void ClicVerificarUsuario(object sender, RoutedEventArgs e)
        {
            ResetearCampos();
            bool sonCamposValidos = true;
            string correo = txtBlockCorreoElectronico.Text;
            string contrasena = pssBoxContrasena.Password;

            if (string.IsNullOrWhiteSpace(correo) || correo.Length > 100)
            {
                sonCamposValidos = false;
                txtBlockCorreoElectronico.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
            }
            if (string.IsNullOrWhiteSpace(contrasena))
            {
                sonCamposValidos = false;
                pssBoxContrasena.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");

            }

            if (sonCamposValidos)
            {
                VerificarUsuario(correo, contrasena);
            }
            
        }

        private void VerificarUsuario(string correo, string contrasena)
        {
            Codigo codigo;
            Usuario usuario = null;
            try
            {
                UsuarioClient proxy = new UsuarioClient();
                (usuario, codigo) = proxy.ValidarUsuario(correo, contrasena);
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

            VentanaMensaje ventanaMensaje;

            switch (codigo)
            {
                case Codigo.EXITO:
                    EntrarSistema(usuario);
                    break;
                case Codigo.ERROR_SERVIDOR:
                    ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
                    ventanaMensaje.Mostrar();
                    break;
                case Codigo.ERROR_BD:
                    ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
                    ventanaMensaje.Mostrar();
                    break;
            }

        }

        private void EntrarSistema(Usuario usuario)
        {
            if (usuario == null)
            {
                txtBlockErrorCorreo.Visibility = Visibility.Visible;
            }
            else if (usuario.idUsuario == 0)
            {
                txtBlockErrorContrasena.Visibility = Visibility.Visible;
            }
            else
            {
                DirigirPaginaMenuPrincipal(usuario);
            }
        }

        private void DirigirPaginaMenuPrincipal(Usuario usuario)
        {
            UsuarioSingleton.SettearUsuario(usuario);

            MainWindow ventanaPrincipal = (MainWindow) Window.GetWindow(this);
            switch (usuario.TipoUsuario.nombre)
            {
                case "Asesor de Crédito":
                    MenuPrincipalAsesorCreditoPagina menuAsesor = new MenuPrincipalAsesorCreditoPagina();
                    ventanaPrincipal.CambiarPagina(menuAsesor);
                    break;
                case "Analista de Crédito":
                    MenuPrincipalAnalistaCreditoPagina menuAnalista = new MenuPrincipalAnalistaCreditoPagina();
                    ventanaPrincipal.CambiarPagina(menuAnalista);
                    break;
                case "Administrador":
                    MenuPrincipalAdministradorPagina menuAdmin = new MenuPrincipalAdministradorPagina();
                    ventanaPrincipal.CambiarPagina(menuAdmin);
                    break;
            }
        }

        private void ResetearCampos()
        {
            pssBoxContrasena.Background = Brushes.Transparent;
            txtBlockCorreoElectronico.Background = Brushes.Transparent;
            txtBlockErrorCorreo.Visibility = Visibility.Hidden;
            txtBlockErrorContrasena.Visibility = Visibility.Hidden;
        }

        private void ClicSalir(object sender, RoutedEventArgs e)
        {
            Window ventana = Window.GetWindow(this);
            ventana.Close();
        }
    }
}
