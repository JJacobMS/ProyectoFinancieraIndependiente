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
        private List<string> _listNombrePoliticas = new List<string>() { };
        private List<int> _listIdPoliticas = new List<int>();
        private Checklist _checklist;
        private Politica[] politicas_;
        private int _idChecklist;

        public FormularioChecklistPagina()
        {
            InitializeComponent();
        }
        public FormularioChecklistPagina(Checklist checklist)
        {
            InitializeComponent();
            _checklist = checklist;
            _idChecklist = checklist.idChecklist;
            Console.WriteLine(_idChecklist);
            RecuperarChecklist();
        }

        private void RecuperarChecklist() 
        {
            Codigo codigo = new Codigo();
            try
            {
                politicas_ = new Politica[] { };
                ChecklistClient proxy = new ChecklistClient();
                (codigo, politicas_) = proxy.RecuperarPoliticasChecklistIdChecklist(_checklist.idChecklist);
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
                    CargarChecklist();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    DeshabilitarBotones();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    DeshabilitarBotones();
                    break;
            }
        }

        private void CargarChecklist()
        {
            foreach (Politica politica in politicas_)
            {
                _listIdPoliticas.Add(politica.idPolitica);
                _listNombrePoliticas.Add(politica.nombre);
            }
            CargarPoliticasOtorgamiento(_listIdPoliticas, _listNombrePoliticas, _checklist.nombre, _checklist.descripcion, _idChecklist);
        }

        private void DeshabilitarBotones()
        {
            btnAceptar.IsEnabled = false;
            btnAñadirPoliticas.IsEnabled = false;
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
            VentanaMensaje ventana;
            if (_idChecklist==0) 
            {
                ventana = new VentanaMensaje("¿Desea registrar el Checklist?", Mensaje.CONFIRMACION);
            }
            else
            {
                ventana = new VentanaMensaje("¿Desea actualizar el Checklist?", Mensaje.CONFIRMACION);
            }
            if (ventana.MostrarConfirmacion())
            {
                GuardarChecklist();
            }
        }

        private void GuardarChecklist()
        {
            Codigo codigo = new Codigo();
            int[] arrayIdPoliticas = new int[_listIdPoliticas.Count];
            for (int i = 0; i < _listIdPoliticas.Count; i++)
            {
                arrayIdPoliticas[i] = _listIdPoliticas[i];
            }
            try
            {
                Checklist checklist = new Checklist
                {
                    nombre = txtBoxNombreChecklist.Text,
                    descripcion = txtBoxDescripcionChecklist.Text
                };
                ChecklistClient proxy = new ChecklistClient();
                if(_idChecklist==0)
                {
                    codigo = proxy.GuardarChecklist(checklist, arrayIdPoliticas);
                }
                else 
                {
                    checklist.idChecklist = _idChecklist;
                    codigo = proxy.ActualizarChecklist(checklist, arrayIdPoliticas);
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
            VentanaMensaje mensajeError;
            if (_idChecklist != 0) 
            {
                mensajeError = new VentanaMensaje("Se ha registrado el checklist exitosamente", Mensaje.EXITO);
            }
            else
            {
                mensajeError = new VentanaMensaje("Se ha actualizado el checklist exitosamente", Mensaje.EXITO);
            }
            mensajeError.Mostrar();
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
            bool sonPoliticasValidas = true;
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
            if (_listIdPoliticas.Count <= 0)
            {
                sonCamposValidos = false;
                sonPoliticasValidas = false;
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
                if (!sonPoliticasValidas)
                {
                    MostrarVentanaPoliticasNoValidas();
                }
            }
            
            return sonCamposValidos;
        }
        
        private void MostrarVentanaCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
        }
        
        private void MostrarVentanaPoliticasNoValidas()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("“El checklist debe tener por lo menos una política de otorgamiento", Mensaje.ERROR);
            mensajeError.Mostrar();
        }

        public void CargarPoliticasOtorgamiento(List<int> listIdPoliticas, List<string> listNombrePoliticas, string nombrePolitica, string descripcionPolitica, int idChecklist)
        {
            stcPanelPolitica.Children.Clear();
            foreach (string politica in listNombrePoliticas)
            {
                AgregarMensaje(politica);
            }
            _listIdPoliticas = listIdPoliticas;
            _listNombrePoliticas = listNombrePoliticas;
            _idChecklist = idChecklist;
            Console.WriteLine(_idChecklist);
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
            Console.WriteLine(_idChecklist);
            seleccionPoliticas.EnviarDatos(_listIdPoliticas,_listNombrePoliticas,nombre,descripcion, _idChecklist);
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
            Cerrar();
        }

        private void Cerrar()
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            MenuPrincipalAdministradorPagina menu = new MenuPrincipalAdministradorPagina();
            ventanaPrincipal.CambiarPagina(menu);
        }
    }
}
