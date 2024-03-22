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
    /// Lógica de interacción para SolicitudesCreditoPagina.xaml
    /// </summary>
    public partial class SolicitudesCreditoPagina : Page
    {
        SolicitudCredito[] _listaSolicitudesCredito;

        public SolicitudesCreditoPagina()
        {
            InitializeComponent();
        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            Codigo codigoRespuesta = new Codigo();
            SolicitudCredito[] listaSolicitudes = new SolicitudCredito[0];

            try
            {
                CreditoClient creditoClient = new CreditoClient();
                var respuesta = creditoClient.ObtenerSolicitudesCredito();
                var (codigo, solicitudesCredito) = respuesta;

                codigoRespuesta = codigo;
                listaSolicitudes = solicitudesCredito;
            }
            catch (CommunicationException ex)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            switch (codigoRespuesta)
            {
                case Codigo.EXITO:
                    lstBoxSolicitudesCredito.ItemsSource = listaSolicitudes;
                    _listaSolicitudesCredito = listaSolicitudes;
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
            Button btn = sender as Button;

            SolicitudCredito solicitudSeleccionada = btn.DataContext as SolicitudCredito;

            string nombreSolicitante = solicitudSeleccionada.Nombres + " " + solicitudSeleccionada.Apellidos;

            FormularioDictamenPagina formularioDictamen = new FormularioDictamenPagina(solicitudSeleccionada.FolioCredito, nombreSolicitante);

            NavigationService.Navigate(formularioDictamen);
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

            if (_listaSolicitudesCredito != null && !string.IsNullOrEmpty(filtro))
            {
                foreach (var solicitud in _listaSolicitudesCredito)
                {
                    if (solicitud.RfcCliente.ToLower().Contains(filtro))
                    {
                        listaFiltrada.Add(solicitud);
                    }
                }
            }
            else
            {
                listaFiltrada = new List<SolicitudCredito>(_listaSolicitudesCredito);
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

            if (_listaSolicitudesCredito != null)
            {
                foreach (var solicitud in _listaSolicitudesCredito)
                {
                    if (solicitud.TiempoSolicitud.Date == fecha.Date)
                    {
                        listaFiltrada.Add(solicitud);
                    }
                }
            }
            else
            {
                listaFiltrada = new List<SolicitudCredito>(_listaSolicitudesCredito);
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
