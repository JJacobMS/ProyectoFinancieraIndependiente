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
    /// Lógica de interacción para FormularioCondicionCreditoPagina.xaml
    /// </summary>
    public partial class FormularioCondicionCreditoPagina : Page
    {
        private static readonly int _PLAZO_MINIMO = 1;
        private static readonly int _PLAZO_MAXIMO = 240;

        public FormularioCondicionCreditoPagina()
        {
            InitializeComponent();
        }

        private void PrevenirTextoNoNumerico(object sender, TextCompositionEventArgs e)
        {
            if (!EsNumerico(e.Text))
            {
                e.Handled = true; 
            }
        }

        private bool EsNumerico(string texto)
        {
            return int.TryParse(texto, out _);
        }

        private void ClicRegistrarCondicionCredito(object sender, RoutedEventArgs e)
        {
            string identificador = txtBoxIdentificador.Text.Trim();
            string descripcion = txtBoxDescripcion.Text.Trim();
            string plazo = txtBoxPlazo.Text.Trim();
            string tasa = txtBoxInteres.Text.Trim();
            bool tieneIVA = (bool) chkBoxIVA.IsChecked;

            RegistrarCondicionCredito(identificador, descripcion, plazo, tasa, tieneIVA);
        }

        private void RegistrarCondicionCredito(string identificador, string descripcion, string plazo, string tasa, bool tieneIVA) 
        {
            bool camposValidos = ValidarCampos(identificador, descripcion, plazo, tasa, tieneIVA);

            if (camposValidos)
            {
                MostrarVentanaConfirmacion();
            }
        }

        private bool ValidarCampos(string identificador, string descripcion, string plazo, string tasa, bool tieneIVA)
        {
            ReestablecerEstiloCampos();
            bool sonValidos = true;
            string razones = "";

            if (string.IsNullOrEmpty(identificador)) 
            {
                txtBoxIdentificador.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#C46960");
                sonValidos = false;
                razones += "Identificador";
            }
            if (string.IsNullOrEmpty(descripcion))
            {
                txtBoxDescripcion.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#C46960");
                sonValidos = false;
                razones = (razones.Length > 0) ? razones + ", Descripción" : "Descripción";
            }
            if (string.IsNullOrEmpty(plazo))
            {
                txtBoxPlazo.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#C46960");
                sonValidos = false;
                razones = (razones.Length < 0) ? razones + ", Plazo" : "Plazo";
            }
            else
            {
                if(double.Parse(plazo) <= _PLAZO_MINIMO || double.Parse(plazo) > _PLAZO_MAXIMO)
                {
                    txtBoxPlazo.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                    sonValidos = false;
                    razones = (razones.Length < 0) ? razones + ", Plazo no válido" : "Plazo no válido";
                }
            }

            if (string.IsNullOrEmpty(tasa))
            {
                txtBoxInteres.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#C46960");
                sonValidos = false;
                razones = (razones.Length < 0) ? razones + ", Tasa de Interés" : "Tasa de Interés";
            }

            if (!sonValidos)
            {
                MostrarVentanaCamposNoValidos(razones);
            }

            return sonValidos;
        }

        private void ReestablecerEstiloCampos()
        {
            txtBoxIdentificador.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#A9D4D6");
            txtBoxDescripcion.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#A9D4D6");
            txtBoxInteres.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#A9D4D6");
            txtBoxPlazo.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#A9D4D6");
        }

        private void MostrarVentanaCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
        }
        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea registrar la condición de crédito?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                GuardarCondicionCredito();
                LimpiarCampos();
            }
        }

        private void GuardarCondicionCredito()
        {
            Codigo codigo;

            CondicionCredito condicionCredito = new CondicionCredito();
            condicionCredito.identificador = txtBoxIdentificador.Text.Trim();
            condicionCredito.descripcion = txtBoxDescripcion.Text.Trim();
            condicionCredito.plazoMeses = int.Parse(txtBoxPlazo.Text.Trim());
            condicionCredito.tieneIVA = (bool)chkBoxIVA.IsChecked;
            condicionCredito.estaActiva = true;

            try
            {
                CondicionCreditoClient condicionCreditoClient = new CondicionCreditoClient();
                codigo = condicionCreditoClient.GuardarCondicionCredito(condicionCredito);
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

        private void MostrarVentanaRegistroExito()
        {
            VentanaMensaje exito = new VentanaMensaje(
                "Se ha registrado la condicion de credito exitosamente", Mensaje.EXITO);
            exito.Mostrar();
        }

        private void LimpiarCampos()
        {
            txtBoxIdentificador.Text = "";
            txtBoxDescripcion.Text = "";
            txtBoxInteres.Text = "";
            txtBoxPlazo.Text = "";

            chkBoxIVA.IsChecked = false;
        }

        private void ClicRegresar(object sender, MouseButtonEventArgs e)
        {
            MenuPrincipalAnalistaCreditoPagina menuPrincipal = new MenuPrincipalAnalistaCreditoPagina();

            NavigationService.Navigate(menuPrincipal);
        }
    }
}
