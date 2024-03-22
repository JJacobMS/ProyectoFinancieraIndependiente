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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioChecklistPagina.xaml
    /// </summary>
    public partial class FormularioChecklistPagina : Page
    {
        private List<string> listNombrePoliticas_ = new List<string>() { };
        List<int> listIdPoliticas_ = new List<int>();
        public FormularioChecklistPagina()
        {
            InitializeComponent();
        }

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {
                bool sonCamposValidos = ValidarCampos();
                if (sonCamposValidos)
                {
                    MostrarVentanaConfirmacion();
                }
        }

        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea registrar el Checklist?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                GuardarChecklist();
            }
        }

        private void GuardarChecklist()
        {
            Codigo codigo = new Codigo();
            int[] arrayIdPoliticas = new int[listIdPoliticas_.Count];
            for (int i = 0; i < listIdPoliticas_.Count; i++)
            {
                arrayIdPoliticas[i] = listIdPoliticas_[i];
            }
            try
            {
                Checklist checklistNueva = new Checklist
                {
                    nombre = txtBoxNombreChecklist.Text,
                    descripcion = txtBoxDescripcionChecklist.Text
                };
                ChecklistClient proxy = new ChecklistClient();
                codigo = proxy.GuardarChecklist(checklistNueva, arrayIdPoliticas);
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

        private void MostrarVentanaExito()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Se ha registrado el checklist exitosamente", Mensaje.EXITO);
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
            string nombreChecklist = txtBoxNombreChecklist.Text;
            string descripcionChecklist = txtBoxDescripcionChecklist.Text;
            string razones = "";

            if (String.IsNullOrWhiteSpace(nombreChecklist))
            {
                sonCamposValidos = false;
                txtBoxNombreChecklist.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                razones += "Nombre checklist";
            }
            else
            {
                txtBoxNombreChecklist.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            if (String.IsNullOrWhiteSpace(descripcionChecklist))
            {
                sonCamposValidos = false;
                txtBoxDescripcionChecklist.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                razones = (razones.Length > 0) ? razones + ", Descripción" : "Descripción";
            }
            else
            {
                txtBoxDescripcionChecklist.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }
            if (listIdPoliticas_.Count <= 0)
            {
                sonCamposValidos = false;
                scrViewerPoliticas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CE7F77"));
                razones = (razones.Length > 0) ? razones + ", Politicas" : "Politicas";
            }
            else
            {
                scrViewerPoliticas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2F0EF"));
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

        public void CargarPoliticasOtorgamiento(List<int> listIdPoliticas, List<string> listNombrePoliticas, string nombrePolitica, string descripcionPolitica)
        {
            listNombrePoliticas_ = listNombrePoliticas;
            foreach (string politica in listNombrePoliticas)
            {
                AgregarMensaje(politica);
            }
            listIdPoliticas_ = listIdPoliticas;
            txtBoxNombreChecklist.Text = nombrePolitica;
            txtBoxDescripcionChecklist.Text = descripcionPolitica;
        }

        private void AgregarMensaje(string nombrePolitica)
        {
            Label politicaOtorgamiento = new Label();
            politicaOtorgamiento.Style = (Style)FindResource("estiloLabelChecklist");
            politicaOtorgamiento.Content = nombrePolitica;
            stcPanelPolitica.Children.Add(politicaOtorgamiento);
        }

        private void ClicAñadirPoliticas(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ListaSeleccionPoliticasPagina seleccionPoliticas = new ListaSeleccionPoliticasPagina();
            string nombre = txtBoxNombreChecklist.Text;
            string descripcion = txtBoxDescripcionChecklist.Text;
            seleccionPoliticas.EnviarDatos(listIdPoliticas_,listNombrePoliticas_,nombre,descripcion);
            ventanaPrincipal.CambiarPagina(seleccionPoliticas);
        }

        private void TextChangedtxtBoxNombre(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoNombre = 25;
            if (txtBoxNombreChecklist.Text.Length > tamañoMaximoNombre)
            {
                txtBoxNombreChecklist.Text = txtBoxNombreChecklist.Text.Substring(0, tamañoMaximoNombre);
                txtBoxNombreChecklist.SelectionStart = txtBoxNombreChecklist.Text.Length;
            }
        }

        private void TextChangedtxtBoxDescripcion(object sender, TextChangedEventArgs e)
        {
            int tamañoMaximoNombre = 45;
            if (txtBoxDescripcionChecklist.Text.Length > tamañoMaximoNombre)
            {
                txtBoxDescripcionChecklist.Text = txtBoxDescripcionChecklist.Text.Substring(0, tamañoMaximoNombre);
                txtBoxDescripcionChecklist.SelectionStart = txtBoxDescripcionChecklist.Text.Length;
            }
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            MenuPrincipalAdministradorPagina menu = new MenuPrincipalAdministradorPagina();
            ventanaPrincipal.CambiarPagina(menu);
        }
    }
}
