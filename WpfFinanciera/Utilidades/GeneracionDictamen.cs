using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFinanciera.ServicioFinancieraIndependiente;

namespace WpfFinanciera.Utilidades
{
    public static class GeneracionDictamen
    {
        public static byte[] GenerarDictamen(Credito credito)
        {
            byte[] archivo =
            Document.Create(container =>
            {
                string linea = "_____";

                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(1.27f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10.5f).FontFamily(Fonts.Tahoma).LineHeight(1.75f));

                    page.Header().Element(Header);
                    page.Content().Element(Content);

                });

                void Header(IContainer containerHeader)
                {
                    containerHeader.Column(column =>
                    {
                        column.Item().Border(.5f).Background("#065758").Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(8);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Cell().Border(.5f).Column(columnCell =>
                            {
                                columnCell.Item().AlignMiddle().PaddingHorizontal(5).PaddingTop(20).Text("FINANCIERA INDEPENDIENTE").FontColor(Colors.White).LineHeight(1f);
                            });

                            table.Cell().Border(.5f).Column(columnCell =>
                            {
                                columnCell.Item().AlignMiddle().PaddingHorizontal(5).PaddingVertical(16).Text("AVISO PARA PRESENTAR DICTAMEN RESPECTO A LA APROBACIÓN DEL " +
                                "CRÉDITO AL CLIENTE RESPECTO A LAS CONDICIONES Y POLÍTICAS OBLIGADAS A CUMPLIR").FontColor(Colors.White).LineHeight(1f);
                            });

                            try
                            {
                                string path = @"..\..\Recursos\financiera.png";
                                byte[] imageData = File.ReadAllBytes(path);
                                table.Cell().RowSpan(2).Background(Colors.White).AlignMiddle().Padding(2).Image(imageData);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                            }

                            table.Cell().ColumnSpan(2).Border(.5f).Column(columnCell =>
                            {
                                columnCell.Item().AlignCenter().AlignMiddle().PaddingVertical(5).Text("USO EXCLUSIVO PARA LA FINANCIERA").FontColor(Colors.White).LineHeight(1f).FontSize(9);
                            });

                        });
                    });

                }

                void Content(IContainer containerContent)
                {
                    containerContent.Column(column =>
                    {
                        column.Item().AlignCenter().PaddingBottom(2).PaddingTop(24).Text("DATOS DEL CLIENTE").LineHeight(1f);

                        column.Item().Border(.5f).PaddingHorizontal(9).PaddingVertical(6).Table(table =>
                        {

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn(.5f);
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Nombre(s): ");
                                text.Span(DarFormato(credito.Cliente.nombres, 50)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("RFC: ");
                                text.Span(DarFormato(credito.Cliente.rfc, 13)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Apellidos: ");
                                text.Span(DarFormato(credito.Cliente.apellidos, 50)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Teléfono de casa: ");
                                text.Span(DarFormato(credito.Cliente.Telefono[0].numeroTelefonico, 15)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Dirección: ");
                                text.Span(DarFormato(credito.Cliente.direccion, 50)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Teléfono personal: ");
                                text.Span(DarFormato(credito.Cliente.Telefono[1].numeroTelefonico, 15)).Underline();
                            });

                            table.Cell().ColumnSpan(2).Text(text =>
                            {
                                text.Span("Correo electrónico: ");
                                text.Span(DarFormato(credito.Cliente.correoElectronico, 80)).Underline();
                            });

                            table.Cell().ColumnSpan(2).Text(text =>
                            {
                                text.Span("Cuenta de cobro: ");
                                text.Span(DarFormato(credito.Cliente.cuentaCobro, 80)).Underline();
                            });

                            table.Cell().ColumnSpan(2).Text(text =>
                            {
                                text.Span("Cuenta de depósito: ");
                                text.Span(DarFormato(credito.Cliente.cuentaDeposito, 80)).Underline();
                            });


                        });

                        column.Item().AlignCenter().PaddingBottom(2).PaddingTop(24).Text("DATOS DEL CRÉDITO").LineHeight(1f);

                        column.Item().Border(.5f).PaddingHorizontal(9).PaddingVertical(6).Table(table =>
                        {

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Folio: ");
                                text.Span(DarFormato(credito.folioCredito.ToString(), 80)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Fecha de solicitud: ");
                                text.Span(DarFormato(credito.fechaSolicitud.ToShortTimeString(), 60)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Monto: ");
                                text.Span(DarFormato(credito.monto.ToString(), 80)).Underline();
                            });

                        });

                        column.Item().AlignCenter().PaddingBottom(2).PaddingTop(24).Text("CONCLUSIÓN DEL DICTAMEN").LineHeight(1f);

                        column.Item().Border(.5f).PaddingHorizontal(9).PaddingVertical(6).Table(table =>
                        {

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Conclusión:");
                            });


                            table.Cell().Text(text =>
                            {
                                text.Span("Aceptado: ");
                                string estado = credito.Dictamen[0].estaAprobado ? "X" : "";
                                text.Span(linea).Underline();
                                text.Span(estado).FontColor("#065758").Underline();
                                text.Span(linea).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Rechazado: ");
                                string estado = credito.Dictamen[0].estaAprobado ? "" : "X";
                                text.Span(linea).Underline();
                                text.Span(estado).FontColor("#9B2E24").Underline();
                                text.Span(linea).Underline();
                            });

                            table.Cell().ColumnSpan(3).Text(text =>
                            {
                                text.Span("Fecha de revisión: ");
                                text.Span(DarFormato(credito.Dictamen[0].fechaHora.ToShortTimeString(), 100)).Underline();
                            });

                            table.Cell().ColumnSpan(3).Text(text =>
                            {
                                text.Span("Comentarios: ").LineHeight(1f);
                                text.Span(DarFormato(credito.Dictamen[0].observaciones, 200)).Underline().LineHeight(1f);
                            });

                            table.Cell().ColumnSpan(3).PaddingVertical(5).Text(text =>
                            {
                                text.Span("Examinado por: ").LineHeight(1f);
                                text.Span(DarFormato(credito.Dictamen[0].Usuario.nombres + " " + credito.Dictamen[0].Usuario.apellidos, 100)).Underline().LineHeight(1f);
                            });

                        });

                    });
                }
            })
            .GeneratePdf();

            return archivo;
        }

        private static string DarFormato(string cadena, int totales)
        {
            cadena = " " + cadena;
            for (int i = cadena.Length; i < totales; i++)
            {
                cadena += " ";
            }
            return cadena + " ";
        }
    }
}
