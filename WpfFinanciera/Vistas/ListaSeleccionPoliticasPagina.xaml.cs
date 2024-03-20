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
    /// Lógica de interacción para ListaSeleccionPoliticasPagina.xaml
    /// </summary>
    public partial class ListaSeleccionPoliticasPagina : Page
    {
        private List<int> listIdPoliticas_ = new List<int>();
        public ListaSeleccionPoliticasPagina()
        {
            InitializeComponent();
        }

        private void ClicAceptarCambios(object sender, RoutedEventArgs e)
        {

        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Politica politica = checkBox.CommandParameter as Politica;
            listIdPoliticas_.Add(politica.idPolitica);
        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Politica politica = checkBox.CommandParameter as Politica;
            listIdPoliticas_.Remove(politica.idPolitica);
        }
    }
}
