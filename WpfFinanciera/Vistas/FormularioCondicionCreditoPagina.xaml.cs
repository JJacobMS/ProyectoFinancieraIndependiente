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
        private CondicionCredito _condicionCreditoActual;
        private bool _esConsulta = false;

        public FormularioCondicionCreditoPagina()
        {
            InitializeComponent();
        }

        public FormularioCondicionCreditoPagina(CondicionCredito condicionCredito)
        {
            InitializeComponent();

            txtBoxIdentificador.Text = condicionCredito.identificador;
            txtBoxIdentificador.IsEnabled = false;
            txtBoxDescripcion.Text = condicionCredito.descripcion;
            txtBoxDescripcion.IsEnabled = false;
            txtBoxPlazo.Text = condicionCredito.plazoMeses.ToString();
            txtBoxPlazo.IsEnabled = false;
            txtBoxInteres.Text = condicionCredito.tasaInteres.ToString();
            txtBoxInteres.IsEnabled = false;
            chkBoxIVA.Visibility = Visibility.Collapsed;
            txtBlockIva.Visibility = Visibility.Visible;
            txtBlockIva.Text = condicionCredito.tieneIVA ? "Sí" : "No";
            txtBlockEstadoCampo.Visibility = Visibility.Visible;
            txtBlockEstadoInfo.Visibility = Visibility.Visible;
            txtBlockEstadoInfo.Text = condicionCredito.estaActiva ? "Activa" : "Inactiva";
            btnRegistrar.Visibility = Visibility.Collapsed;
            btnModificar.Visibility = Visibility.Visible;

            _condicionCreditoActual = condicionCredito;
            _esConsulta = true;
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
            bool camposValidos = ValidarCampos(identificador, descripcion, plazo, tasa);

            if (camposValidos)
            {
                MostrarVentanaConfirmacion();
            }
        }

        private bool ValidarCampos(string identificador, string descripcion, string plazo, string tasa)
        {
            ReestablecerEstiloCampos();
            bool sonValidos = true;
            string razones = "";

            if (string.IsNullOrEmpty(identificador))
            {
                txtBoxIdentificador.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EACBCB");
                sonValidos = false;
                razones += "Identificador";
            }

            if (string.IsNullOrEmpty(descripcion))
            {
                txtBoxDescripcion.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EACBCB");
                sonValidos = false;
                razones += (razones != "") ? ", Descripción" : "Descripción";
            }

            if (string.IsNullOrEmpty(plazo))
            {
                txtBoxPlazo.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EACBCB");
                sonValidos = false;
                razones += (razones != "") ? ", Plazo" : "Plazo";
            }
            else if (double.Parse(plazo) <= _PLAZO_MINIMO || double.Parse(plazo) > _PLAZO_MAXIMO)
            {
                txtBoxPlazo.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EACBCB");
                sonValidos = false;
                razones += (razones != "") ? ", Plazo no válido" : "Plazo no válido";
            }

            if (string.IsNullOrEmpty(tasa))
            {
                txtBoxInteres.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EACBCB");
                sonValidos = false;
                razones += (razones != "") ? ", Tasa de Interés" : "Tasa de Interés";
            }

            if (!sonValidos)
            {
                MostrarVentanaCamposNoValidos(razones);
            }

            return sonValidos;
        }

        private void ReestablecerEstiloCampos()
        {
            if (!_esConsulta)
            {
                txtBoxIdentificador.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#A9D4D6");
                txtBoxIdentificador.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#A9D4D6");
            }
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
            string mensaje = _esConsulta ? "¿Está seguro que desea actualizar la condición de crédito?" : "¿Está seguro que desea registrar la condición de crédito?";
            VentanaMensaje ventana = new VentanaMensaje(mensaje, Mensaje.CONFIRMACION);
            bool respuesta = ventana.MostrarConfirmacion();

            if (respuesta && !_esConsulta)
            {
                GuardarCondicionCredito();
                LimpiarCampos();
            } else if(respuesta && _esConsulta)
            {
                ValidarCondicionNoActiva();
            }
        }

        private void ValidarCondicionNoActiva()
        {
            Codigo codigo;
            bool esActiva = false;

            try
            {
                CondicionCreditoClient condicionCreditoClient = new CondicionCreditoClient();
                var respuesta = condicionCreditoClient.esActivaCondicionCredito(_condicionCreditoActual.idCondicionCredito);
                var (codigoRespuesta, esActivaRespuesta) = respuesta;

                codigo = codigoRespuesta;
                esActiva = esActivaRespuesta;
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
                    if (esActiva)
                    {
                        MostrarVentanaError();
                    }
                    else
                    {
                        ActualizarCondicionCredito();
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

        private void MostrarVentanaError()
        {
            VentanaMensaje error = new VentanaMensaje("No se pudo actualizar la condición de crédito. Esta condición ya se encuentra activa en un crédito", Mensaje.ERROR);
            error.Mostrar();
        }

        private void ActualizarCondicionCredito()
        {
            Codigo codigo;

            _condicionCreditoActual.descripcion = txtBoxDescripcion.Text.Trim();
            _condicionCreditoActual.plazoMeses = int.Parse(txtBoxPlazo.Text.Trim());
            _condicionCreditoActual.tasaInteres = (int) double.Parse(txtBoxInteres.Text.Trim());
            _condicionCreditoActual.tieneIVA = (bool)chkBoxIVA.IsChecked;
            _condicionCreditoActual.estaActiva = (bool)chkBoxActiva.IsChecked;

            try
            {
                CondicionCreditoClient condicionCreditoClient = new CondicionCreditoClient();
                codigo = condicionCreditoClient.ActualizarCondicionCredito(_condicionCreditoActual);
            }
            catch (CommunicationException)
            {
                codigo = Codigo.ERROR_SERVIDOR;
            }
            catch (TimeoutException)
            {
                codigo = Codigo.ERROR_SERVIDOR;
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    MostrarVentanaActualizacionExito();
                    ClicRegresar(null, null);
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBD();
                    break;
            }
        }

        private void MostrarVentanaActualizacionExito()
        {
            VentanaMensaje exito = new VentanaMensaje("Se ha actualizado la condicion de credito exitosamente", Mensaje.EXITO);
            exito.Mostrar();
        }

        private void GuardarCondicionCredito()
        {
            Codigo codigo;

            CondicionCredito condicionCredito = new CondicionCredito();
            condicionCredito.identificador = txtBoxIdentificador.Text.Trim();
            condicionCredito.descripcion = txtBoxDescripcion.Text.Trim();
            condicionCredito.plazoMeses = int.Parse(txtBoxPlazo.Text.Trim());
            condicionCredito.tasaInteres = (int) double.Parse(txtBoxInteres.Text.Trim());
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
            if (!_esConsulta)
            {
                MenuPrincipalAdministradorPagina menuPrincipal = new MenuPrincipalAdministradorPagina();
                NavigationService.Navigate(menuPrincipal);
            }
            else
            {
                CondicionesCreditoPagina condicionesCredito = new CondicionesCreditoPagina();
                NavigationService.Navigate(condicionesCredito);
            }
        }

        private void ClicModificar(object sender, RoutedEventArgs e)
        {
            txtBoxDescripcion.IsEnabled = true;
            txtBoxInteres.IsEnabled = true;
            txtBoxPlazo.IsEnabled = true;
            chkBoxIVA.Visibility = Visibility.Visible;
            txtBlockIva.Visibility = Visibility.Collapsed;
            txtBlockEstadoCampo.Text = "Activa";
            chkBoxActiva.IsChecked = _condicionCreditoActual.estaActiva;
            txtBlockEstadoInfo.Visibility = Visibility.Collapsed;
            chkBoxActiva.Visibility = Visibility.Visible;
            chkBoxActiva.Visibility = Visibility.Visible;
            btnModificar.Visibility = Visibility.Collapsed;
            btnActualizar.Visibility = Visibility.Visible;
        }

        private void ClicActualizar(object sender, RoutedEventArgs e)
        {
            string descripcion = txtBoxDescripcion.Text.Trim();
            string plazo = txtBoxPlazo.Text.Trim();
            string tasa = txtBoxInteres.Text.Trim();
            bool tieneIVA = (bool)chkBoxIVA.IsChecked;
            bool estaActiva = (bool)chkBoxActiva.IsChecked;

            ActualizarCondicionCredito(descripcion, plazo, tasa, tieneIVA, estaActiva);
        }

        private void ActualizarCondicionCredito(string descripcion, string plazo, string tasa, bool tieneIVA, bool estaActiva)
        {
            bool camposValidos = ValidarCampos(_condicionCreditoActual.identificador,descripcion,plazo,tasa);

            if (camposValidos)
            {
                MostrarVentanaConfirmacion();
            }
        }
    }
}
