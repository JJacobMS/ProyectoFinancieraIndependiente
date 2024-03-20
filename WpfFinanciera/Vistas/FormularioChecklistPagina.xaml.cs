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
    /// Lógica de interacción para FormularioChecklistPagina.xaml
    /// </summary>
    public partial class FormularioChecklistPagina : Page
    {
        public FormularioChecklistPagina()
        {
            InitializeComponent();
            AgregarMensaje("politica 1");
            AgregarMensaje("politica 2");
        }

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {

        }

        private void AgregarMensaje(string nombrePolitica)
        {
            Label politicaOtorgamiento = new Label();
            politicaOtorgamiento.Style = (Style)FindResource("estiloLabelChecklist");
            politicaOtorgamiento.Content = nombrePolitica;
            stcPanelPolitica.Children.Add(politicaOtorgamiento);
        }

        private void ClicAñadirPoliticas(object sender, RoutedEventArgs e)
        {

        }
    }
}
