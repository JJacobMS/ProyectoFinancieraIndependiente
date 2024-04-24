using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioUsuarioPagina.xaml
    /// </summary>
    public partial class FormularioUsuarioPagina : Page
    {
        string _nombres;
        string _apellidos;
        string _correo;
        string _contraseña;
        string _confirmarContraseña;
        bool _contraseñasIguales;
        int _idTipoUsuarioSeleccionado;

        public FormularioUsuarioPagina()
        {
            InitializeComponent();
        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            ObtenerTiposUsuario();
        }

        private void ObtenerTiposUsuario()
        {
            Codigo codigoRespuesta = new Codigo();
            TipoUsuario[] listaTiposUsuario = new TipoUsuario[0];

            try
            {
                TipoUsuarioClient tipoUsuarioClient = new TipoUsuarioClient();
                var respuesta = tipoUsuarioClient.RecuperarTiposUsuario();
                var (tiposUsuario, codigo) = respuesta;

                codigoRespuesta = codigo;
                listaTiposUsuario = tiposUsuario;
            }
            catch (CommunicationException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }
            catch (TimeoutException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }

            switch (codigoRespuesta)
            {
                case Codigo.EXITO:
                    if(listaTiposUsuario != null)
                    {
                        int lastIndex = listaTiposUsuario.Length - 1;
                        TipoUsuario[] nuevosTiposUsuario = new TipoUsuario[lastIndex];
                        Array.Copy(listaTiposUsuario, nuevosTiposUsuario, lastIndex);
                        listaTiposUsuario = nuevosTiposUsuario;
                        lstBoxTiposUsuario.ItemsSource = listaTiposUsuario;
                    }
                    else
                    {
                        bdrTiposUsuario.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                        MostrarMensajeTiposUsuarios();
                    }
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    RegresarAMenuPrincipal();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    RegresarAMenuPrincipal();
                    break;
            }
        }

        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje errorServidor = new VentanaMensaje(
               "Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            errorServidor.Mostrar();
        }

        private void MostrarVentanaErrorBD()
        {
            VentanaMensaje errorBD = new VentanaMensaje(
                "Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            errorBD.Mostrar();
        }

        private void MostrarMensajeTiposUsuarios()
        {
            txtBlockMensajeNoTiposUsuario.Visibility = Visibility.Visible;
            lstBoxTiposUsuario.Visibility = Visibility.Collapsed;
        }

        private void ClicRegresar(object sender, MouseButtonEventArgs e)
        {
            RegresarAMenuPrincipal();
        }

        private void RegresarAMenuPrincipal()
        {
            MenuPrincipalAdministradorPagina menuPrincipal = new MenuPrincipalAdministradorPagina();

            NavigationService.Navigate(menuPrincipal);
        }

        private void VerificarSeleccionTipoUsuario(object sender, SelectionChangedEventArgs e)
        {
            ActualizarValoresCampos();

            if(_nombres != "" && _apellidos != "" && _correo != "" && _contraseña != "" && _confirmarContraseña != "" && _contraseñasIguales)
            {
                HabilitarRegistrar();
            }
        }

        private void ActualizarValoresCampos()
        {
            _nombres = txtBoxNombre.Text.Trim();
            _apellidos = txtBoxApellidos.Text.Trim();
            _correo = txtBoxCorreo.Text.Trim();
            _contraseña = string.IsNullOrEmpty(pswBoxContraseña.Password.Trim()) ? txtBoxMostrarContraseña.Text : pswBoxContraseña.Password.Trim();
            _confirmarContraseña = string.IsNullOrEmpty(pswBoxContraseñaRepetida.Password.Trim()) ? txtBoxMostrarContraseñaRepetida.Text : pswBoxContraseñaRepetida.Password.Trim();
            _contraseñasIguales = _contraseña == _confirmarContraseña;

            if(lstBoxTiposUsuario.SelectedIndex != -1)
            {
                _idTipoUsuarioSeleccionado = (lstBoxTiposUsuario.SelectedItem as TipoUsuario).idTipoUsuario;
            }
        }

        private void HabilitarRegistrar()
        {
            btnRegistrar.IsEnabled = true;
        }

        private void ClicRegistrar(object sender, RoutedEventArgs e)
        {
            string motivos;

            motivos = ValidarCorreo();
            motivos += ValidarContraseñaSegura();

            if(motivos != "")
            {
                MostrarVentanaError(motivos);
            }
            else
            {
                MostrarVentanaConfirmacion();
            }
        }

        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventanaConfirmacion = new VentanaMensaje("¿Desea registrar el usuario ingresado?", Mensaje.CONFIRMACION);
            if(ventanaConfirmacion.MostrarConfirmacion())
            {
                RegistrarUsuario();
            }
        }

        private void RegistrarUsuario()
        {
            Codigo codigoRespuesta = new Codigo();
            bool correoUnico = false;

            try
            {
                UsuarioClient usuarioClient = new UsuarioClient();
                var respuesta = usuarioClient.EsCorreoUnico(_correo);
                var (codigo, esCorreoUnico) = respuesta;

                codigoRespuesta = codigo;
                correoUnico = esCorreoUnico;
            }
            catch (CommunicationException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }
            catch (TimeoutException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }

            switch(codigoRespuesta)
            {
                case Codigo.EXITO:
                    if(correoUnico)
                    {
                        RegistrarUsuarioBD();
                    }
                    else
                    {
                        MostrarVentanaError("Correo ya registrado.");
                    }
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
            }
        }

        private void RegistrarUsuarioBD()
        {
            Console.WriteLine(_idTipoUsuarioSeleccionado);

            Usuario usuarioNuevo = new Usuario
            {
                nombres = _nombres,
                apellidos = _apellidos,
                correoElectronico = _correo,
                contrasenha = _contraseña,
                TipoUsuario_idTipoUsuario = _idTipoUsuarioSeleccionado
            };

            Codigo codigoRespuesta = new Codigo();

            try
            {
                UsuarioClient usuarioClient = new UsuarioClient();
                var codigo = usuarioClient.GuardarUsuario(usuarioNuevo);

                codigoRespuesta = codigo;
            }
            catch (CommunicationException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }
            catch (TimeoutException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }

            switch (codigoRespuesta)
            {
                case Codigo.EXITO:
                    MostrarVentanaExito();
                    RegresarAMenuPrincipal();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
            }
        }

        private void MostrarVentanaExito()
        {
            VentanaMensaje exito = new VentanaMensaje("Se ha registrado el usuario exitosamente", Mensaje.EXITO);
            exito.Mostrar();
        }

        private void MostrarVentanaError(string motivos)
        {
            VentanaMensaje error = new VentanaMensaje("Los campos ingresados no son válidos", motivos);
            error.Mostrar();
        }

        private string ValidarCorreo()
        {
            string motivos;

            try
            {
                _ = new MailAddress(_correo);
                motivos = "";
            }
            catch (FormatException)
            {
                motivos = "Correo con formato no válido.";
            }

            return motivos;
        }

        private string ValidarContraseñaSegura()
        {
            string motivos = "";
            string expresion = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d@$!%*?#\-&_]{8,}$";

            TimeSpan timeout = TimeSpan.FromMilliseconds(500);

            try
            {
                if (!Regex.IsMatch(_contraseña, expresion, RegexOptions.None, timeout))
                {
                    motivos = "Contraseña débil, ingresa una contraseña que contenga al menos una mayúscula, una minúscula y un número e intenta de nuevo.";
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                motivos = "Error al validar la contraseña.";
            }

            return motivos;
        }


        private void VerificarCambioTexto(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            ActualizarValoresCampos();

            if (textBox != null)
            {
                string nombreTextBox = textBox.Name;

                if ((nombreTextBox == "txtBoxNombre" || nombreTextBox == "txtBoxApellidos") &&
                    !Regex.IsMatch(textBox.Text, "^[a-zA-Z0-9]+$"))
                {
                    textBox.Text = string.Join("", textBox.Text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));
                }

                if (textBox.Text.Trim() == "")
                {
                    MarcarCampoRojo(textBox);
                    DeshabilitarRegistrar();
                }
                else
                {
                    textBox.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#A9D4D6");
                }
            }

            if(_nombres != "" && _apellidos != "" && _correo != "" && _contraseña != "" && _confirmarContraseña != "" && _contraseñasIguales && lstBoxTiposUsuario.SelectedIndex != -1)
            {
                HabilitarRegistrar();
            }
            else
            {
                DeshabilitarRegistrar();
            }
        }

        private void DeshabilitarRegistrar()
        {
            btnRegistrar.IsEnabled = false;
        }

        private void MarcarCampoRojo(TextBox textBox)
        {
            textBox.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FCC6C1");
        }

        private void ClicVerContraseña(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            image.Source = new BitmapImage(new Uri("/Recursos/opened-eye.png", UriKind.Relative));

            MostrarTextoContraseña(image);

            image.MouseDown -= ClicVerContraseña;
            image.MouseDown += ClicOcultarContraseña;

            VerificarSeleccionTipoUsuario(null, null);
        }

        private void MostrarTextoContraseña(Image image)
        {
            if (image.Name == "imgVerContraseña")
            {
                txtBoxMostrarContraseña.Visibility = Visibility.Visible;
                txtBoxMostrarContraseña.Text = pswBoxContraseña.Password;
                pswBoxContraseña.Password = "";
                pswBoxContraseña.IsEnabled = false;
                pswBoxContraseña.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#A9D4D6");
            }
            else
            {
                txtBoxMostrarContraseñaRepetida.Visibility = Visibility.Visible;
                txtBoxMostrarContraseñaRepetida.Text = pswBoxContraseñaRepetida.Password;
                pswBoxContraseñaRepetida.Password = "";
                pswBoxContraseñaRepetida.IsEnabled = false;
                pswBoxContraseñaRepetida.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#A9D4D6");
            }
        }

        private void ClicOcultarContraseña(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            image.Source = new BitmapImage(new Uri("/Recursos/slashed-eye.png", UriKind.Relative));

            OcultarTextoContraseña(image);

            image.MouseDown -= ClicOcultarContraseña;
            image.MouseDown += ClicVerContraseña;
        }

        private void OcultarTextoContraseña(Image image)
        {
            if (image.Name == "imgVerContraseña")
            {
                txtBoxMostrarContraseña.Visibility = Visibility.Hidden;
                pswBoxContraseña.Password = txtBoxMostrarContraseña.Text;
                pswBoxContraseña.IsEnabled = true;

            }
            else
            {
                txtBoxMostrarContraseñaRepetida.Visibility = Visibility.Hidden;
                pswBoxContraseñaRepetida.Password = txtBoxMostrarContraseñaRepetida.Text;
                pswBoxContraseñaRepetida.IsEnabled = true;
            }
        }

        private void VerificarCambioContraseña(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            ActualizarValoresCampos();

            if (passwordBox.Password.Trim() == "")
            {
                passwordBox.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FCC6C1");
                DeshabilitarRegistrar();
            }
            else
            {
                passwordBox.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#A9D4D6");
            }

            if (_nombres != "" && _apellidos != "" && _correo != "" && _contraseña != "" && _confirmarContraseña != "" && _contraseñasIguales && lstBoxTiposUsuario.SelectedIndex != -1)
            {
                HabilitarRegistrar();
            }
            else
            {
                DeshabilitarRegistrar();
            }
        }
    }
}
