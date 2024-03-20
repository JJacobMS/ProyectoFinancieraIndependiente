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

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioSolicitudCreditoPagina.xaml
    /// </summary>
    public partial class FormularioSolicitudCreditoPagina : Page
    {
        public FormularioSolicitudCreditoPagina()
        {
            InitializeComponent();

        }

        private void CargarPagina(object sender, RoutedEventArgs e)
        {
            lstBoxCondicionesCredito.ItemsSource = new List<CondicionCredito>
            {
            };
        }

        private void ClicSolicitarCredito(object sender, RoutedEventArgs e)
        {

        }
    }
}
