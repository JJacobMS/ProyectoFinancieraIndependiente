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

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para ListaCreditosPagina.xaml
    /// </summary>
    public partial class ListaCreditosPagina : Page
    {
        private List<Credito> _creditos;
        public ListaCreditosPagina()
        {
            InitializeComponent();
            CargarPaginaLista();
        }
        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MenuPrincipalAsesorCreditoPagina menu = new MenuPrincipalAsesorCreditoPagina();
            MainWindow main = (MainWindow) Window.GetWindow(this);
            main.CambiarPagina(menu);
        }

        private void CargarPaginaLista()
        {
            RecuperarCreditos();
        }

        private void RecuperarCreditos()
        {
            _creditos = new List<Credito>();
            Codigo codigo;
            try
            {
                Credito[] creditos;
                CreditoClient proxy = new CreditoClient();
                (creditos, codigo) = proxy.RecuperarCreditosRegistrados();
                if (creditos != null)
                {
                    _creditos = creditos.ToList();
                }
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }


            switch (codigo)
            {
                case Codigo.EXITO:
                    if (_creditos.Count == 0)
                    {
                        MostrarNoHayCreditosRegistrados();
                    }
                    else
                    {
                        CargarListaCreditos(_creditos);
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

        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje ventana = new VentanaMensaje("Error. No se pudo conectar con el servidor.Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventana.Mostrar();
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje ventana = new VentanaMensaje("Error. No se pudo conectar con la base de datos.Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventana.Mostrar();
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
            List<Credito> creditosFiltrados = _creditos;
            ComboBoxItem estatus = (ComboBoxItem) cmbBoxEstatus.SelectedItem;
            string busqueda = txtBoxBarraBuscar.Text.Trim();
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                 creditosFiltrados = _creditos.Where(
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
