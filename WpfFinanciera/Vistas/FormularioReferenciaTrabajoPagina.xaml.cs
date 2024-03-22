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
        public FormularioReferenciaTrabajoPagina(ListaReferenciaTrabajosPagina listaTrabajos, FormularioClientePagina formularioPagina)
        {
            InitializeComponent();
            _listaTrabajos = listaTrabajos;
            _formularioPagina = formularioPagina;
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
