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
    /// Lógica de interacción para FormularioSolicitudCreditoPagina.xaml
    /// </summary>
    public partial class FormularioSolicitudCreditoPagina : Page
    {
        private static readonly string PREFIJO_DATOS_OCULTOS = "••••";

        private ClienteRFC clienteActual = new ClienteRFC();
        private string[] clienteTelefonos = new string[0];
        private string[] clienteTelefonos = new string[0];
        private bool seMostroVentanaErrorBD = false;
        private bool seMostroVentanaErrorServidor = false;
        private CondicionCredito condicionCreditoActual;
        private Checklist checklistActual;
        private int montoActual;

        public FormularioSolicitudCreditoPagina(ClienteRFC clienteActual, string[] clienteTelefonos)
        {
            InitializeComponent();

            this.clienteActual = clienteActual;
            this.clienteTelefonos = clienteTelefonos;
        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            txtBlockCorreo.Text = clienteActual.CorreoElectronico;
            txtBlockCuentaCobro.Text = PREFIJO_DATOS_OCULTOS + clienteActual.CuentaCobro;
            txtBlockCuentaDeposito.Text = PREFIJO_DATOS_OCULTOS + clienteActual.CuentaDeposito;
            txtBlockNombre.Text = clienteActual.Nombres + " " + clienteActual.Apellidos;
            txtBlockTelefonoCasa.Text = PREFIJO_DATOS_OCULTOS + clienteTelefonos[1];
            txtBlockTelefonoPersonal.Text = PREFIJO_DATOS_OCULTOS + clienteTelefonos[3];

            ObtenerCondicionesCredito();
            ObtenerChecklists();
        }

        private void ObtenerChecklists()
        {
            Codigo codigoRespuesta = new Codigo();
            Checklist[] listaChecklists = new Checklist[0];

            try
            {
                ChecklistClient checklistSolClient = new ChecklistClient();
                var respuesta = checklistSolClient.ObtenerChecklists();
                var (codigo, checklists) = respuesta;
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
                    if (listaChecklists != null)
                    {
                        lstBoxChecklists.ItemsSource = listaChecklists;
                    }
                    else
                    {
                        bdrChecklistTabla.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                        MostrarMensajeNoChecklists();
                        btnSolicitarCredito.IsEnabled = false;
                    }
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
            }
        }

        private void MostrarMensajeNoChecklists()
        {
            txtBlockMensajeNoChecklists.Visibility = Visibility.Visible;
            lstBoxChecklists.Visibility = Visibility.Collapsed;
        }

        private void MostrarVentanaErrorBD()
        {
            if (seMostroVentanaErrorBD)
            {
                VentanaMensaje errorServidor = new VentanaMensaje(
                "Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
                errorServidor.Mostrar();

                seMostroVentanaErrorBD = true;
            }
        }

        private void MostrarVentanaErrorServidor()
        {
            if (seMostroVentanaErrorServidor)
            {
                VentanaMensaje errorBaseDatos = new VentanaMensaje(
               "Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
                errorBaseDatos.Mostrar();

                seMostroVentanaErrorServidor = true;
            }
        }

        private void ObtenerCondicionesCredito()
        {
            CondicionCreditoClient condicionCreditoClient = new CondicionCreditoClient();
            var respuesta = condicionCreditoClient.ObtenerCondicionesCreditoActivas();
            var (codigo, condicionesCredito) = respuesta;

            switch (codigo)
            {
                case Codigo.EXITO:
                    if (condicionesCredito != null)
                    {
                        lstBoxCondicionesCredito.ItemsSource = condicionesCredito;
                    }
                    else
                    {
                        bdrCondicionesCreditoTabla.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                        MostrarMensajeNoCondiciones();
                        btnSolicitarCredito.IsEnabled = false;
                    }
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
            }
        }

        private void MostrarMensajeNoCondiciones()
        {
            txtBlockMensajeNoCondiciones.Visibility = Visibility.Visible;
            lstBoxCondicionesCredito.Visibility = Visibility.Collapsed;
            grdEncabezadosCondicionCredito.Visibility = Visibility.Collapsed;
        }

        private void ClicSolicitarCredito(object sender, RoutedEventArgs e)
        {
            string monto = txtBoxMonto.Text;
            CondicionCredito condicionSeleccionada = lstBoxCondicionesCredito.SelectedItem as CondicionCredito;
            Checklist checklistSeleccionada = lstBoxChecklists.SelectedItem as Checklist;

            condicionCreditoActual = condicionSeleccionada;
            checklistActual = checklistSeleccionada;
            SolicitarCredito(clienteActual, monto, condicionSeleccionada, checklistSeleccionada);
        }

        private void SolicitarCredito(ClienteRFC clienteActual, string monto, CondicionCredito condicionSeleccionada, Checklist checklistSeleccionada)
        {
            if (ValidarDatos(monto))
            {
                MostrarVentanaConfirmacion();
            }
        }

        private bool ValidarDatos(string monto)
        {
            txtBoxMonto.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#A9D4D6");
            bool esValido = true;

            if (string.IsNullOrEmpty(monto))
            {
                txtBoxMonto.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                MostrarVentanaCamposNoValidos("Monto");
                esValido = false;
            }
            else
            {
                montoActual = int.Parse(monto);
            }

            return esValido;
        }

        private void MostrarVentanaCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventana = new VentanaMensaje($"Crédito {condicionCreditoActual.descripcion}  por ${montoActual}", 
                Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                GuardarInformacionSolicitud();
            }
        }

        private void GuardarInformacionSolicitud()
        {
            Codigo codigo;

            Credito credito = new Credito();
            credito.Cliente_idCliente = clienteActual.IdCliente;
            credito.Checklist_idChecklist = checklistActual.idChecklist;
            credito.CondicionCredito_idCondicionCredito = condicionCreditoActual.idCondicionCredito;
            credito.monto = montoActual;
            credito.fechaSolicitud = DateTime.Now;
            credito.saldoPendiente = montoActual;
            credito.deudaExtra = 0;

            CreditoClient creditoClient = new CreditoClient();
            codigo = creditoClient.GuardarInformacionSolicitud(credito);

            switch(codigo)
            {
                case Codigo.EXITO:
                    MostrarVentanaRegistroExito();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
            }
        }

        private void MostrarVentanaRegistroExito()
        {
            VentanaMensaje exito = new VentanaMensaje(
                            "Se ha registrado la solicitud de credito exitosamente", Mensaje.EXITO);
            exito.Mostrar();
        }

        private void PrevenirTextoNoNumerico(object sender, TextCompositionEventArgs e)
        {
            if (!EsNumerico(e.Text) || e.Text.Contains(" "))
            {
                e.Handled = true;
            }
        }

        private bool EsNumerico(string texto)
        {
            return int.TryParse(texto, out _);
        }

        private void SeleccionarCondicionCredito(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (lstBoxChecklists.SelectedItem != null)
            {
                btnSolicitarCredito.IsEnabled = true;
            }
        }

        private void SeleccionarChecklist(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (lstBoxCondicionesCredito.SelectedItem != null)
            {
                btnSolicitarCredito.IsEnabled = true;
            }
        }

        private void ClicRegresar(object sender, MouseButtonEventArgs e)
        {
            BusquedaRFCCliente busquedaRFCCliente = new BusquedaRFCCliente();

            NavigationService.Navigate(busquedaRFCCliente);
        }
    }
}
