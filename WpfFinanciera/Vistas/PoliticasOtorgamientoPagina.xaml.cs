using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para PoliticasOtorgamientoPagina.xaml
    /// </summary>
    public partial class PoliticasOtorgamientoPagina : Page
    {
        public List<Politica> ListaDePoliticas { get; set; }
        private Politica[] _politicas;
        private Politica[] _politicasFiltradasPorEstatus;

        public PoliticasOtorgamientoPagina()
        {
            InitializeComponent();
            RecuperarPoliticas();
            cmbBoxEstatus.Items.Add("Esta Activa");
            cmbBoxEstatus.Items.Add("Esta Inactiva");
        }

        private void RecuperarPoliticas()
        {
            Codigo codigo = new Codigo();
            _politicas = new Politica[] { };
            try
            {
                PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                (codigo, _politicas) = proxy.RecuperarPoliticas();
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
                    CargarPoliticas();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }

        private void CargarPoliticas() 
        {
            if (_politicas != null && _politicas.Length > 0)
            {
                lstBoxPoliticas.ItemsSource = ListaDePoliticas;
                ListaDePoliticas = new List<Politica>();
                foreach (Politica politica in _politicas)
                {
                    ListaDePoliticas.Add(politica);
                }
                lstBoxPoliticas.ItemsSource = ListaDePoliticas;
                this.DataContext = this;
            }
            else
            {
                MostrarVentanaPoliticasVacias();
            }
        }

        private void MostrarVentanaPoliticasVacias()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No existen politicas actualmente, Intentelo mas tarde", Mensaje.ERROR);
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

        private void ClicBuscarPolitica(object sender, RoutedEventArgs e)
        {
            FiltrarCampos();
            LimpiarCampos();
        }
        
        private void ObtenerCombobox()
        {
            bool estaActiva;
            _politicasFiltradasPorEstatus = new Politica[0];
            if (cmbBoxEstatus.SelectedItem != null)
            {
                estaActiva = true;
                string selectedItem = cmbBoxEstatus.SelectedItem.ToString();
                if (selectedItem == "Esta Inactiva")
                {
                    estaActiva = false;
                }
                _politicasFiltradasPorEstatus = _politicas.Where(politica => politica.estaActiva == estaActiva).ToArray();
            }
        }
        
        private void FiltrarCampos()
        {
            lstBoxPoliticas.ItemsSource = null;
            lstBoxPoliticas.Items.Clear();
            DateTime? fechavigencia = dtPickerFechaVigencia.SelectedDate?.Date;
            DateTime fechaInicioDia = DateTime.MinValue;
            DateTime fechaFinDia = DateTime.MinValue;
            if (fechavigencia.HasValue)
            {
                fechaInicioDia = new DateTime(fechavigencia.Value.Year, fechavigencia.Value.Month, fechavigencia.Value.Day);
                fechaFinDia = fechaInicioDia.AddDays(1).AddTicks(-1);
            }
            string politicaBusqueda = string.IsNullOrEmpty(txtBoxBarraBuscar.Text) ? string.Empty : txtBoxBarraBuscar.Text.ToLower();
            ObtenerCombobox();
            if (_politicas != null && _politicas.Count() > 0)
            {
                Politica[] politicasFiltradasNombre = _politicas.Where(politica => politica.nombre.ToLower().Contains(politicaBusqueda)).ToArray();
                Politica[] politicasFiltradasDescripcion = _politicas.Where(politica => politica.descripcion.ToLower().Contains(politicaBusqueda)).ToArray();
                Politica[] politicasFiltradasPorFecha = _politicas.Where(politica => politica.vigencia.Date >= fechaInicioDia && politica.vigencia.Date <= fechaFinDia).ToArray();
                HashSet<Politica> conjuntoPoliticas = new HashSet<Politica>(politicasFiltradasPorFecha);
                conjuntoPoliticas.UnionWith(_politicasFiltradasPorEstatus);
                conjuntoPoliticas.UnionWith(politicasFiltradasNombre);
                conjuntoPoliticas.UnionWith(politicasFiltradasDescripcion);
                List<Politica> politicasUnicas = conjuntoPoliticas.ToList();
                lstBoxPoliticas.ItemsSource = politicasUnicas;
            }
        }

        private void LimpiarCampos()
        {
            dtPickerFechaVigencia.SelectedDate = null;
            txtBoxBarraBuscar.Text = null;
            cmbBoxEstatus.SelectedItem = null;
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

        private void ClicActualizarPolitica(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Politica politica = btn.CommandParameter as Politica;
            FormularioPoliticaOtorgamientoPagina formularioPolitica = new FormularioPoliticaOtorgamientoPagina(politica);
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(formularioPolitica);
        }

        private void ClicEliminarPolitica(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Politica politica = btn.CommandParameter as Politica;
            EliminarPolitica(politica);
        }
        
        private void EliminarPolitica(Politica politica) 
        {
            Codigo codigo = new Codigo();
            int filasAfectadas = 0;
            try
            {
                PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                (codigo, filasAfectadas) = proxy.EliminarPolitica(politica.idPolitica);
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
                    RecuperarPoliticas();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    if (filasAfectadas == 0)
                    {
                        MostrarVentanaPoliticaOcupada();
                    }
                    else
                    {
                        MostrarVentanaErrorBaseDatos();
                    }
                    break;
            }
        }
        private void MostrarVentanaExito()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Se ha eliminado la politica exitosamente", Mensaje.EXITO);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaPoliticaOcupada()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("No se puede eliminar una politica asociada a un checklist", Mensaje.ERROR);
            mensajeError.Mostrar();
        }

    }
}
