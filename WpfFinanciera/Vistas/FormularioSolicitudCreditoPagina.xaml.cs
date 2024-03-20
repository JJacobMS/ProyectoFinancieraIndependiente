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
    /// Lógica de interacción para FormularioSolicitudCreditoPagina.xaml
    /// </summary>
    public partial class FormularioSolicitudCreditoPagina : Page
    {
        private static readonly string PREFIJO_DATOS_OCULTOS = "••••";

        private Cliente clienteActual = new Cliente();
        private string[] clienteTelefonos = new string[0];

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

            CargarCondicionesCredito();
            CargarChecklists();
        }

        private void CargarChecklists()
        {

        }

        private void CargarCondicionesCredito()
        {

        }

        private void ClicSolicitarCredito(object sender, RoutedEventArgs e)
        {

        }
    }
}
