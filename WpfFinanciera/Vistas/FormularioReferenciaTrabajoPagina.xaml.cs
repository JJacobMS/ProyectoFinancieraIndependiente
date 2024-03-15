using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Net.Mime.MediaTypeNames;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioReferenciaTrabajoPagina.xaml
    /// </summary>
    public partial class FormularioReferenciaTrabajoPagina : Page
    {
        private ListaReferenciaTrabajosPagina _listaTrabajos;
        private FormularioClientePagina _formularioPagina;
        public FormularioReferenciaTrabajoPagina(ListaReferenciaTrabajosPagina listaTrabajos, FormularioClientePagina formularioPagina)
        {
            InitializeComponent();
            _listaTrabajos = listaTrabajos;
            _formularioPagina = formularioPagina;
        }

        private void ClickRegresar(object sender, RoutedEventArgs e)
        {
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(_listaTrabajos);
        }

        private void ClicAdjuntarReferenciaTrabajoCliente(object sender, RoutedEventArgs e)
        {

        }

        private void ValidarCampoVaciosYValidos()
        {

        }

        private void PreviewKeyDownValidarNumero(object sender, KeyEventArgs e)
        {
            if (!char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)))
            {
                e.Handled = true;
            }
        }
    }
}
