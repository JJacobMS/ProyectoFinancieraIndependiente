﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
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
    /// Lógica de interacción para FormularioDictamenPagina.xaml
    /// </summary>
    public partial class FormularioDictamenPagina : Page
    {
        private int folio_;
        private int idUsuario_;
        private String nombreSolicitante_;
        private List<int> listIdPoliticas_ = new List<int>();
        private String nombreChecklist_;
        private Politica[] politicas_;
        bool esAprobado_;
        public FormularioDictamenPagina()
        {
            folio_ = 1;
            idUsuario_ = 1;
            nombreSolicitante_ = "Jesus Jacob";
            InitializeComponent();
            RecuperarDatosPagina();
        }

        
        private void RecuperarDatosPagina()
        {
            Codigo codigo = new Codigo();
            politicas_ = new Politica[]{};

            try
            {
                PoliticaOtorgamientoClient proxy = new PoliticaOtorgamientoClient();
                (codigo, politicas_) = proxy.RecuperarPoliticasChecklist(1);
                ChecklistClient proxyChecklist = new ChecklistClient();
                (codigo, nombreChecklist_) = proxyChecklist.RecuperarChecklist(1);
                Console.WriteLine(nombreChecklist_ + " ");
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            switch (codigo)
            {
                case Codigo.EXITO:
                    CargarDatosPagina(politicas_);
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }
        
        private void CargarDatosPagina(Politica[] politicas)
        {
            if (politicas!=null && politicas.Length > 0)
            {
                lstViewPoliticasOtorgamiento.ItemsSource = politicas;
                txtBlockSolicitante.Text = "Solicitante: " + nombreSolicitante_;
                txtBlockFolio.Text = "Folio de Solicitud: " + folio_;
                txtBlockChecklist.Text = "Checklist: "+ nombreChecklist_;
            }
            else
            {
                MostrarVentanaPoliticasVacias();
            }
        }
        private void MostrarVentanaPoliticasVacias()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No existen politicas para este Checklist, Intentelo mas tarde", Mensaje.ERROR);
            mensajeError.Mostrar();

        }
        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            mensajeError.Mostrar();
            Cerrar();
        }
        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            mensajeError.Mostrar();
            Cerrar();
        }

        private void Cerrar()
        {

        }

        private void ClicAceptar(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCampos();
            if (sonCamposValidos)
            {
                esAprobado_ = ValidarAprobacion();
                MostrarVentanaConfirmacion();
            }
        }

        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea registrar el Dictamen?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                GuardarDictamen();
            }
        }

        private void GuardarDictamen()
        {
            Dictamen dictamen = new Dictamen
            {
                observaciones = txtBoxAnotaciones.Text,
                estaAprobado = esAprobado_,
                fechaHora = DateTime.Now,
                Credito_folioCredito = folio_,
                Usuario_idUsuario = idUsuario_
            };
            Codigo codigo;
            try
            {
                DictamenClient proxy = new DictamenClient();
                codigo = proxy.GuardarDictamen(dictamen);
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex.ToString());
            }
            switch (codigo)
            {
                case Codigo.EXITO:
                    MostrarVentanaExito();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }

        private void MostrarVentanaExito()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Se ha registrado el dictamen exitosamente", Mensaje.EXITO);
            mensajeError.Mostrar();
        }

        private bool ValidarAprobacion() 
        {
            bool esAprobado = true;
            if (listIdPoliticas_.Count==politicas_.Length) 
            {
                esAprobado = true;
            } else 
            {
                esAprobado = false;
            }
            return esAprobado;
        }

        private bool ValidarCampos()
        {
            bool sonCamposValidos = true;
            string anotaciones = txtBoxAnotaciones.Text;
            string razones = "";

            if (String.IsNullOrWhiteSpace(anotaciones))
            {
                sonCamposValidos = false;
                txtBoxAnotaciones.Style = (Style)FindResource("estiloTxtBoxFormularioRojo");
                razones += "Anotaciones";
            }
            else
            {
                txtBoxAnotaciones.Style = (Style)FindResource("estiloTxtBoxFormulario");
            }

            if (!sonCamposValidos)
            {
                MostrarVentanaCamposNoValidos(razones);
            }
            return sonCamposValidos;
        }

        private void MostrarVentanaCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
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

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {

        }
    }
}