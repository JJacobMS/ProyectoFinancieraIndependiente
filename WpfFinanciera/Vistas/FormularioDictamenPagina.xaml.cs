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
    /// Lógica de interacción para FormularioDictamenPagina.xaml
    /// </summary>
    public partial class FormularioDictamenPagina : Page
    {
        public FormularioDictamenPagina()
        {
            InitializeComponent();
        }

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {

        }

        private void ClicCancelar(object sender, RoutedEventArgs e)
        {

        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Politica politica = checkBox.CommandParameter as Politica;

        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
