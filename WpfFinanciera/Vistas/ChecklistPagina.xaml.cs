using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Lógica de interacción para ChecklistPagina.xaml
    /// </summary>
    public partial class ChecklistPagina : Page
    {
        public List<Checklist> ListaDeChecklist { get; set; }
        private Checklist[] _checklist;
        public ChecklistPagina()
        {
            InitializeComponent();
            RecuperarChecklist();
        }

        private void RecuperarChecklist() 
        {
            Codigo codigo = new Codigo();
            _checklist = new Checklist[] { };
            try
            {
                ChecklistClient proxy = new ChecklistClient();
                (codigo, _checklist) = proxy.ObtenerChecklists();
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
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }
        
        private void CargarChecklist()
        {
            if (_checklist != null && _checklist.Length > 0)
            {
                lstBoxChecklist.ItemsSource = ListaDeChecklist;
                ListaDeChecklist = new List<Checklist>();
                foreach (Checklist checklist in _checklist)
                {
                    ListaDeChecklist.Add(checklist);
                }
                lstBoxChecklist.ItemsSource = ListaDeChecklist;
                this.DataContext = this;
            }
            else
            {
                MostrarVentanaChecklistVacios();
            }
        }

        private void MostrarVentanaChecklistVacios()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error.No existen checklist actualmente, Inténtelo más tarde", Mensaje.ERROR);
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

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MenuPrincipalAdministradorPagina menu = new MenuPrincipalAdministradorPagina();
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(menu);
        }

        private void ClicBuscarChecklist(object sender, RoutedEventArgs e)
        {
            FiltrarCampos();
            LimpiarCampos();
        }
        
        private void FiltrarCampos()
        {
            lstBoxChecklist.ItemsSource = null;
            lstBoxChecklist.Items.Clear();
            string checklistBusqueda = string.IsNullOrEmpty(txtBoxBarraBuscar.Text) ? string.Empty : txtBoxBarraBuscar.Text.ToLower();
            if (_checklist != null && _checklist.Count() > 0)
            {
                Checklist[] checklistFiltradasNombre = _checklist.Where(checklist => checklist.nombre.ToLower().Contains(checklistBusqueda)).ToArray();
                Checklist[] checklistFiltradasDescripcion = _checklist.Where(checklist => checklist.descripcion.ToLower().Contains(checklistBusqueda)).ToArray();
                HashSet<Checklist> conjuntoPoliticas = new HashSet<Checklist>(checklistFiltradasNombre);
                conjuntoPoliticas.UnionWith(checklistFiltradasDescripcion);
                Console.WriteLine(checklistFiltradasNombre.Length);
                Console.WriteLine(checklistFiltradasDescripcion.Length);

                List<Checklist> politicasUnicas = conjuntoPoliticas.ToList();
                lstBoxChecklist.ItemsSource = politicasUnicas;
            }
        }

        private void LimpiarCampos()
        {
            txtBoxBarraBuscar.Text = null;
        }
        
        private void ClicVerPoliticas(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Checklist seleccionChecklist = btn.CommandParameter as Checklist;
            ListaSeleccionPoliticasPagina politicasChecklist = new ListaSeleccionPoliticasPagina(seleccionChecklist);
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(politicasChecklist);
        }

        private void ClicEliminarChecklist(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Checklist checklist = btn.CommandParameter as Checklist;
            EliminarChecklist(checklist);
        }
       
        private void EliminarChecklist(Checklist checklist)
        {
            Codigo codigo = new Codigo();
            int filasAfectadas = 0;
            try
            {
                ChecklistClient proxy = new ChecklistClient();
                (codigo, filasAfectadas) = proxy.EliminarChecklist(checklist.idChecklist);
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
                    if (filasAfectadas != 0)
                    {
                        MostrarVentanaChecklistOcupado();
                    }
                    else
                    {
                        MostrarVentanaExito();
                        RecuperarChecklist();
                    }
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
            VentanaMensaje mensajeError = new VentanaMensaje("Se ha eliminado el checklist exitosamente", Mensaje.EXITO);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaChecklistOcupado()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("No se puede eliminar un checklist asociado a un crédito", Mensaje.ERROR);
            mensajeError.Mostrar();
        }
    }
}
