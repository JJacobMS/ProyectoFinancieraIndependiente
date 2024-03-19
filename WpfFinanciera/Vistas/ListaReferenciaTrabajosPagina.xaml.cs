using System;
using System.Collections;
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
    /// Lógica de interacción para ListaReferenciaTrabajosPagina.xaml
    /// </summary>
    public partial class ListaReferenciaTrabajosPagina : Page
    {
        private FormularioClientePagina _formularioPagina;
        private List<ReferenciaTrabajo> _listaReferenciasTrabajo;
        public ListaReferenciaTrabajosPagina(FormularioClientePagina formularioPagina)
        {
            InitializeComponent();
            _formularioPagina = formularioPagina;
            CargarPaginaListaReferenciasTrabajo();
        }
        private void CargarPaginaListaReferenciasTrabajo()
        {
            Codigo codigo;
            try
            {
                ReferenciaTrabajo[] referencias;
                ReferenciaTrabajoClient proxy = new ReferenciaTrabajoClient();
                (referencias, codigo) = proxy.RecuperarReferenciasTrabajo();
                if (referencias != null)
                {
                    _listaReferenciasTrabajo = referencias.ToList();
                }
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

            VentanaMensaje ventanaMensaje;

            switch (codigo)
            {
                case Codigo.EXITO:
                    CargarListaReferenciasTrabajo(_listaReferenciasTrabajo);
                    break;
                case Codigo.ERROR_SERVIDOR:
                    ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
                    ventanaMensaje.Mostrar();
                    break;
                case Codigo.ERROR_BD:
                    ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
                    ventanaMensaje.Mostrar();
                    break;
            }

        }
        private void CargarListaReferenciasTrabajo(List<ReferenciaTrabajo> referenciasTrabajoRegistradas)
        {
            lstBoxReferenciasTrabajo.ItemsSource = referenciasTrabajoRegistradas;
        }
        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(_formularioPagina);
        }

        private void ClicAgregarNuevaReferencia(object sender, RoutedEventArgs e)
        {
            CargarFormularioReferenciaTrabajo();
        }
        
        private void CargarFormularioReferenciaTrabajo()
        {
            FormularioReferenciaTrabajoPagina formularioReferenciaTrabajoPagina = new FormularioReferenciaTrabajoPagina(this, _formularioPagina);
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(formularioReferenciaTrabajoPagina);
        }

        private void ClicBuscarReferenciaTrabajo(object sender, RoutedEventArgs e)
        {
            FiltrarReferenciasTrabajo(txtBoxBarraBuscar.Text);
        }

        private void FiltrarReferenciasTrabajo(string busqueda)
        {
            lstBoxReferenciasTrabajo.Style = (Style)FindResource("estiloLstBoxReferenciaTrabajo");
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                CargarListaReferenciasTrabajo(_listaReferenciasTrabajo);
            }
            else
            {
                List<ReferenciaTrabajo> referenciaTrabajos = _listaReferenciasTrabajo.Where(
                    referencia => referencia.nombre.Contains(busqueda) || referencia.direccion.Contains(busqueda) || referencia.telefono.Contains(busqueda))
                    .ToList();
                CargarListaReferenciasTrabajo(referenciaTrabajos);
            }
        }

        private void ClicAdjuntarReferenciaAlCliente(object sender, RoutedEventArgs e)
        {
            Button btnReferenciaTrabajo = sender as Button;
            ReferenciaTrabajo referenciaTrabajo = (ReferenciaTrabajo) btnReferenciaTrabajo.CommandParameter;
            _formularioPagina.AgregarReferenciaTrabajo(referenciaTrabajo);
            MainWindow ventana = (MainWindow)Window.GetWindow(this);
            ventana.CambiarPagina(_formularioPagina);
        }

    }
}
