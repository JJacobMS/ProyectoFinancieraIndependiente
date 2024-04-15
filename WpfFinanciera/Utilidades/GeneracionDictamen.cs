using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFinanciera.Utilidades
{
    public static class GeneracionDictamen
    {
        public static void GenerarDictamen()
        {
            byte[] archivo =
            Document.Create(container =>
            {
                string nombre = "Sulem Un nombre muy largo de muchos caracteres a a"; //
                string apellidos = "Martínez Aguilar con un apellido largo largo largo";
                string rfc = "QUMA470929F37";
                string telefono = "2281367803";
                string correo = "svanfiovñdulem477isblvuiabvbaibviuasvifuovnuribfv@gamiadvioafnvoaf";
                string direccion = "Ezqeuqle alva avila aca por nombirem aieav esat a1";
                string cuenta = "ES12 3456 7890 12 123456789";

                string folio = "000000001";
                string fecha = "28/02/2024 11:05";
                string monto = "$20,000";

                string comentarios = "dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv s" +
                "f e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs sf e dvsvs vsfv sf e dvsvs vsfv sf e dvsvs vsfv sf";


                bool esAprobado = false;
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
                                table.Cell().RowSpan(2).Background(Colors.White).AlignMiddle().Padding(2).Image("Recursos/financiera.png");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                Console.WriteLine("pipipi");//To-do
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
                                text.Span(DarFormato(nombre, 50)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("RFC: ");
                                text.Span(DarFormato(rfc, 13)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Apellidos: ");
                                text.Span(DarFormato(apellidos, 50)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Teléfono de casa: ");
                                text.Span(DarFormato(telefono, 15)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Dirección: ");
                                text.Span(DarFormato(direccion, 50)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Teléfono personal: ");
                                text.Span(DarFormato(telefono, 15)).Underline();
                            });

                            table.Cell().ColumnSpan(2).Text(text =>
                            {
                                text.Span("Correo electrónico: ");
                                text.Span(DarFormato(correo, 80)).Underline();
                            });

                            table.Cell().ColumnSpan(2).Text(text =>
                            {
                                text.Span("Cuenta de cobro: ");
                                text.Span(DarFormato(cuenta, 80)).Underline();
                            });

                            table.Cell().ColumnSpan(2).Text(text =>
                            {
                                text.Span("Cuenta de depósito: ");
                                text.Span(DarFormato(cuenta, 80)).Underline();
                            });

                            table.Cell().ColumnSpan(2).Text(text =>
                            {
                                text.Span("Estado del cliente: ");
                                text.Span(DarFormato(cuenta, 80)).Underline();
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
                                text.Span(DarFormato(folio, 80)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Fecha de solicitud: ");
                                text.Span(DarFormato(fecha, 60)).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Monto: ");
                                text.Span(DarFormato(monto, 80)).Underline();
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
                                string estado = esAprobado ? "X" : "";
                                text.Span(linea).Underline();
                                text.Span(estado).FontColor("#065758").Underline();
                                text.Span(linea).Underline();
                            });

                            table.Cell().Text(text =>
                            {
                                text.Span("Rechazado: ");
                                string estado = esAprobado ? "" : "X";
                                text.Span(linea).Underline();
                                text.Span(estado).FontColor("#9B2E24").Underline();
                                text.Span(linea).Underline();
                            });

                            table.Cell().ColumnSpan(3).Text(text =>
                            {
                                text.Span("Fecha de revisión: ");
                                text.Span(DarFormato(fecha, 100)).Underline();
                            });

                            table.Cell().ColumnSpan(3).Text(text =>
                            {
                                text.Span("Comentarios: ").LineHeight(1f);
                                text.Span(DarFormato(comentarios, 200)).Underline().LineHeight(1f);
                            });

                            table.Cell().ColumnSpan(3).PaddingVertical(5).Text(text =>
                            {
                                text.Span("Examinado por: ").LineHeight(1f);
                                text.Span(DarFormato(nombre + " " + apellidos, 100)).Underline().LineHeight(1f);
                            });

                        });

                    });
                }
            })
            .GeneratePdf();

            File.WriteAllBytes("archivo.pdf", archivo);
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
