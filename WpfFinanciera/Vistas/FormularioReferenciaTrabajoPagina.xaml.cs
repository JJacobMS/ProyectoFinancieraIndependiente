using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using static System.Net.Mime.MediaTypeNames;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioReferenciaTrabajoPagina.xaml
    /// </summary>
    public partial class FormularioReferenciaTrabajoPagina : Page
    {
        private ListaReferenciaTrabajosPagina _listaTrabajos;
        private FormularioClientePagina _formularioPagina;

        private string _rfcCliente;
        private ReferenciaTrabajo _referenciaTrabajo;

        public FormularioReferenciaTrabajoPagina(ListaReferenciaTrabajosPagina listaTrabajos, FormularioClientePagina formularioPagina)
        {
            InitializeComponent();
            _listaTrabajos = listaTrabajos;
            _formularioPagina = formularioPagina;
        }
        public FormularioReferenciaTrabajoPagina(ListaReferenciaTrabajosPagina listaTrabajos, FormularioClientePagina formularioPagina, string rfcCliente, ReferenciaTrabajo referenciaPrevia, bool esRegistro)
        {
            InitializeComponent();
            _listaTrabajos = listaTrabajos;
            _formularioPagina = formularioPagina;
            _rfcCliente = rfcCliente;
            _referenciaTrabajo = referenciaPrevia;
            if (esRegistro)
            {
                CargarFormularioReferenciaTrabajoRegistrar(referenciaPrevia);
            }
            else
            {
                CargarFormularioReferenciaTrabajoActualizar(referenciaPrevia);
            }

        }
        private void CargarFormularioReferenciaTrabajoActualizar(ReferenciaTrabajo referenciaTrabajo)
        {
            txtBoxNombre.IsEnabled = false;
            txtBoxNombre.Text = referenciaTrabajo.nombre;
            txtBoxDireccion.Text = referenciaTrabajo.direccion;
            txtBoxTelefono.Text = referenciaTrabajo.telefono;

            txtBlockGuardarReferencia.Text = "Guardar Cambios del Centro de Trabajo Actual";
            btnGuardarReferencia.Click -= ClicAdjuntarReferenciaTrabajoCliente;
            btnGuardarReferencia.Click += ClicGuardarCambiosReferenciaTrabajoActual;
        }

        private void ClicGuardarCambiosReferenciaTrabajoActual(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCampoVaciosYValidos();
            txtBoxNombre.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#073843");
            if (sonCamposValidos)
            {
                ActualizarReferenciaTrabajoActual();
            }
        }

        private void ActualizarReferenciaTrabajoActual()
        {
            ReferenciaTrabajo referenciaActualizada = new ReferenciaTrabajo
            {
                idReferenciaTrabajo = _referenciaTrabajo.idReferenciaTrabajo,
                nombre = _referenciaTrabajo.nombre,
                direccion = txtBoxDireccion.Text.Trim(),
                telefono = txtBoxTelefono.Text.Trim()
            };

            Codigo codigo;
            try
            {
                ReferenciaTrabajoClient proxy = new ReferenciaTrabajoClient();
                codigo = proxy.ActualizarInformacionReferenciaTrabajoActual(referenciaActualizada);
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    _referenciaTrabajo = referenciaActualizada;
                    MostrarVentanaExitoActualizacion();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }

        private void CargarFormularioReferenciaTrabajoRegistrar(ReferenciaTrabajo referenciaPrevia)
        {
            txtBlockGuardarReferencia.Text = "Guardar Referencia Trabajo Nueva";
            btnGuardarReferencia.Click -= ClicAdjuntarReferenciaTrabajoCliente;
            btnGuardarReferencia.Click += ClicGuardarReferenciaTrabajoNueva;
        }

        private void ClicGuardarReferenciaTrabajoNueva(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCampoVaciosYValidos();
            if (sonCamposValidos)
            {
                MostrarVentanaConfirmacionRegistroNuevo();
            }
        }

        private void MostrarVentanaConfirmacionRegistroNuevo()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea agregar la referencia de trabajo?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                ActualizarReferenciaTrabajo();
            }
        }

        private void ActualizarReferenciaTrabajo()
        {
            ReferenciaTrabajo referenciaNueva = new ReferenciaTrabajo
            {
                nombre = txtBoxNombre.Text.Trim(),
                direccion = txtBoxDireccion.Text.Trim(),
                telefono = txtBoxTelefono.Text.Trim()
            };

            Codigo codigo;
            int idReferenciaTrabajo = 0;
            try
            {
                ReferenciaTrabajoClient proxy = new ReferenciaTrabajoClient();
                (idReferenciaTrabajo, codigo) = proxy.ActualizarYCambiarReferenciaTrabajo(referenciaNueva, _rfcCliente);
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    _referenciaTrabajo = referenciaNueva;
                    _referenciaTrabajo.idReferenciaTrabajo = idReferenciaTrabajo;
                    MostrarVentanaExitoActualizacion();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }

        private void MostrarVentanaExitoActualizacion()
        {
            _formularioPagina.AgregarReferenciaTrabajo(_referenciaTrabajo);
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            ventanaPrincipal.CambiarPagina(_formularioPagina);

            VentanaMensaje ventana = new VentanaMensaje("Se ha actualizado la referencia de trabajo exitosamente", Mensaje.EXITO);
            ventana.Mostrar();
        }

        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
        }

        private void ClickRegresar(object sender, RoutedEventArgs e)
        {
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(_listaTrabajos);
        }

        private void ClicAdjuntarReferenciaTrabajoCliente(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCampoVaciosYValidos();
            if (sonCamposValidos)
            {
                MostrarVentanaConfirmacion();
            }
        }

        private bool ValidarCampoVaciosYValidos()
        {
            ResetearCampos();
            bool sonCamposValidos = true;
            string razones = "";
            string nombre = txtBoxNombre.Text.Trim();
            string direccion = txtBoxDireccion.Text.Trim();
            string telefono = txtBoxTelefono.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 100)
            {
                txtBoxNombre.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones += "Nombre (debe ser menor a 101 caracteres)";
            }

            if (string.IsNullOrWhiteSpace(direccion) || direccion.Length > 50)
            {
                txtBoxDireccion.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0 ) ? razones + ",  " : razones;
                razones += "Dirección (debe ser menor a 51 caracteres)";
            }

            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length > 12)
            {
                txtBoxTelefono.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : "";
                razones += "Teléfono (debe ser menor a 13 caracteres)";
            }

            if (!sonCamposValidos)
            {
                MostrarVentanaCamposNoValidos(razones);
            }

            return sonCamposValidos;
        }

        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea agregar la referencia de trabajo?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                EnviarReferenciaTrabajoFormulario();
            }
        }
        private void EnviarReferenciaTrabajoFormulario()
        {
            ReferenciaTrabajo referencia = new ReferenciaTrabajo { idReferenciaTrabajo = 0, nombre = txtBoxNombre.Text.Trim(), direccion = txtBoxDireccion.Text.Trim(), telefono = txtBoxTelefono.Text.Trim() };
            _formularioPagina.AgregarReferenciaTrabajo(referencia);
            MainWindow ventana = (MainWindow) Window.GetWindow(this);
            ventana.CambiarPagina(_formularioPagina);

        }
        private void ResetearCampos()
        {
            txtBoxNombre.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxDireccion.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxTelefono.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
        }

        private void MostrarVentanaCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
        }

        private void PreviewKeyDownValidarNumero(object sender, KeyEventArgs e)
        {
            if (!char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) && e.Key != Key.Delete && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }
    }
}
