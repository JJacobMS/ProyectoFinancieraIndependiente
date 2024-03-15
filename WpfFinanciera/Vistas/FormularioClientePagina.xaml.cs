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
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioClientePagina.xaml
    /// </summary>
    public partial class FormularioClientePagina : Page
    {
        private Dictionary<TipoArchivo, Button> botonesTipoArchivo = new Dictionary<TipoArchivo, Button>();

        public FormularioClientePagina()
        {
            InitializeComponent();
            botonesTipoArchivo.Add(TipoArchivo.IdentificacionOficial, btnAgregarArchivoIdentificacion);
            botonesTipoArchivo.Add(TipoArchivo.ComprobanteDomicilio, btnAgregarComprobanteDomicilio);
            botonesTipoArchivo.Add(TipoArchivo.ComprobanteIngreso, btnAgregarComprobanteIngreso);
            botonesTipoArchivo.Add(TipoArchivo.ComprobanteTrabajo, btnAgregarComprobanteTrabajo);
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

            Button btnDocumento = botonesTipoArchivo[tipoArchivo];

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

            Button btnDocumento = botonesTipoArchivo[tipoArchivo];
            btnDocumento.Style = (Style)FindResource("estiloBtnAgregarArchivo");
            btnDocumento.Click += ClicAgregarArchivo;

        }

        private void ClicSeleccionarReferenciaTrabajo(object sender, RoutedEventArgs e)
        {
            SeleccionarReferenciaTrabajo();
        }

        private void SeleccionarReferenciaTrabajo()
        {
            ListaReferenciaTrabajosPagina listaRTpagina = new ListaReferenciaTrabajosPagina(this);
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(listaRTpagina);
        }
    }

    public enum TipoArchivo
    {
        IdentificacionOficial, ComprobanteIngreso, ComprobanteTrabajo, ComprobanteDomicilio
    }
}


