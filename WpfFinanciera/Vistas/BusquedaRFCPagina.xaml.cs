using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para BusquedaRFCCliente.xaml
    /// </summary>
    public partial class BusquedaRFCCliente : Page
    {
        private Cliente cliente;

        public BusquedaRFCCliente()
        {
            InitializeComponent();
        }

        private void ClicBuscarCliente(object sender, RoutedEventArgs e)
        {
            string rfc = txtBoxRfc.Text.Trim();

            bool esValido = ValidarRFC(rfc);

            if (esValido)
            {
                BuscarClientePorRFC(rfc);
            }
        }

        private void BuscarClientePorRFC(string rfc)
        {
            ClienteRFCClient clienteRFCClient = new ClienteRFCClient();

            var respuesta = clienteRFCClient.BuscarClientePorRFC(rfc);
            var (codigo, cliente) = respuesta;

            switch (codigo)
            {
                case Codigo.EXITO:
                    if(cliente != null)
                    {
                        this.cliente = cliente;

                        MostrarOpciones();
                    }
                    else
                    {
                        //TODO Lógica para redirigir a Registro
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

        private void MostrarOpciones()
        {
            btnBuscar.Visibility = Visibility.Collapsed;
            txtBoxRfc.IsEnabled = false;
            stkPanelOpciones.Visibility = Visibility.Visible;
            txtBlockRfcCliente.Text = "Cliente encontrado";
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

        private void ClicSolicitarCredito(object sender, RoutedEventArgs e)
        {
            
        }

        private void ClicActualizarDatos(object sender, RoutedEventArgs e)
        {
            //TODO Redirección a Actualizar Datos de Cliente
        }

        private void CambioTextoRfc(object sender, TextChangedEventArgs e)
        {
            if (txtBoxRfc.Text.Length == 13)
            {
                btnBuscar.IsEnabled = true;
            }
            else
            {
                btnBuscar.IsEnabled = false;
            }
        }

        private void PrevenirTextoNoAlfanumerico(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool ValidarRFC(string rfc)
        {
            bool esValido = true;
            string patronRFC = @"^[a-zA-Z]{4}[0-9]{6}[a-zA-Z0-9]{3}$";
            string razones = "";

            if (string.IsNullOrEmpty(txtBoxRfc.Text.Trim()))
            {
                esValido = false;
                razones = "RFC";
            } 
            else
            {
                esValido = Regex.IsMatch(rfc, patronRFC);
                razones = "RFC. Recuerda que el formato es ABCD123456 + Homoclave";
            }

            if(!esValido)
            {
                MostrarVentanaCamposNoValidos(razones);
            }

            return esValido;
        }

        private void MostrarVentanaCamposNoValidos(string razones)
        {
            VentanaMensaje camposIncorrectos = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            camposIncorrectos.Mostrar();
        }
    }
}
