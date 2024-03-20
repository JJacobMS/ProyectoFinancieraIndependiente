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

            try {
                bool sonCamposValidos = ValidarCampos();
                if (sonCamposValidos)
                {
                    Politica politicaNueva = new Politica
                    {
                        nombre = txtBoxNombre.Text,
                        descripcion = txtBoxDescripcion.Text,
                        vigencia = (DateTime)dtPickerFechaVigencia.SelectedDate,
                        estaActiva = true
                    };
                    PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                    int respuesta = proxy.GuardarPoliticaOtorgamiento(politicaNueva);
                    Console.WriteLine(respuesta);
                    //MostrarVentanaErrorBaseDatos
                    //MostrarVentanaExito()

                }
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.ToString());
                MostrarVentanaErrorServidor();
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.ToString());
                MostrarVentanaErrorServidor();
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


        private void ClicCancelar(object sender, RoutedEventArgs e)
        {

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
