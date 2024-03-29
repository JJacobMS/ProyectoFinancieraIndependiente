﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;
using WpfFinanciera.ServicioFinancieraIndependiente;
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioReferenciaClientePagina.xaml
    /// </summary>
    public partial class FormularioReferenciaClientePagina : Page
    {
        private FormularioClientePagina _formularioCliente;
        private byte[] _documentoReferencia;
        private string _nombreArchivo;
        private string _numeroReferenciaCliente;
        public FormularioReferenciaClientePagina(FormularioClientePagina formularioCliente, string numeroReferenciaCliente)
        {
            InitializeComponent();
            _formularioCliente = formularioCliente;
            _documentoReferencia = null;
            _nombreArchivo = null;
            _numeroReferenciaCliente = numeroReferenciaCliente;
            CargarPaginaFormularioReferenciaCliente(numeroReferenciaCliente);
        }

        private void CargarPaginaFormularioReferenciaCliente(string numeroReferenciaCliente)
        {
            rnNumeroReferenciaCliente.Text = numeroReferenciaCliente;
            rnBtnAgregarReferencia.Text = numeroReferenciaCliente;
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(_formularioCliente);
        }

        private void ClicAdjuntarDocumento(object sender, RoutedEventArgs e)
        {
            AgregarArchivo();
        }
        private void AgregarArchivo()
        {
            OpenFileDialog ventanaArchivo = new OpenFileDialog();
            ventanaArchivo.Title = "Seleccione el archivo con la identificación oficial";
            ventanaArchivo.CheckFileExists = true;
            ventanaArchivo.CheckPathExists = true;
            ventanaArchivo.Filter = "Solo archivos PDF (*.pdf)|*.pdf";
            bool respuesta = (bool)ventanaArchivo.ShowDialog();

            if (respuesta)
            {
                string nombreArchivo = ventanaArchivo.SafeFileName;
                btnAgregarArchivo.Style = (Style)FindResource("estiloBtnArchivoAdjuntado");
                btnAgregarArchivo.Click -= ClicAdjuntarDocumento;
                btnAgregarArchivo.DataContext = nombreArchivo;

                try
                {
                    _documentoReferencia = File.ReadAllBytes(ventanaArchivo.FileName);
                    _nombreArchivo = nombreArchivo;
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    MostrarMensajeErrorArchivo();
                }

            }
        }
        private void MostrarMensajeErrorArchivo()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("Error. No se pudo agregar el documento", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
        }
        private void ClicEliminarDocumento(object sender, RoutedEventArgs e)
        {
            _documentoReferencia = null;
            _nombreArchivo = null;
            btnAgregarArchivo.Style = (Style)FindResource("estiloBtnAgregarArchivo");
            btnAgregarArchivo.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            btnAgregarArchivo.Click += ClicAdjuntarDocumento;

        }

        private void ClicAgregarReferenciaCliente(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCamposVaciosYValidos();
            if (sonCamposValidos)
            {
                bool documentoExcedeTamanio = ValidarTamanioArchivo();
                if (!documentoExcedeTamanio)
                {
                    MostrarVentanaConfirmacion();
                }
            }
        }

        private bool ValidarCamposVaciosYValidos()
        {
            ResetearCampos();
            bool sonCamposValidos = true;
            string razones = "";

            string nombre = txtBoxNombreReferencia.Text.Trim();
            string apellidos = txtBoxApellidosReferencia.Text.Trim();
            string telefono = txtBoxTelefonoReferencia.Text.Trim();
            string descripcion = txtBoxDescripcionReferencia.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 50)
            {
                txtBoxNombreReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones += "Nombre (debe ser menor a 51 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(apellidos) || apellidos.Length > 50)
            {
                txtBoxApellidosReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Apellidos (debe ser menor a 51 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length > 12)
            {
                txtBoxTelefonoReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Teléfono (debe ser menor a 13 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Length > 35)
            {
                txtBoxDescripcionReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Descripción (debe ser menor a 36 caracteres)";

            }
            if (_documentoReferencia == null || _nombreArchivo.Length > 50)
            {
                btnAgregarArchivo.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Documento Referencia Cliente (el nombre debe ser menor a 51 caracteres)";
            }

            if (!sonCamposValidos)
            {
                MostrarMensajeCamposNoValidos(razones);
            }

            return sonCamposValidos;

        }
        private bool ValidarTamanioArchivo()
        {
            bool documentoExcedeTamanio = true;
            if (_documentoReferencia != null)
            {
                float tamanioArchivo = (_documentoReferencia.Length / 1024.0F);
                if (tamanioArchivo < 1001)
                {
                    documentoExcedeTamanio = false;
                }
            }

            if (documentoExcedeTamanio)
            {
                btnAgregarArchivo.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                MostrarVentanaTamanioNoValido();
            }
            return documentoExcedeTamanio;
        }
        private void ResetearCampos()
        {
            btnAgregarArchivo.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxNombreReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxApellidosReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxTelefonoReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxDescripcionReferencia.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
        }
        private void MostrarMensajeCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
        }
        private void MostrarVentanaTamanioNoValido()
        {
            VentanaMensaje mensajeError = new VentanaMensaje("El documento excede el tamaño límite de 1MB, reemplace el archivo", Mensaje.ERROR);
            mensajeError.Mostrar();
        }
        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea agregar la referencia de cliente?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                EnviarReferenciaCliente();
            }
        }
        private void EnviarReferenciaCliente()
        {
            TipoDocumento tipo = new TipoDocumento { descripcion = "Referencia Cliente " +  _numeroReferenciaCliente};
            Documento documentoReferencia = new Documento
            {
                idDocumento = 0, archivo = _documentoReferencia, nombre = Path.GetFileNameWithoutExtension(_nombreArchivo), 
                extension = Path.GetExtension(_nombreArchivo), TipoDocumento = tipo
            };
            ReferenciaCliente referenciaCliente = new ReferenciaCliente
            {
                idReferenciaCliente = 0,
                nombres = txtBoxNombreReferencia.Text.Trim(),
                apellidos = txtBoxApellidosReferencia.Text.Trim(),
                descripcion = txtBoxDescripcionReferencia.Text.Trim(),
                telefono = txtBoxTelefonoReferencia.Text.Trim()
            };
            _formularioCliente.AgregarReferenciaCliente(referenciaCliente, documentoReferencia, _numeroReferenciaCliente);
            MainWindow ventana = (MainWindow)Window.GetWindow(this);
            ventana.CambiarPagina(_formularioCliente);

        }

        private void PreviewKeyDownValidarNumero(object sender, KeyEventArgs e)
        {
            if (!char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) && e.Key != Key.Delete && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }
    }
}
