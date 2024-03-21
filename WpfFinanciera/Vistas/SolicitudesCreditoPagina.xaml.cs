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
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para SolicitudesCreditoPagina.xaml
    /// </summary>
    public partial class SolicitudesCreditoPagina : Page
    {
        SolicitudCredito[] listaSolicitudesCredito;

        public SolicitudesCreditoPagina()
        {
            InitializeComponent();
        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            CreditoClient creditoClient = new CreditoClient();
            var respuesta = creditoClient.ObtenerSolicitudesCredito();
            var (codigo, solicitudesCredito) = respuesta;

            switch (codigo)
            {
                case Codigo.EXITO:
                    lstBoxSolicitudesCredito.ItemsSource = solicitudesCredito;
                    listaSolicitudesCredito = solicitudesCredito;
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
            }
        }

        private void ClicEvaluarSolicitud(object sender, RoutedEventArgs e)
        {
           
        }

        private void MostrarVentanaErrorBD()
        {
            VentanaMensaje errorServidor = new VentanaMensaje(
                "Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            errorServidor.Mostrar();
        }

        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje errorBaseDatos = new VentanaMensaje(
                "Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            errorBaseDatos.Mostrar();
        }

        private void ClicBuscar(object sender, RoutedEventArgs e)
        {
            string filtro = txtBoxBusquedaRFCSolicitudCredito.Text.ToLower();

            List<SolicitudCredito> listaFiltrada = FiltrarLista(filtro);

            lstBoxSolicitudesCredito.ItemsSource = listaFiltrada;
        }

        private List<SolicitudCredito> FiltrarLista(string filtro)
        {
            List<SolicitudCredito> listaFiltrada = new List<SolicitudCredito>();

            if (listaSolicitudesCredito != null && !string.IsNullOrEmpty(filtro))
            {
                foreach (var solicitud in listaSolicitudesCredito)
                {
                    if (solicitud.RfcCliente.ToLower().Contains(filtro))
                    {
                        listaFiltrada.Add(solicitud);
                    }
                }
            }
            else
            {
                listaFiltrada = new List<SolicitudCredito>(listaSolicitudesCredito);
            }

            return listaFiltrada;
        }

        private void IngresarFecha(object sender, SelectionChangedEventArgs e)
        {
            if (dtPickerBusquedaFechaSolicitudCredito.SelectedDate.HasValue)
            {
                DateTime fechaSeleccionada = dtPickerBusquedaFechaSolicitudCredito.SelectedDate.Value;
                List<SolicitudCredito> listaFiltrada = FiltrarListaPorFecha(fechaSeleccionada);

                lstBoxSolicitudesCredito.ItemsSource = listaFiltrada;
            }
        }

        private List<SolicitudCredito> FiltrarListaPorFecha(DateTime fecha)
        {
            List<SolicitudCredito> listaFiltrada = new List<SolicitudCredito>();

            if (listaSolicitudesCredito != null)
            {
                foreach (var solicitud in listaSolicitudesCredito)
                {
                    if (solicitud.TiempoSolicitud.Date == fecha.Date)
                    {
                        listaFiltrada.Add(solicitud);
                    }
                }
            }
            else
            {
                listaFiltrada = new List<SolicitudCredito>(listaSolicitudesCredito);
            }

            return listaFiltrada;
        }

        private void ClicRegresar(object sender, MouseButtonEventArgs e)
        {
            MenuPrincipalAnalistaCreditoPagina menuPrincipal = new MenuPrincipalAnalistaCreditoPagina();

            NavigationService.Navigate(menuPrincipal);
        }
    }
}
