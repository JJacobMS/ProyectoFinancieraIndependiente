using System;
using System.Collections.Generic;
using System.Linq;
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
            bool sonCamposValidos = ValidarCampos();
            if (sonCamposValidos)
            {
                Console.WriteLine("Validos");
                PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                //int respuesta = proxy.GuardarPoliticaOtorgamiento(1);
                //Console.WriteLine("Respuesta "+respuesta);
            }
            else
            {
                MostrarVentanaCamposNoValidos();

            }
        }

        private bool ValidarCampos()
        {
            bool esValido = true;
            string nombrePolitica = txtBoxNombre.Text;
            string descripcionPolitica = txtBoxDescripcion.Text;
            DateTime? fechaVigencia = dtPickerFechaVigencia.SelectedDate;
            if (String.IsNullOrWhiteSpace(nombrePolitica))
            { 
                esValido = false;
                txtBoxNombre.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
            }
            else
            {
                txtBoxNombre.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            if (String.IsNullOrWhiteSpace(descripcionPolitica))
            {
                esValido = false;
                txtBoxDescripcion.Style = (Style) FindResource("estiloTxtBoxFormularioRojo");
            }
            else
            {
                txtBoxDescripcion.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            if (!fechaVigencia.HasValue)
            {
                esValido = false;
                dtPickerFechaVigencia.Style = (Style)FindResource("estiloDatePickerFormularioPoliticaOtorgamientoRojo");
            }
            else
            {
                Console.WriteLine(fechaVigencia);
                dtPickerFechaVigencia.Style = (Style)FindResource("estiloDatePickerFormularioPoliticaOtorgamiento");

            }
            return esValido;
        }

        private void MostrarVentanaCamposNoValidos()
        {
            Console.WriteLine("No son campos validos");
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
