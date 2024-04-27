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
    /// Lógica de interacción para CondicionesCreditoPagina.xaml
    /// </summary>
    public partial class CondicionesCreditoPagina : Page
    {
        CondicionCredito[] _listaCondicionesCredito;

        public CondicionesCreditoPagina()
        {
            InitializeComponent();
        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            ObtenerCondicionesCreditoRegistradas();
        }

        private void ObtenerCondicionesCreditoRegistradas()
        {
            Codigo codigoRespuesta = new Codigo();

            try
            {
                CondicionCreditoClient proxy = new CondicionCreditoClient();
                var respuesta = proxy.ObtenerCondicionesCreditoRegistradas();
                var (codigo, condicionesCredito) = respuesta;

                codigoRespuesta = codigo;
                _listaCondicionesCredito = condicionesCredito;
            }
            catch (CommunicationException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }
            catch (TimeoutException)
            {
                codigoRespuesta = Codigo.ERROR_SERVIDOR;
            }

            switch (codigoRespuesta)
            {
                case Codigo.EXITO:
                    CargarLista();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    IrMenuPrincipal();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    IrMenuPrincipal();
                    break;
            }
        }

        private void CargarLista()
        {
            lstBoxCondicionesCredito.ItemsSource = _listaCondicionesCredito;
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

        private void IrMenuPrincipal()
        {
            MenuPrincipalAdministradorPagina menuPrincipal = new MenuPrincipalAdministradorPagina();

            NavigationService.Navigate(menuPrincipal);
        }

        private void ClicRegresar(object sender, MouseButtonEventArgs e)
        {
            IrMenuPrincipal();
        }

        private void ClicBuscar(object sender, RoutedEventArgs e)
        {
            string busqueda = txtBoxBusquedaCondicionCredito.Text.Trim().ToLower();

            List<CondicionCredito> listaFiltrada = FiltrarLista(busqueda);

            lstBoxCondicionesCredito.ItemsSource = _listaCondicionesCredito;
        }

        private List<CondicionCredito> FiltrarLista(string busqueda)
        {
            List<CondicionCredito> listaFiltrada = new List<CondicionCredito>();

            if (_listaCondicionesCredito != null && !string.IsNullOrEmpty(busqueda))
            {
                foreach (var condicionCredito in _listaCondicionesCredito)
                {
                    if (condicionCredito.identificador.ToLower().Contains(busqueda) || condicionCredito.descripcion.ToLower().Contains(busqueda))
                    {
                        listaFiltrada.Add(condicionCredito);
                    }
                }
            }
            else
            {
                listaFiltrada = new List<CondicionCredito>(_listaCondicionesCredito);
            }

            return listaFiltrada;
        }

        private void ClicRegistro(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                CondicionCredito condicionCreditoSeleccionada = (CondicionCredito)lstBoxCondicionesCredito.SelectedItem;

                if (condicionCreditoSeleccionada != null)
                {
                    FormularioCondicionCreditoPagina formularioCondicion = new FormularioCondicionCreditoPagina(condicionCreditoSeleccionada);

                    NavigationService.Navigate(formularioCondicion);
                }
            }
        }
    }
}
