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
using System.Windows.Shapes;

namespace WpfFinanciera.Utilidades
{
    /// <summary>
    /// Lógica de interacción para VentanaMensaje.xaml
    /// </summary>
    public partial class VentanaMensaje : Window
    {
        public VentanaMensaje(string mensaje, Mensaje tipoMensaje)
        {
            InitializeComponent();
            btnBackground.DataContext = mensaje;
            switch (tipoMensaje)
            {
                case Mensaje.ERROR:
                    break;
                case Mensaje.CONFIRMACION:
                    grdBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#446CFF");
                    btnBackground.Style = (Style)FindResource("estiloBtnVentanaMensajeCONFIRMACION");
                    break;
                case Mensaje.EXITO:
                    grdBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#052E2F");
                    btnBackground.Style = (Style)FindResource("estiloBtnVentanaMensajeEXITO");
                    break;
            }
        }

        public VentanaMensaje(string titulo, string razones)
        {
            InitializeComponent();
            btnBackground.Style = (Style)FindResource("estiloBtnVentanaMensajeERRORRAZONES");
            var contexto = new
            {
                titulo, razones
            };
            btnBackground.DataContext = contexto;
        }

        public void Mostrar()
        {
            Show();
        }

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClicCancelar(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClicConfirmar(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public enum Mensaje
    {
        ERROR, CONFIRMACION, EXITO
    }
}
