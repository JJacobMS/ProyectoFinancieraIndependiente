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
        private List<int> _listIdPoliticas;
        private List<int> _listIdPoliticasAntiguas;
        private List<string> _listNombrePoliticas;
        private List<string> _listNombrePoliticasAntiguas;
        private Politica[] _politicas;
        private Checklist _checklist;
        private string _descripcion;
        private string _nombre;
        private bool _esVerPoliticas;
        private int _idChecklist;
        public ListaSeleccionPoliticasPagina()
        {
            InitializeComponent();
            btnAceptarCambios.Content = "Aceptar Cambios";
        }

        public ListaSeleccionPoliticasPagina(Checklist checklist)
        {
            InitializeComponent();
            _esVerPoliticas = true;
            _checklist = checklist;
            Console.WriteLine(_idChecklist);
            _idChecklist = checklist.idChecklist;
            _politicas = new Politica[] { };
            GrdViewColumnCheckbox.Width = 0;
            GrdViewColumnCheckbox.CellTemplate = new DataTemplate(); 
            btnAceptarCambios.Content = "Actualizar Checklist";
            RecuperarPoliticas();
        }

        private void ClicAceptarCambios(object sender, RoutedEventArgs e)
        {
            if (_esVerPoliticas)
            {
                MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
                FormularioChecklistPagina formularioChecklist = new FormularioChecklistPagina(_checklist);
                ventanaPrincipal.CambiarPagina(formularioChecklist);
            }
            else
            {
                Console.WriteLine(_idChecklist);
                MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
                FormularioChecklistPagina formularioChecklist = new FormularioChecklistPagina();
                formularioChecklist.CargarPoliticasOtorgamiento(_listIdPoliticas, _listNombrePoliticas, _nombre, _descripcion, _idChecklist);
                ventanaPrincipal.CambiarPagina(formularioChecklist);
            }
            
        }

        public void EnviarDatos(List<int> listIdPoliticas, List<string> listNombrePoliticas, string nombre, string descripcion, int idChecklist)
        {
            _listIdPoliticas = new List<int>();
            _politicas = new Politica[] { };
            _listNombrePoliticasAntiguas = new List<string>(listNombrePoliticas);
            _listNombrePoliticas = listNombrePoliticas;
            _listIdPoliticas = listIdPoliticas;
            _listIdPoliticasAntiguas = new List<int>(listIdPoliticas);
            _nombre = nombre;
            _descripcion = descripcion;
            _idChecklist = idChecklist;
            Console.WriteLine(_idChecklist);
            RecuperarPoliticas();
        }

        private void RecuperarPoliticas()
        {
            Codigo codigo = new Codigo();
            _politicas = new Politica[] { };

            try
            {
                if (_esVerPoliticas)
                {
                    ChecklistClient proxy = new ChecklistClient();
                    (codigo, _politicas) = proxy.RecuperarPoliticasChecklistIdChecklist(_checklist.idChecklist);
                }
                else
                {
                    PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                    (codigo, _politicas) = proxy.RecuperarPoliticas();
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
            if (_esVerPoliticas) 
            {
                MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
                ChecklistPagina checklistPagina = new ChecklistPagina();
                ventanaPrincipal.CambiarPagina(checklistPagina);
            } 
            else 
            {
                MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
                FormularioChecklistPagina formularioChecklist = new FormularioChecklistPagina();
                formularioChecklist.CargarPoliticasOtorgamiento(_listIdPoliticasAntiguas, _listNombrePoliticasAntiguas, _nombre, _descripcion, _idChecklist);
                ventanaPrincipal.CambiarPagina(formularioChecklist);
            }

        }

        private void CargarPoliticasOtorgamiento()
        {
            _listNombrePoliticas = new List<string> { };
            if (_listIdPoliticas != null && _listIdPoliticas.Count() > 0 && _politicas != null && _politicas.Length > 0)
            {
                foreach (int idPolitica in _listIdPoliticas)
                {
                    foreach (Politica politica in _politicas)
                    {
                        if (politica.idPolitica == idPolitica)
                        {
                            _listNombrePoliticas.Add(politica.nombre);
                            politica.checkbox = true;
                            break;
                        }
                    }
                }
            }
            if (_politicas != null && _politicas.Length > 0)
            {
                lstViewPoliticasOtorgamiento.ItemsSource = _politicas;
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
            _listIdPoliticas.Add(politica.idPolitica);
            _listNombrePoliticas.Add(politica.nombre);

        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Politica politica = checkBox.CommandParameter as Politica;
            _listIdPoliticas.Remove(politica.idPolitica);
            _listNombrePoliticas.Remove(politica.nombre);

        }

        private void FiltrarCampos() 
        {
            lstViewPoliticasOtorgamiento.ItemsSource = null;
            lstViewPoliticasOtorgamiento.Items.Clear();

            DateTime? fechavigencia = dtPickerFechaVigencia.SelectedDate?.Date;
            DateTime fechaInicioDia = DateTime.MinValue;
            DateTime fechaFinDia = DateTime.MinValue;
            if (fechavigencia.HasValue)
            {
                fechaInicioDia = new DateTime(fechavigencia.Value.Year, fechavigencia.Value.Month, fechavigencia.Value.Day);
                fechaFinDia = fechaInicioDia.AddDays(1).AddTicks(-1);
            }
            string politicaBusqueda = txtBoxBarraBuscar.Text.ToLower();
            if(_politicas != null && _politicas.Count() > 0 )
            {
                Politica[] politicasFiltradasNombre = _politicas.Where(politica => politica.nombre.ToLower().Contains(politicaBusqueda)).ToArray();
                Politica[] politicasFiltradasDescripcion = _politicas.Where(politica => politica.descripcion.ToLower().Contains(politicaBusqueda)).ToArray();
                Politica[] politicasFiltradasPorFecha = _politicas.Where(politica => politica.vigencia.Date >= fechaInicioDia && politica.vigencia.Date <= fechaFinDia).ToArray();
                HashSet<Politica> conjuntoPoliticas = new HashSet<Politica>(politicasFiltradasPorFecha);
                conjuntoPoliticas.UnionWith(politicasFiltradasNombre);
                conjuntoPoliticas.UnionWith(politicasFiltradasDescripcion);

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
