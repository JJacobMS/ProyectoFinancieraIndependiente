using Microsoft.Win32;
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
    /// Lógica de interacción para FormularioClientePagina.xaml
    /// </summary>
    public partial class FormularioClientePagina : Page
    {
        private Dictionary<TipoArchivo, Button> _botonesTipoArchivo = new Dictionary<TipoArchivo, Button>();
        private ReferenciaTrabajo _referenciaTrabajo;
        private Dictionary<string, Documento> _documentosCliente = new Dictionary<string, Documento>();
        private Dictionary<string, ReferenciaCliente> _referenciasCliente = new Dictionary<string, ReferenciaCliente>();
        public FormularioClientePagina()
        {
            InitializeComponent();
            _referenciaTrabajo = null;

            _referenciasCliente.Add("1", null);
            _referenciasCliente.Add("2", null);

            _documentosCliente.Add("Identificación Oficial", null);
            _documentosCliente.Add("Comprobante Domicilio", null);
            _documentosCliente.Add("Comprobante Ingreso", null);
            _documentosCliente.Add("Comprobante Trabajo", null);
            _documentosCliente.Add("Referencia Cliente 1", null);
            _documentosCliente.Add("Referencia Cliente 2", null);

            _botonesTipoArchivo.Add(TipoArchivo.IdentificacionOficial, btnAgregarArchivoIdentificacion);
            _botonesTipoArchivo.Add(TipoArchivo.ComprobanteDomicilio, btnAgregarComprobanteDomicilio);
            _botonesTipoArchivo.Add(TipoArchivo.ComprobanteIngreso, btnAgregarComprobanteIngreso);
            _botonesTipoArchivo.Add(TipoArchivo.ComprobanteTrabajo, btnAgregarComprobanteTrabajo);
        }

        private void ClicAgregarArchivo(object sender, RoutedEventArgs e)
        {
            Button btnAgregarArchivo = sender as Button;
            string tipoCadena = btnAgregarArchivo.CommandParameter.ToString();
            TipoArchivo tipoArchivo = (TipoArchivo)Enum.Parse(typeof(TipoArchivo), tipoCadena);
            AgregarArchivo(tipoArchivo);
        }

        private void AgregarArchivo(TipoArchivo tipoArchivo)
        {
            OpenFileDialog ventanaArchivo = new OpenFileDialog();
            ventanaArchivo.Title = "Seleccione el archivo con la identificación oficial";
            ventanaArchivo.CheckFileExists = true;
            ventanaArchivo.CheckPathExists = true;
            ventanaArchivo.Filter = "Solo archivos PDF (*.pdf)|*.pdf";
            bool respuesta = (bool)ventanaArchivo.ShowDialog();

            Button btnDocumento = _botonesTipoArchivo[tipoArchivo];

            if (respuesta)
            {
                string nombreArchivo = ventanaArchivo.SafeFileName;
                btnDocumento.Style = (Style)FindResource("estiloBtnArchivoAdjuntado");
                btnDocumento.Click -= ClicAgregarArchivo;
                btnDocumento.DataContext = nombreArchivo;
                btnDocumento.CommandParameter = tipoArchivo;
            }
        }

        private void ClicEliminarDocumento(object sender, RoutedEventArgs e)
        {
            Button btnEliminar = sender as Button;
            string tipoCadena = btnEliminar.CommandParameter.ToString();
            TipoArchivo tipoArchivo = (TipoArchivo)Enum.Parse(typeof(TipoArchivo), tipoCadena);
            string nombreArchivo = btnEliminar.DataContext.ToString();

            Button btnDocumento = _botonesTipoArchivo[tipoArchivo];
            btnDocumento.Style = (Style)FindResource("estiloBtnAgregarArchivo");
            btnDocumento.Click += ClicAgregarArchivo;

        }

        private void ClicSeleccionarReferenciaTrabajo(object sender, RoutedEventArgs e)
        {
            ListaReferenciaTrabajosPagina listaRTpagina = new ListaReferenciaTrabajosPagina(this);
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(listaRTpagina);
        }

        public void AgregarReferenciaTrabajo(ReferenciaTrabajo referenciaTrabajo)
        {
            _referenciaTrabajo = referenciaTrabajo;
            txtBoxNombreReferenciaTrabajo.Text = referenciaTrabajo.nombre;
            txtBoxDireccionReferenciaTrabajo.Text = referenciaTrabajo.direccion;
            txtBoxTelefonoReferenciaTrabajo.Text = referenciaTrabajo.telefono;
        }

        private void ClicRegistrarReferenciaCliente(object sender, RoutedEventArgs e)
        {
            Button btnReferenciaCliente = sender as Button;
            FormularioReferenciaClientePagina formularioReferenciaCliente = new FormularioReferenciaClientePagina(this, btnReferenciaCliente.CommandParameter.ToString());
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(formularioReferenciaCliente);
        }

        public void AgregarReferenciaCliente(ReferenciaCliente referenciaCliente, Documento documentoReferencia, string numeroReferencia)
        {
            _referenciasCliente[numeroReferencia] = referenciaCliente;
            _documentosCliente[documentoReferencia.TipoDocumento.descripcion] = documentoReferencia;
            
            switch (numeroReferencia)
            {
                case "1":
                    txtBoxNombreReferenciaCliente1.Text = referenciaCliente.nombres + " " + referenciaCliente.apellidos;
                    txtBoxDescripcionReferenciaCliente1.Text = referenciaCliente.descripcion;
                    txtBoxTelefonoReferenciaCliente1.Text = referenciaCliente.telefono;
                    break;
                case "2":
                    txtBoxNombreReferenciaCliente2.Text = referenciaCliente.nombres + " " + referenciaCliente.apellidos;
                    txtBoxDescripcionReferenciaCliente2.Text = referenciaCliente.descripcion;
                    txtBoxTelefonoReferenciaCliente2.Text = referenciaCliente.telefono;
                    break;
            }
        }
    }

    public enum TipoArchivo
    {
        IdentificacionOficial, ComprobanteIngreso, ComprobanteTrabajo, ComprobanteDomicilio
    }
}


