using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para ListaCreditosPagina.xaml
    /// </summary>
    public partial class ListaCreditosPagina : Page
    {
        private List<Credito> creditos;
        public ListaCreditosPagina()
        {
            InitializeComponent();
            RecuperarCreditos();
        }
        private void ClicRegresar(object sender, RoutedEventArgs e)
        {

        }
        private void RecuperarCreditos()
        {
            Cliente cl = new Cliente
            {
                rfc = "QU27805274802"
            };
            EstatusCredito estatusA = new EstatusCredito { nombre = "Aprobado"};
            EstatusCredito estatusR = new EstatusCredito { nombre = "Rechazado" };

            creditos = new List<Credito> { new Credito { folioCredito = 163913951, Cliente = cl, monto = 202109, fechaSolicitud = DateTime.Now, EstatusCredito = estatusR },
            new Credito { folioCredito = 2, Cliente = cl, monto = 109, fechaSolicitud = DateTime.Now, EstatusCredito = estatusA }};
            //creditos = new List<Credito>();
            if (creditos.Count == 0)
            {
                MostrarNoHayCreditosRegistrados();
            }
            else
            {
                CargarListaCreditos(creditos);
            }
        }

        private void MostrarNoHayCreditosRegistrados()
        {
            lstBoxCreditos.Style = (Style)FindResource("estiloLstBoxCreditosVacia");
            btnBuscar.IsEnabled = false;
        }

        private void CargarListaCreditos(List<Credito> creditos)
        {
            lstBoxCreditos.ItemsSource = creditos;
        }

        private void ClicBuscar(object sender, RoutedEventArgs e)
        {
            List<Credito> creditosFiltrados = FiltrarCreditosRegistrados();
            if (creditosFiltrados.Count == 0)
            {
                MostrarCreditosNoEncontrados();
            }
            else
            {
                lstBoxCreditos.Style = null;
                CargarListaCreditos(creditosFiltrados);
            }
        }

        private List<Credito> FiltrarCreditosRegistrados()
        {
            List<Credito> creditosFiltrados = creditos;
            ComboBoxItem estatus = (ComboBoxItem) cmbBoxEstatus.SelectedItem;
            string busqueda = txtBoxBarraBuscar.Text.Trim();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                 creditosFiltrados = creditos.Where(
                    credito => credito.folioCredito.ToString().Contains(busqueda)).ToList();
            }

            if (estatus != null)
            {
                creditosFiltrados = creditosFiltrados.Where(
                    credito => credito.EstatusCredito.nombre.Equals(estatus.Content)).ToList();
            }
            return creditosFiltrados;           
        }

        private void MostrarCreditosNoEncontrados()
        {
            lstBoxCreditos.Style = (Style)FindResource("estiloLstBoxCreditosSinBusqueda");
        }
        private void ClicVerMas(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Credito credito = btn.CommandParameter as Credito;
            DetallesCreditoPagina p = new DetallesCreditoPagina(credito);
            MainWindow m = (MainWindow)Window.GetWindow(this);
            m.CambiarPagina(p);
        }
    }
}
