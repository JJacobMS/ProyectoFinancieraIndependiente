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
                new CondicionCredito("Inversión",60,2, true),
                new CondicionCredito("Vivienda",100,2, false),
                new CondicionCredito("Préstamo 50,000",30,5, true),
                new CondicionCredito("Carro",180,2, false),
                new CondicionCredito("Mobiliario",10,2, true)
            };
        }

        private void ClicSolicitarCredito(object sender, RoutedEventArgs e)
        {

        }
    }

    class CondicionCredito
    {
        public string Descripcion { get; set; }
        public int Plazo { get; set; }
        public double TasaInteres { get; set; }
        public bool TieneIVA { get; set; }

        public CondicionCredito(string descripcion, int plazo, double tasaInteres, bool tieneIVA)
        {
            Descripcion = descripcion;
            Plazo = plazo;
            TasaInteres = tasaInteres;
            TieneIVA = tieneIVA;
        }
    }
}
