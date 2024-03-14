using System;
using System.Collections;
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
            //Client proxy = new Client();
            //proxy.RecuperarReferenciasTrabajo();
            _listaReferenciasTrabajo = new List<ReferenciaTrabajo>
            {
                new ReferenciaTrabajo{ Nombre = "Lores undozgbr pag bfobspfon Lores undozgbr pag bfobspfon", Direccion = "Lores undozgbr pag bfobspfon Lores undozgbr pag bfobspfon", Telefono = "2281367802"},
                new ReferenciaTrabajo{ Nombre = "Lores BNUDIZB pag bfobspfon Lores undozgbr pag bfobspfon", Direccion = "VNSO undozgbr pag bfobspfon Lores undozgbr pag bfobspfon", Telefono = "2234567802"}

            };

            //swith(codigo)
            CargarListaReferenciasTrabajo(_listaReferenciasTrabajo);

        }
        private void CargarListaReferenciasTrabajo(List<ReferenciaTrabajo> referenciasTrabajos)
        {
            lstBoxReferenciasTrabajo.ItemsSource = referenciasTrabajos;
        }
        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(_formularioPagina);
        }
    }

    public class ReferenciaTrabajo
    {
        public int IdReferenciaTrabajo { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        public ReferenciaTrabajo()
        {
            
        }
    }
}
