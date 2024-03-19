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
    /// Lógica de interacción para FormularioCondicionCreditoPagina.xaml
    /// </summary>
    public partial class FormularioCondicionCreditoPagina : Page
    {
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

        }
    }
}
