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
    /// Lógica de interacción para ListaSeleccionPoliticasPagina.xaml
    /// </summary>
    public partial class ListaSeleccionPoliticasPagina : Page
    {
        private List<int> listIdPoliticas_;
        private List<int> listIdPoliticasAntiguas_;
        private List<string> listNombrePoliticas_;
        private List<string> listNombrePoliticasAntiguas_;
        private Politica[] politicas_;
        private string descripcion_;
        private string nombre_;
        public ListaSeleccionPoliticasPagina()
        {
            InitializeComponent();
        }

        private void ClicAceptarCambios(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            FormularioChecklistPagina formularioChecklist = new FormularioChecklistPagina();
            formularioChecklist.CargarPoliticasOtorgamiento(listIdPoliticas_, listNombrePoliticas_, nombre_, descripcion_);
            ventanaPrincipal.CambiarPagina(formularioChecklist);
        }

        public void EnviarDatos(List<int> listIdPoliticas, List<string> listNombrePoliticas, string nombre, string descripcion)
        {
            listIdPoliticas_ = new List<int>();
            politicas_ = new Politica[] { };
            listNombrePoliticasAntiguas_ = new List<string>(listNombrePoliticas);
            listNombrePoliticas_ = listNombrePoliticas;
            listIdPoliticas_ = listIdPoliticas;
            listIdPoliticasAntiguas_ = new List<int>(listIdPoliticas);
            nombre_ = nombre;
            descripcion_ = descripcion;
            RecuperarPoliticas();
        }

        private void RecuperarPoliticas()
        {
            Codigo codigo = new Codigo();
            politicas_ = new Politica[] { };

            try
            {
                PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                (codigo, politicas_) = proxy.RecuperarPoliticas();
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
                    CargarPoliticasOtorgamiento();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }


        private void MostrarVentanaPoliticasVacias()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No existen politicas actualmente o no se encuentran vigentes, Intentelo mas tarde", Mensaje.ERROR);
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

        private void Cerrar()
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            FormularioChecklistPagina formularioChecklist = new FormularioChecklistPagina();
            formularioChecklist.CargarPoliticasOtorgamiento(listIdPoliticasAntiguas_, listNombrePoliticasAntiguas_, nombre_, descripcion_);
            ventanaPrincipal.CambiarPagina(formularioChecklist);
        }

        private void CargarPoliticasOtorgamiento()
        {
            listNombrePoliticas_ = new List<string> { };
            if (listIdPoliticas_ != null && listIdPoliticas_.Count() > 0 && politicas_ != null && politicas_.Length > 0)
            {
                foreach (int idPolitica in listIdPoliticas_)
                {
                    foreach (Politica politica in politicas_)
                    {
                        if (politica.idPolitica == idPolitica)
                        {
                            listNombrePoliticas_.Add(politica.nombre);
                            politica.checkbox = true;
                            break;
                        }
                    }
                }
            }
            if (politicas_ != null && politicas_.Length > 0)
            {
                lstViewPoliticasOtorgamiento.ItemsSource = politicas_;
            }
            else
            {
                MostrarVentanaPoliticasVacias();
            }
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Politica politica = checkBox.CommandParameter as Politica;
            listIdPoliticas_.Add(politica.idPolitica);
            listNombrePoliticas_.Add(politica.nombre);
        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Politica politica = checkBox.CommandParameter as Politica;
            listIdPoliticas_.Remove(politica.idPolitica);
            listNombrePoliticas_.Remove(politica.nombre);
        }

        private void FiltrarCampos() 
        {
            lstViewPoliticasOtorgamiento.ItemsSource = null;
            lstViewPoliticasOtorgamiento.Items.Clear();
            DateTime? fechavigencia = dtPickerFechaVigencia.SelectedDate;

            string politicaBusqueda = txtBoxBarraBuscar.Text.ToLower();
            if(politicas_ != null && politicas_.Count() > 0 )
            {
                Politica[] politicasFiltradasNombre = politicas_.Where(politica => politica.nombre.ToLower().Contains(politicaBusqueda)).ToArray();
                Politica[] politicasFiltradasDescripcion = politicas_.Where(politica => politica.descripcion.ToLower().Contains(politicaBusqueda)).ToArray();
                Politica[] politicasFiltradasPorFecha = politicas_.Where(politica => politica.vigencia == fechavigencia).ToArray();

                HashSet<Politica> conjuntoPoliticas = new HashSet<Politica>(politicasFiltradasNombre);
                conjuntoPoliticas.UnionWith(politicasFiltradasDescripcion);
                conjuntoPoliticas.UnionWith(politicasFiltradasPorFecha);

                List<Politica> politicasUnicas = conjuntoPoliticas.ToList();

                lstViewPoliticasOtorgamiento.ItemsSource = politicasUnicas;
            }
        }

        private void LimpiarCampos()
        {
            dtPickerFechaVigencia.SelectedDate = null;
            txtBoxBarraBuscar.Text = null;
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

        private void ClicBuscarPolitica(object sender, RoutedEventArgs e)
        {
            FiltrarCampos();
            LimpiarCampos();
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            Cerrar();
        }
    }
}
