using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
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
using WpfFinanciera.ServicioFinancieraIndependiente;
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para FormularioClientePagina.xaml
    /// </summary>
    public partial class FormularioClientePagina : Page
    {
        private Dictionary<string, Button> _botonesTipoArchivo = new Dictionary<string, Button>();
        private Dictionary<string, Documento> _documentos = new Dictionary<string, Documento>();
        private ReferenciaCliente[] _referenciasCliente = new ReferenciaCliente[2];
        private ReferenciaTrabajo _referenciaTrabajo;
        private bool _esRedirigidoBusquedaRFC = false;

        private Cliente _clienteFormulario;

        public FormularioClientePagina()
        {
            InitializeComponent();
            CargarPaginaFormularioCliente();
        }

        public FormularioClientePagina(string rfcClienteNuevo)
        {
            InitializeComponent();
            CargarPaginaFormularioCliente();

            txtBoxRFCCliente.Text = rfcClienteNuevo;
            _esRedirigidoBusquedaRFC = true;
        }

        public FormularioClientePagina(Cliente cliente)
        {
            InitializeComponent();
            CargarPaginaFormularioActualizar(cliente);
        }

        private void CargarPaginaFormularioActualizar(Cliente cliente)
        {
            _documentos.Add("Identificación Oficial", null);
            _documentos.Add("Comprobante Domicilio", null);
            _documentos.Add("Comprobante Ingreso", null);
            _documentos.Add("Comprobante Trabajo", null);
            _documentos.Add("Referencia Cliente 1", null);
            _documentos.Add("Referencia Cliente 2", null);

            _botonesTipoArchivo.Add("Identificación Oficial", btnAgregarArchivoIdentificacion);
            _botonesTipoArchivo.Add("Comprobante Domicilio", btnAgregarComprobanteDomicilio);
            _botonesTipoArchivo.Add("Comprobante Ingreso", btnAgregarComprobanteIngreso);
            _botonesTipoArchivo.Add("Comprobante Trabajo", btnAgregarComprobanteTrabajo);

            btnGuardarCambios.Content = "Guardar Cambios del Cliente";
            btnGuardarCambios.Click -= ClicRegistrarCliente;
            btnGuardarCambios.Click += ClicGuardarCambiosCliente;
            
            btnActualizarReferenciaTrabajo.Content = "Actualizar Referencia de Trabajo";
            btnActualizarReferenciaTrabajo.Click -= ClicSeleccionarReferenciaTrabajo;
            btnActualizarReferenciaTrabajo.Click += ClicActualizarReferenciaTrabajo;

            txtBlockActualizarReferencia1.Text = "Actualizar Referencia del Cliente";
            btnActualizarReferenciaCliente1.Click -= ClicRegistrarReferenciaCliente;
            btnActualizarReferenciaCliente1.Click += ClicActualizarReferenciaCliente;

            txtBlockActualizarReferencia2.Text = "Actualizar Referencia del Cliente";
            btnActualizarReferenciaCliente2.Click -= ClicRegistrarReferenciaCliente;
            btnActualizarReferenciaCliente2.Click += ClicActualizarReferenciaCliente;

            txtBlockTituloPagina.Text = "Actualización del Cliente";

            txtBoxRFCCliente.IsEnabled = false;

            RecuperarDetallesCliente(cliente.rfc);
        }

        private void RecuperarDetallesCliente(string rfc)
        {
            _clienteFormulario = new Cliente();
            Codigo codigo;
            try
            {
                Cliente clienteRecuperado;
                ClienteClient proxy = new ClienteClient();
                (clienteRecuperado, codigo) = proxy.RecuperarDetallesClienteFormulario(rfc);

                if (clienteRecuperado != null)
                {
                    _clienteFormulario = clienteRecuperado;
                }
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    MostrarInformacionCliente();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }

        }

        private void MostrarInformacionCliente()
        {
            txtBoxNombreCliente.Text = _clienteFormulario.nombres;
            txtBoxApellidoCliente.Text = _clienteFormulario.apellidos;
            txtBoxTelefonoCasaCliente.Text = _clienteFormulario.Telefono[0].numeroTelefonico;
            txtBoxTelefonoPersonalCliente.Text = _clienteFormulario.Telefono[1].numeroTelefonico;
            txtBoxRFCCliente.Text = _clienteFormulario.rfc;
            txtBoxCuentaCobroCliente.Text = _clienteFormulario.cuentaCobro;
            txtBoxCuentaDepositoCliente.Text = _clienteFormulario.cuentaDeposito;
            txtBoxCorreoCliente.Text = _clienteFormulario.correoElectronico;
            txtBoxDireccionCliente.Text = _clienteFormulario.direccion;

            txtBoxNombreReferenciaTrabajo.Text = _clienteFormulario.ReferenciaTrabajo.nombre;
            txtBoxDireccionReferenciaTrabajo.Text = _clienteFormulario.ReferenciaTrabajo.direccion;
            txtBoxTelefonoReferenciaTrabajo.Text = _clienteFormulario.ReferenciaTrabajo.telefono;

            txtBoxNombreReferenciaCliente1.Text = _clienteFormulario.ReferenciaCliente[0].nombres + _clienteFormulario.ReferenciaCliente[0].apellidos;
            txtBoxDescripcionReferenciaCliente1.Text = _clienteFormulario.ReferenciaCliente[0].descripcion;
            txtBoxTelefonoReferenciaCliente1.Text = _clienteFormulario.ReferenciaCliente[0].telefono;

            txtBoxNombreReferenciaCliente2.Text = _clienteFormulario.ReferenciaCliente[1].nombres + _clienteFormulario.ReferenciaCliente[1].apellidos;
            txtBoxDescripcionReferenciaCliente2.Text = _clienteFormulario.ReferenciaCliente[1].descripcion;
            txtBoxTelefonoReferenciaCliente2.Text = _clienteFormulario.ReferenciaCliente[1].telefono;

            for (int i=0; i < _clienteFormulario.Documento.Length; i++)
            {
                Documento documento = _clienteFormulario.Documento[i];
                if (_botonesTipoArchivo.ContainsKey(documento.TipoDocumento.descripcion))
                {
                    Button btnDocumento = _botonesTipoArchivo[documento.TipoDocumento.descripcion];
                    _botonesTipoArchivo[documento.TipoDocumento.descripcion].Style = (Style)FindResource("estiloBtnArchivoAdjuntado");
                    btnDocumento.Click -= ClicAgregarArchivo;
                    btnDocumento.DataContext = documento.nombre;
                    btnDocumento.CommandParameter = documento.TipoDocumento.descripcion;
                }

                _documentos[documento.TipoDocumento.descripcion] = documento;
            }

            _referenciaTrabajo = _clienteFormulario.ReferenciaTrabajo;
            _referenciasCliente = _clienteFormulario.ReferenciaCliente;
        }

        private void ClicActualizarReferenciaTrabajo(object sender, RoutedEventArgs e)
        {
            ListaReferenciaTrabajosPagina lista = new ListaReferenciaTrabajosPagina(this, _referenciaTrabajo, _clienteFormulario.rfc);
            MainWindow ventana = (MainWindow)Window.GetWindow(this);
            ventana.CambiarPagina(lista);
        }

        private void ClicActualizarReferenciaCliente(object sender, RoutedEventArgs e)
        {
            Button btnReferenciaCliente = (Button)sender;
            FormularioReferenciaClientePagina formularioReferenciaCliente = new FormularioReferenciaClientePagina(this, _clienteFormulario, btnReferenciaCliente.CommandParameter.ToString());
            MainWindow ventana = (MainWindow)Window.GetWindow(this);
            ventana.CambiarPagina(formularioReferenciaCliente);
        }

        private void ClicGuardarCambiosCliente(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCamposVaciosYValidos();
            if (sonCamposValidos)
            {
                bool documentoExcedeTamanio = ValidarTamanioArchivo();
                if (!documentoExcedeTamanio)
                {
                    MostrarVentanaConfirmacionActualizar();
                }
            }
        }

        private void MostrarVentanaConfirmacionActualizar()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea actualizar al cliente " + txtBoxNombreCliente.Text + " ?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                ActualizarInformacionRegistrada();
            }
        }

        private void ActualizarInformacionRegistrada()
        {
            Telefono[] telefonos = new Telefono[2];
            telefonos[0] = new Telefono { numeroTelefonico = txtBoxTelefonoCasaCliente.Text };
            telefonos[1] = new Telefono { numeroTelefonico = txtBoxTelefonoPersonalCliente.Text };


            Cliente cliente = new Cliente
            {
                nombres = txtBoxNombreCliente.Text.Trim(),
                esDeudor = false,
                apellidos = txtBoxApellidoCliente.Text.Trim(),
                rfc = txtBoxRFCCliente.Text.Trim(),
                cuentaCobro = txtBoxCuentaCobroCliente.Text.Trim(),
                cuentaDeposito = txtBoxCuentaDepositoCliente.Text.Trim(),
                correoElectronico = txtBoxCorreoCliente.Text.Trim(),
                direccion = txtBoxDireccionCliente.Text.Trim(),
                Telefono = telefonos
            };

            Documento[] documentosCliente = new Documento[6];
            int i = 0;
            foreach (KeyValuePair<string, Documento> entrada in _documentos)
            {
                documentosCliente[i] = entrada.Value;
                i++;
            }

            cliente.Documento = documentosCliente;

            Codigo codigo;
            try
            {
                ClienteClient proxy = new ClienteClient();
                codigo = proxy.ActualizarInformacionCliente(cliente);
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    MostrarVentanaExitoActualizacion();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }
        private void MostrarVentanaExitoActualizacion()
        {
            MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
            BusquedaRFCCliente busqueda = new BusquedaRFCCliente();
            ventanaPrincipal.CambiarPagina(busqueda);

            VentanaMensaje ventana = new VentanaMensaje("Se ha actualizado al cliente exitosamente", Mensaje.EXITO);
            ventana.Mostrar();
        }

        private void CargarPaginaFormularioCliente()
        {
            _referenciaTrabajo = null;

            _documentos.Add("Identificación Oficial", null);
            _documentos.Add("Comprobante Domicilio", null);
            _documentos.Add("Comprobante Ingreso", null);
            _documentos.Add("Comprobante Trabajo", null); 
            _documentos.Add("Referencia Cliente 1", null);
            _documentos.Add("Referencia Cliente 2", null);

            _botonesTipoArchivo.Add("Identificación Oficial", btnAgregarArchivoIdentificacion);
            _botonesTipoArchivo.Add("Comprobante Domicilio", btnAgregarComprobanteDomicilio);
            _botonesTipoArchivo.Add("Comprobante Ingreso", btnAgregarComprobanteIngreso);
            _botonesTipoArchivo.Add("Comprobante Trabajo", btnAgregarComprobanteTrabajo);
        }

        private void ClicAgregarArchivo(object sender, RoutedEventArgs e)
        {
            Button btnAgregarArchivo = sender as Button;
            string tipoArchivo = btnAgregarArchivo.CommandParameter.ToString();
            AgregarArchivo(tipoArchivo);
        }

        private void AgregarArchivo(string tipoArchivo)
        {
            OpenFileDialog ventanaArchivo = new OpenFileDialog();
            ventanaArchivo.Title = "Seleccione el archivo con la identificación oficial";
            ventanaArchivo.CheckFileExists = true;
            ventanaArchivo.CheckPathExists = true;
            ventanaArchivo.Filter = "Solo archivos PDF (*.pdf)|*.pdf";
            bool respuesta = (bool)ventanaArchivo.ShowDialog();

            Button btnDocumento = _botonesTipoArchivo[tipoArchivo];

            if (respuesta)
            {
                byte[] archivo = null;
                try
                {
                    archivo = File.ReadAllBytes(ventanaArchivo.FileName);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }

                if (archivo != null)
                {
                    string nombreArchivo = ventanaArchivo.SafeFileName;
                    btnDocumento.Style = (Style)FindResource("estiloBtnArchivoAdjuntado");
                    btnDocumento.Click -= ClicAgregarArchivo;
                    btnDocumento.DataContext = nombreArchivo;
                    btnDocumento.CommandParameter = tipoArchivo;

                    TipoDocumento tipo = new TipoDocumento { idTipoDocumento = 0, descripcion = tipoArchivo };
                    Documento documentoReferencia = new Documento
                    {
                        idDocumento = 0,
                        archivo = archivo,
                        nombre = Path.GetFileName(nombreArchivo),
                        TipoDocumento = tipo
                    };

                    _documentos[tipoArchivo] = documentoReferencia;
                }

            }
        }

        private void ClicEliminarDocumento(object sender, RoutedEventArgs e)
        {
            Button btnEliminar = sender as Button;
            string tipoArchivo = btnEliminar.CommandParameter.ToString();

            Button btnDocumento = _botonesTipoArchivo[tipoArchivo];
            btnDocumento.Style = (Style)FindResource("estiloBtnAgregarArchivo");
            btnDocumento.Click += ClicAgregarArchivo;

            _documentos[tipoArchivo] = null;
        }

        private void ClicSeleccionarReferenciaTrabajo(object sender, RoutedEventArgs e)
        {
            ListaReferenciaTrabajosPagina listaRTpagina = new ListaReferenciaTrabajosPagina(this);
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(listaRTpagina);
        }

        public void AgregarReferenciaTrabajo(ReferenciaTrabajo referenciaTrabajo)
        {
            _referenciaTrabajo = referenciaTrabajo;
            txtBoxNombreReferenciaTrabajo.Text = referenciaTrabajo.nombre;
            txtBoxDireccionReferenciaTrabajo.Text = referenciaTrabajo.direccion;
            txtBoxTelefonoReferenciaTrabajo.Text = referenciaTrabajo.telefono;
        }

        private void ClicRegistrarReferenciaCliente(object sender, RoutedEventArgs e)
        {
            Button btnReferenciaCliente = sender as Button;
            FormularioReferenciaClientePagina formularioReferenciaCliente = new FormularioReferenciaClientePagina(this, btnReferenciaCliente.CommandParameter.ToString());
            MainWindow principal = (MainWindow)Window.GetWindow(this);
            principal.CambiarPagina(formularioReferenciaCliente);
        }

        public void AgregarReferenciaCliente(ReferenciaCliente referenciaCliente, Documento documentoReferencia, string numeroReferencia)
        {
            int numReferenciaCliente = int.Parse(numeroReferencia) - 1;

            if (_clienteFormulario != null)
            {
                _clienteFormulario.ReferenciaCliente[numReferenciaCliente] = referenciaCliente;
                int numDocumento = _documentos.ToList().FindIndex(documento => documento.Value.TipoDocumento.descripcion.Equals("Referencia Cliente " + numeroReferencia));
                _clienteFormulario.Documento[numDocumento] = documentoReferencia;
            }
           
            _referenciasCliente[numReferenciaCliente] = referenciaCliente;
            _documentos[documentoReferencia.TipoDocumento.descripcion] = documentoReferencia;
            
            switch (numeroReferencia)
            {
                case "1":
                    txtBoxNombreReferenciaCliente1.Text = referenciaCliente.nombres + " " + referenciaCliente.apellidos;
                    txtBoxDescripcionReferenciaCliente1.Text = referenciaCliente.descripcion;
                    txtBoxTelefonoReferenciaCliente1.Text = referenciaCliente.telefono;
                    break;
                case "2":
                    txtBoxNombreReferenciaCliente2.Text = referenciaCliente.nombres + " " + referenciaCliente.apellidos;
                    txtBoxDescripcionReferenciaCliente2.Text = referenciaCliente.descripcion;
                    txtBoxTelefonoReferenciaCliente2.Text = referenciaCliente.telefono;
                    break;
            }
        }

        private void ClicRegistrarCliente(object sender, RoutedEventArgs e)
        {
            bool sonCamposValidos = ValidarCamposVaciosYValidos();
            if (sonCamposValidos)
            {
                bool documentoExcedeTamanio = ValidarTamanioArchivo();
                if (!documentoExcedeTamanio)
                {
                    bool esUnicoRfc = ValidarRfcUnico();
                    if (esUnicoRfc)
                    {
                        MostrarVentanaConfirmacion();
                    }
                }
            }
        }

        private bool ValidarCamposVaciosYValidos()
        {
            ResetearCampos();
            bool sonCamposValidos = true;
            string razones = "";

            string nombres = txtBoxNombreCliente.Text.Trim();
            string apellidos = txtBoxApellidoCliente.Text.Trim();
            string telefonoCasa = txtBoxTelefonoCasaCliente.Text.Trim();
            string telefonoPersonal = txtBoxTelefonoPersonalCliente.Text.Trim();
            string rfc = txtBoxRFCCliente.Text.Trim();
            string cuentaCobro = txtBoxCuentaCobroCliente.Text.Trim();
            string cuentaDeposito = txtBoxCuentaDepositoCliente.Text.Trim();
            string correoElectronico = txtBoxCorreoCliente.Text.Trim();
            string direccion = txtBoxDireccionCliente.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombres) || nombres.Length > 50)
            {
                txtBoxNombreCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = "Nombre(s) (debe ser menor a 51 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(apellidos) || apellidos.Length > 50)
            {
                txtBoxApellidoCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Apellidos (debe ser menor a 51 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(telefonoCasa) || telefonoCasa.Length > 15)
            {
                txtBoxTelefonoCasaCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Teléfono de casa (debe ser menor a 16 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(telefonoPersonal) || telefonoPersonal.Length > 15)
            {
                txtBoxTelefonoPersonalCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Teléfono personal (debe ser menor a 16 caracteres)";
            }

            if (string.IsNullOrWhiteSpace(rfc) || !Regex.IsMatch(rfc, @"^[a-zA-Z]{4}[0-9]{6}[a-zA-Z0-9]{3}$"))
            {
                txtBoxRFCCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "RFC (debe ser igual a 13 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(cuentaCobro) || cuentaCobro.Length > 50)
            {
                txtBoxCuentaCobroCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Cuenta de cobro (debe ser menor a 51 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(cuentaDeposito) || cuentaDeposito.Length > 50)
            {
                txtBoxCuentaDepositoCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Cuenta de depósito (debe ser menor a 51 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(correoElectronico) || correoElectronico.Length > 100 || !Regex.IsMatch(correoElectronico, @"^[a-zA-Z0-9._%+-]+@(gmail\.com|hotmail\.com|outlook\.com|estudiantes\.uv\.mx|uv\.mx|yahoo\.com)$"))
            {
                txtBoxCorreoCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Correo Electrónico (debe ser menor a 101 caracteres)";
            }
            if (string.IsNullOrWhiteSpace(direccion) || direccion.Length > 50)
            {
                txtBoxDireccionCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Dirección (debe ser menor a 21 caracteres)";
            }
            if (_referenciaTrabajo == null)
            {
                brdReferenciaTrabajoBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FCC6C1");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Referencia Trabajo";
            }
            if (_referenciasCliente[0] == null)
            {
                brdReferenciaCliente1.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FCC6C1");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Referencia Cliente #1";
            }
            if (_referenciasCliente[1] == null)
            {
                brdReferenciaCliente2.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FCC6C1");
                sonCamposValidos = false;
                razones = (razones.Length > 0) ? razones + ", " : razones;
                razones += "Referencia Cliente #2";
            }
            foreach(KeyValuePair<string, Documento> entrada in _documentos)
            {
                if (entrada.Value == null && !entrada.Key.Equals("Referencia Cliente 1") && !entrada.Key.Equals("Referencia Cliente 2"))
                {
                    _botonesTipoArchivo[entrada.Key].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                    sonCamposValidos = false;
                    razones = (razones.Length > 0) ? razones + ", " : razones;
                    razones += entrada.Key + " (el documento es un campo obligatorio)";
                }
                else if (entrada.Value != null && entrada.Value.nombre.Length > 50)
                {
                    _botonesTipoArchivo[entrada.Key].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                    sonCamposValidos = false;
                    razones = (razones.Length > 0) ? razones + ", " : razones;
                    razones += entrada.Key + " (el nombre debe ser menor a 51 caracteres)";
                }
            }

            if (!sonCamposValidos)
            {
                MostrarMensajeCamposNoValidos(razones);
            }

            return sonCamposValidos;

        }

        private bool ValidarTamanioArchivo()
        {
            bool documentoExcedeTamanio = false;
            string nombreDocumentos = "";
            foreach (KeyValuePair<string, Documento> entrada in _documentos)
            {
                if (entrada.Value != null && entrada.Key != "Referencia Cliente 1" && entrada.Key != "Referencia Cliente 2")
                {
                    float tamanioArchivo = (entrada.Value.archivo.Length / 1024.0F);
                    if (tamanioArchivo > 1000)
                    {
                        documentoExcedeTamanio = true;
                        _botonesTipoArchivo[entrada.Key].Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#C46960");
                        nombreDocumentos = (nombreDocumentos.Length > 0) ? nombreDocumentos + ", " : nombreDocumentos;
                        nombreDocumentos += entrada.Key;
                    }   
                }
            }

            if (documentoExcedeTamanio)
            {
                MostrarVentanaTamanioNoValido(nombreDocumentos);
            }
            return documentoExcedeTamanio;
        }

        private bool ValidarRfcUnico()
        {
            bool esUnicoRfc = false;
            Codigo codigo;
            try
            {
                ClienteClient proxy = new ClienteClient();
                (esUnicoRfc, codigo) = proxy.ValidarRfcClienteUnico(txtBoxRFCCliente.Text.Trim());
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    if (!esUnicoRfc)
                    {
                        MostrarVentanaRfcRepetido();
                    }
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }

            return esUnicoRfc;
        }

        private void MostrarMensajeCamposNoValidos(string razones)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("Los campos ingresados no son válidos", razones);
            mensajeError.Mostrar();
        }

        private void MostrarVentanaTamanioNoValido(string nombreDocumentos)
        {
            VentanaMensaje mensajeError = new VentanaMensaje("El documento excede el tamaño límite de 1MB, reemplace el archivo", nombreDocumentos);
            mensajeError.Mostrar();
        }

        private void ResetearCampos()
        {
            txtBoxNombreCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxApellidoCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxTelefonoCasaCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxTelefonoPersonalCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxRFCCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxCuentaCobroCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxCuentaDepositoCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxCorreoCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            txtBoxDireccionCliente.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#127B7C");
            brdReferenciaTrabajoBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#E2F0EF");
            brdReferenciaCliente1.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#E2F0EF");
            brdReferenciaCliente2.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#E2F0EF");
        }

        private void PreviewKeyDownValidarNumero(object sender, KeyEventArgs e)
        {
            if (!char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) && e.Key != Key.Delete && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }

        private void MostrarVentanaRfcRepetido()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("El RFC ingresado ya está registrado en el sistema, inténtelo de nuevo con otro RFC o verifique la cuenta del cliente existente", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
        }
        private void MostrarVentanaErrorServidor()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con el servidor. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
            DeshabilitarBotones();
        }

        private void MostrarVentanaErrorBaseDatos()
        {
            VentanaMensaje ventanaMensaje = new VentanaMensaje("Error. No se pudo conectar con la base de datos. Inténtelo de nuevo o hágalo más tarde", Mensaje.ERROR);
            ventanaMensaje.Mostrar();
            DeshabilitarBotones();
        }

        private void MostrarVentanaConfirmacion()
        {
            VentanaMensaje ventana = new VentanaMensaje("¿Desea registrar al cliente " + txtBoxNombreCliente.Text + " ?", Mensaje.CONFIRMACION);
            if (ventana.MostrarConfirmacion())
            {
                GuardarCliente();
            }
        }
        private void GuardarCliente()
        {
            Telefono[] telefonos = new Telefono[2];
            telefonos[0] = new Telefono { numeroTelefonico = txtBoxTelefonoCasaCliente.Text };
            telefonos[1] = new Telefono { numeroTelefonico = txtBoxTelefonoPersonalCliente.Text };


            Cliente cliente = new Cliente
            {
                 nombres = txtBoxNombreCliente.Text.Trim(),
                 esDeudor = false,
                 apellidos = txtBoxApellidoCliente.Text.Trim(),
                 rfc = txtBoxRFCCliente.Text.Trim(),
                 cuentaCobro = txtBoxCuentaCobroCliente.Text.Trim(),
                 cuentaDeposito = txtBoxCuentaDepositoCliente.Text.Trim(),
                 correoElectronico = txtBoxCorreoCliente.Text.Trim(),
                 direccion = txtBoxDireccionCliente.Text.Trim(),
                 Telefono = telefonos
            };

            Documento[] documentosCliente = new Documento[6];
            int i = 0;
            foreach (KeyValuePair<string, Documento> entrada in _documentos)
            {
                documentosCliente[i] = entrada.Value;
                i++;
            }

            Codigo codigo;
            try
            {
                ClienteClient proxy = new ClienteClient();
                codigo = proxy.GuardarInformacionCliente(cliente, _referenciaTrabajo, _referenciasCliente, documentosCliente);
            }
            catch (CommunicationException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                codigo = Codigo.ERROR_SERVIDOR;
                Console.WriteLine(ex);
            }

            switch (codigo)
            {
                case Codigo.EXITO:
                    MostrarVentanaExitoRegistro();
                    break;
                case Codigo.ERROR_SERVIDOR:
                    MostrarVentanaErrorServidor();
                    break;
                case Codigo.ERROR_BD:
                    MostrarVentanaErrorBaseDatos();
                    break;
            }
        }

        private void MostrarVentanaExitoRegistro()
        {
            MainWindow ventanaPrincipal = (MainWindow) Window.GetWindow(this);
            BusquedaRFCCliente busqueda = new BusquedaRFCCliente();
            ventanaPrincipal.CambiarPagina(busqueda);

            VentanaMensaje ventana = new VentanaMensaje("Se ha registrado al cliente exitosamente", Mensaje.EXITO);
            ventana.Mostrar();
        }

        private void ClicRegresar(object sender, RoutedEventArgs e)
        {
            if (_esRedirigidoBusquedaRFC == true)
            {
                MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
                BusquedaRFCCliente busquedaRFCCliente = new BusquedaRFCCliente();
                ventanaPrincipal.CambiarPagina(busquedaRFCCliente);
            }
            else
            {
                MainWindow ventanaPrincipal = (MainWindow)Window.GetWindow(this);
                MenuPrincipalAsesorCreditoPagina menuAsesor = new MenuPrincipalAsesorCreditoPagina();
                ventanaPrincipal.CambiarPagina(menuAsesor);
            }
        }

        private void DeshabilitarBotones()
        {
            btnActualizarReferenciaTrabajo.IsEnabled = false;
            btnAgregarArchivoIdentificacion.IsEnabled = false;
            btnAgregarComprobanteDomicilio.IsEnabled = false;
            btnAgregarComprobanteIngreso.IsEnabled = false;
            btnAgregarComprobanteTrabajo.IsEnabled=false;
            btnActualizarReferenciaCliente1.IsEnabled = false;
            btnActualizarReferenciaCliente2.IsEnabled = false;
            btnGuardarCambios.IsEnabled = false;
        }
    }

}


