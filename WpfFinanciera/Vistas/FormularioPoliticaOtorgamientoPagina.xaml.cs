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
        public FormularioPoliticaOtorgamientoPagina()
        {
            InitializeComponent();
        }

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {
            Codigo codigo = new Codigo();
            try 
            {
                bool sonCamposValidos = ValidarCampos();
                if (sonCamposValidos)
                {
                    DateTime? fechaVigencia = dtPickerFechaVigencia.SelectedDate;
                    DateTime fechaActual = DateTime.Now.Date;
                    DateTime fechaSeleccionada = fechaVigencia.Value.Date;
                    bool estaActiva = fechaSeleccionada > fechaActual ? true : false;
                    Politica politicaNueva = new Politica
                    {
                        nombre = txtBoxNombre.Text,
                        descripcion = txtBoxDescripcion.Text,
                        vigencia = (DateTime)dtPickerFechaVigencia.SelectedDate,
                        estaActiva = estaActiva
                    };
                    PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                    codigo = proxy.GuardarPoliticaOtorgamiento(politicaNueva);
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
            VentanaMensaje mensajeError = new VentanaMensaje("Se ha registrado la política de otorgamiento exitosamente", Mensaje.EXITO);
            mensajeError.Mostrar();
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
                Console.WriteLine(fechaVigencia);
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
    }
}
