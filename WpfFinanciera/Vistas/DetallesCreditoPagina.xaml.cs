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
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para DetallesCreditoPagina.xaml
    /// </summary>
    public partial class DetallesCreditoPagina : Page
    {
        private Credito credito;
        public DetallesCreditoPagina(Credito credito)
        {
            InitializeComponent();
            CargarDetallesCredito(credito);
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {

        }

        private void CargarDetallesCredito(Credito credito)
        {
            this.credito = credito;
            RecuperarDetallesCredito();
            CargarDetallesCredito();
            if (this.credito.Dictamen == null)
            {
                DeshabilitarBotones();
            }
        }

        private void RecuperarDetallesCredito()
        {
            EstatusCredito estatusA = new EstatusCredito { nombre = "Aprobado" };
            Usuario us = new Usuario
            {
                nombres = "Jazmin", apellidos = "Matinez Aguilar"
            };
            Dictamen dictamen = new Dictamen
            {
                estaAprobado = true, fechaHora = DateTime.Now, Usuario = us,
                observaciones = "dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs sf e"
            };


            Cliente cl = new Cliente
            {
                rfc = "QU27805274802", nombres = "Sulem", apellidos = "Martinez Aguilar"

            };

            CondicionCredito con = new CondicionCredito
            {
                estaActiva = true, identificador = "CC-01", descripcion = "dvsvs vsfv sf e", plazoMeses = 10, tasaInteres = 10,
                tieneIVA = true
            };

            Credito creditoNuevo = new Credito
            {
                folioCredito = 163913951, fechaSolicitud = DateTime.Now, EstatusCredito = estatusA, monto = 200020, saldoPendiente = 9892,
                deudaExtra = 68, Cliente = cl, CondicionCredito = con, Dictamen = new Dictamen[1]
            };

            if (creditoNuevo.Dictamen != null)
            {
                creditoNuevo.Dictamen[0] = dictamen;
            }

            credito = creditoNuevo;
        }

        private void CargarDetallesCredito()
        {
            txtBlockRfc.Text = credito.Cliente.rfc;
            txtBlockNombre.Text = credito.Cliente.nombres + " " + credito.Cliente.apellidos;

            txtBlockEstadoCondicionCredito.Text = (credito.CondicionCredito.estaActiva) ? "Activa" : "No activa";
            llpCondicionCreditoEstado.Fill = credito.CondicionCredito.estaActiva ?
                (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F") :
                (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24");

            txtBlockIdentificadorCondicion.Text = credito.CondicionCredito.identificador;
            txtBlockDescripcionCondicion.Text = credito.CondicionCredito.descripcion;
            txtBlockPlazoMesesCondicion.Text = credito.CondicionCredito.plazoMeses.ToString();
            rnTasaInteres.Text = credito.CondicionCredito.tasaInteres.ToString();
            txtBlockAplicaIva.Text = (credito.CondicionCredito.tieneIVA) ? "Aplica" : "No aplica";
            llpAplicaIva.Fill = credito.CondicionCredito.tieneIVA ?
                (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F") :
                (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24");

            txtBlockFolio.Text = credito.folioCredito.ToString();
            txtBlockFecha.Text = credito.fechaSolicitud.ToString("dd/MM/yyyy HH:mm");
            txtBlockEstatusCodigo.Text = credito.EstatusCredito.nombre;
            rnMonto.Text = credito.monto.ToString("#,##0");
            rnSaldoPendiente.Text = credito.saldoPendiente.ToString("#,##0"); ;
            rnDeudaExtra.Text = credito.deudaExtra.ToString("#,##0");

            if (credito.Dictamen != null)
            {
                txtBlockEstadoDictamen.Text = (credito.Dictamen[0].estaAprobado) ? "Activo" : "No activo";
                llpEstadoDictamen.Fill = credito.Dictamen[0].estaAprobado ?
                    (SolidColorBrush)new BrushConverter().ConvertFromString("#0DB07F") :
                    (SolidColorBrush)new BrushConverter().ConvertFromString("#9B2E24");
                txtBlockFechaDictamen.Text = credito.Dictamen[0].fechaHora.ToString("dd/MM/yyyy HH:mm");
                txtBlockObservacionesDictamen.Text = credito.Dictamen[0].observaciones.ToString();
                txtBlockUsuarioDictamen.Text = credito.Dictamen[0].Usuario.nombres + " " + credito.Dictamen[0].Usuario.apellidos;
            }
            else
            {
                llpEstadoDictamen.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#939393");
            }
        }

        private void DeshabilitarBotones()
        {
            btnConsultarTablaPagos.IsEnabled = false;
            btnGenerarDocumento.IsEnabled = false;
        }

        private void ClicGenerarDocumento(object sender, RoutedEventArgs e)
        {
            GenerarDocumento();
        } 

        private void GenerarDocumento()
        {
            GeneracionDictamen.GenerarDictamen();
        }
    }
}
