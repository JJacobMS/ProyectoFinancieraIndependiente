using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Lógica de interacción para FormularioDictamenPagina.xaml
    /// </summary>
    public partial class FormularioDictamenPagina : Page
    {
        public FormularioDictamenPagina()
        {
            InitializeComponent();
            RecuperarPoliticasChecklist();
        }

        
        private void RecuperarPoliticasChecklist()
        {
            Codigo codigo = new Codigo();
            Politica[] politicas = new Politica[]{};
            try
            {   
                PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                (codigo, politicas) = proxy.RecuperarPoliticasChecklist(1); //(folioCredito);
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
                    CargarPoliticasOtorgamiento(politicas);
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }
        
        private void CargarPoliticasOtorgamiento(Politica[] politicas)
        {
            if (politicas != null || politicas.Length > 0)
            {
                lstViewPoliticasOtorgamiento.ItemsSource = politicas;
            }
            else
            {
                MostrarVentanaPoliticasVacias();
            }
        }
        private void MostrarVentanaPoliticasVacias()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("", Mensaje.ERROR);
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

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {

        }

        private void ClicCancelar(object sender, RoutedEventArgs e)
        {

        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Politica politica = checkBox.CommandParameter as Politica;

        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
