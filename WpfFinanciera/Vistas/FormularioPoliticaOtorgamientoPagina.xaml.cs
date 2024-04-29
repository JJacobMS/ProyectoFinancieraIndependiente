using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Lógica de interacción para FormularioPoliticaOtorgamientoPagina.xaml
    /// </summary>
    public partial class FormularioPoliticaOtorgamientoPagina : Page
    {
        private Politica _politica;
        private bool _esModificar;
        private bool _esIngresar;
        private bool _estaActiva;

        public FormularioPoliticaOtorgamientoPagina()
        {
            InitializeComponent();
            txtBlockTitulo.Text = "Registrar Politica de Otorgamiento";
            ChkBoxPoliticaActiva.Visibility = Visibility.Hidden;
            txtBlockPoliticaActiva.Visibility = Visibility.Hidden;
            _esIngresar = true;
        }

        public FormularioPoliticaOtorgamientoPagina(Politica politica)
        {
            InitializeComponent();
            _politica = politica;
            txtBlockTitulo.Text = "Modificar Politica de Otorgamiento";
            ChkBoxPoliticaActiva.Visibility = Visibility.Visible;
            txtBlockPoliticaActiva.Visibility = Visibility.Visible;
            _esModificar = true;
            CargarPolitica(_politica);
        }

        private void CargarPolitica(Politica politica) 
        {
            txtBoxNombre.Text = politica.nombre;
            txtBoxDescripcion.Text = politica.descripcion;
            dtPickerFechaVigencia.SelectedDate = politica.vigencia.Date;
            ChkBoxPoliticaActiva.IsChecked = politica.estaActiva;
            _estaActiva = politica.estaActiva;
        }

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCampos();
            if (sonCamposValidos)
            {
                Codigo codigo = new Codigo();
                try 
                {
                    DateTime? fechaVigencia = dtPickerFechaVigencia.SelectedDate;
                    DateTime fechaActual = DateTime.Now.Date;
                    DateTime fechaSeleccionada = fechaVigencia.Value.Date;
                    bool estaActiva = fechaSeleccionada > fechaActual ? true : false;
                    if (_esModificar) 
                    {
                        estaActiva = ValidarPoliticaActiva(estaActiva, fechaActual, fechaSeleccionada);
                    }
                    Politica politica = new Politica
                    {
                         nombre = txtBoxNombre.Text,
                         descripcion = txtBoxDescripcion.Text,
                         vigencia = (DateTime)dtPickerFechaVigencia.SelectedDate,
                         estaActiva = estaActiva
                    };
                    PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                    if (_esIngresar) 
                    {
                        codigo = proxy.GuardarPoliticaOtorgamiento(politica);
                    }
                    else if (_esModificar) 
                    {
                        politica.idPolitica = _politica.idPolitica;
                        codigo = proxy.ActualizarPoliticaOtorgamiento(politica);
                    }
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
                        LimpiarCampos();
                        MostrarVentanaExito();
                        break;
                    case Codigo.ERROR_SERVIDOR:
                        MostrarVentanaErrorServidor();
                        break;
                    case Codigo.ERROR_BD:
                        MostrarVentanaErrorBaseDatos();
                        break;
                }
            }
        }

        private bool ValidarPoliticaActiva(bool estaActiva, DateTime fechaActual,DateTime fechaSeleccionada) 
        {
            if (!_estaActiva)
            {
                estaActiva = false;
            }
            if (fechaSeleccionada < fechaActual)
            {
                estaActiva = false;
            }
            return estaActiva;
        }

        private void LimpiarCampos() 
        {
            txtBoxNombre.Text = "";
            txtBoxDescripcion.Text = "";
            dtPickerFechaVigencia.SelectedDate = null;
            dtPickerFechaVigencia.Style = (Style)FindResource("estiloDatePickerFormularioPoliticaOtorgamiento");
            if (dtPickerFechaVigencia.Template != null)
            {
                var textBox = dtPickerFechaVigencia.Template.FindName("PART_TextBox", dtPickerFechaVigencia) as DatePickerTextBox;
                if (textBox != null)
                {
                    var defaultText = "Seleccione una fecha";
                    if (textBox.Text != defaultText)
                    {
                        textBox.Text = defaultText;
                    }
                }
            }
        }
        private void MostrarVentanaExito()
        {
            VentanaMensaje mensajeExito;
            if (_esIngresar) 
            {
                mensajeExito = new VentanaMensaje("Se ha registrado la política de otorgamiento exitosamente", Mensaje.EXITO);
            }
            else 
            {
                mensajeExito = new VentanaMensaje("Se ha actualizado la política de otorgamiento exitosamente", Mensaje.EXITO);
            }
            mensajeExito.Mostrar();
            Cerrar();
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

        private bool ValidarCampos()
        {
            bool sonCamposValidos = true;
            string nombrePolitica = txtBoxNombre.Text;
            string descripcionPolitica = txtBoxDescripcion.Text;
            DateTime? fechaVigencia = dtPickerFechaVigencia.SelectedDate;
            string razones = "";

            if (String.IsNullOrWhiteSpace(nombrePolitica))
            { 
                sonCamposValidos = false;
                txtBoxNombre.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                razones += "Nombre politica";
            }
            else
            {
                txtBoxNombre.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            if (String.IsNullOrWhiteSpace(descripcionPolitica))
            {
                sonCamposValidos = false;
                txtBoxDescripcion.Style = (Style) FindResource("estiloTxtBoxFormularioRojo");
                razones = (razones.Length > 0) ? razones + ", Descripción" : "Descripción";
            }
            else
            {
                txtBoxDescripcion.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            if (!fechaVigencia.HasValue)
            {
                sonCamposValidos = false;
                dtPickerFechaVigencia.Style = (Style)FindResource("estiloDatePickerFormularioPoliticaOtorgamientoRojo");
                razones = (razones.Length > 0) ? razones + ", Fecha vigencia" : "Fecha vigencia";
            }
            else
            {
                dtPickerFechaVigencia.Style = (Style)FindResource("estiloDatePickerFormularioPoliticaOtorgamiento");
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

        private void TextChangedtxtBoxDescripcion(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoDescripcion = 70;
            if (txtBoxDescripcion.Text.Length > tamañoMaximoDescripcion)
            {
                txtBoxDescripcion.Text = txtBoxDescripcion.Text.Substring(0, tamañoMaximoDescripcion);
                txtBoxDescripcion.SelectionStart = txtBoxDescripcion.Text.Length;
            }
        }

        private void TextChangedtxtBoxNombre(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoNombre = 30;
            if (txtBoxNombre.Text.Length > tamañoMaximoNombre)
            {
                txtBoxNombre.Text = txtBoxNombre.Text.Substring(0, tamañoMaximoNombre);
                txtBoxNombre.SelectionStart = txtBoxNombre.Text.Length;
            }
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            Cerrar();
        }

        private void Cerrar()
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            MenuPrincipalAdministradorPagina menu = new MenuPrincipalAdministradorPagina();
            ventanaPrincipal.CambiarPagina(menu);
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            _estaActiva = true;
        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
            _estaActiva = false;
        }
    }
}
