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
    /// Lógica de interacción para FormularioSolicitudCreditoPagina.xaml
    /// </summary>
    public partial class FormularioSolicitudCreditoPagina : Page
    {
        private static readonly string PREFIJO_DATOS_OCULTOS = "••••";

        private Cliente clienteActual = new Cliente();
        private string[] clienteTelefonos = new string[0];
        private bool seMostroVentanaErrorBD = false;
        private bool seMostroVentanaErrorServidor = false;

        public FormularioSolicitudCreditoPagina(Cliente clienteActual, string[] clienteTelefonos)
        {
            InitializeComponent();

            this.clienteActual = clienteActual;
            this.clienteTelefonos = clienteTelefonos;
        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            txtBlockCorreo.Text = clienteActual.correoElectronico;
            txtBlockCuentaCobro.Text = PREFIJO_DATOS_OCULTOS + clienteActual.cuentaCobro;
            txtBlockCuentaDeposito.Text = PREFIJO_DATOS_OCULTOS + clienteActual.cuentaDeposito;
            txtBlockNombre.Text = clienteActual.nombres + " " + clienteActual.apellidos;
            txtBlockTelefonoCasa.Text = PREFIJO_DATOS_OCULTOS + clienteTelefonos[1];
            txtBlockTelefonoPersonal.Text = PREFIJO_DATOS_OCULTOS + clienteTelefonos[3];

            ObtenerCondicionesCredito();
            ObtenerChecklists();
        }

        private void ObtenerChecklists()
        {
            ChecklistSolClient checklistSolClient = new ChecklistSolClient();
            var respuesta = checklistSolClient.ObtenerChecklists();
            var (codigo, checklists) = respuesta;

            switch (codigo)
            {
                case Codigo.EXITO:
                    if (checklists != null)
                    {
                        lstBoxChecklists.ItemsSource = checklists;
                    }
                    else
                    {
                        bdrChecklistTabla.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                        txtBlockMensajeNoChecklists.Visibility = Visibility.Visible;
                        lstBoxChecklists.Visibility = Visibility.Collapsed;
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
                        txtBlockMensajeNoCondiciones.Visibility = Visibility.Visible;
                        lstBoxCondicionesCredito.Visibility = Visibility.Collapsed;
                        grdEncabezadosCondicionCredito.Visibility = Visibility.Collapsed;
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

        private void ClicSolicitarCredito(object sender, RoutedEventArgs e)
        {

        }
    }
}
