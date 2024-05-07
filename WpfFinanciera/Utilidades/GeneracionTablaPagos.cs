using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFinanciera.ServicioFinancieraIndependiente;

namespace WpfFinanciera.Utilidades
{
    public class GeneracionTablaPagos
    {
        public static byte[] GenerarTablaPagos(List<FilaCobro> filas)
        {
            byte[] csvBytes = null;

            Func<FilaCobroMapper> cobroMapFactory = () => new FilaCobroMapper();

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                    {
                        using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                        {
                            csvWriter.Context.RegisterClassMap(cobroMapFactory.Invoke());
                            csvWriter.WriteRecords(filas);
                        }
                    }

                    csvBytes = memoryStream.ToArray();

                }
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return csvBytes;
        }
    }


    public class FilaCobro
    {
        public Cobro CobroRepresentado { get; set; }
        public DateTime RangoPrimero { get; set; }
        public string TipoMes { get; set; }
        public string Dia { get; set; }
        public string Mes { get; set; }
        public string Anio { get; set; }
        public double? TotalPagar { get; set; }
        public double? RestanteAPagar { get; set; }
        public double? TotalPagarMes { get; set; }
        public string Folio { get; set; }
        public string Fecha { get; set; }
        public string Importe { get; set; }
        public double? RestanteAPagarPorMes { get; set; }
        public double? DeudaExtra { get; set; }

    }

    public class FilaCobroMapper : ClassMap<FilaCobro>
    {
        public FilaCobroMapper()
        {
            Map(fila => fila.TipoMes).Name("Tipo:");
            Map(fila => fila.Dia).Name("Día inicial:");
            Map(fila => fila.Mes).Name("Mes inicial:").Convert(fila =>
                (fila.Value.Mes != null) ? char.ToUpper(fila.Value.Mes[0]) + fila.Value.Mes.Substring(1) : "");
            Map(fila => fila.Anio).Name("Año inicial:");
            Map(r => r.TotalPagar).Name("Total a pagar:").Convert(fila =>
            {
                return fila.Value.TotalPagar.HasValue ? $"${fila.Value.TotalPagar}" : String.Empty;
            });
            Map(r => r.RestanteAPagar).Name("Restante a pagar:").Convert(fila =>
            {
                return fila.Value.RestanteAPagar.HasValue ? $"${fila.Value.RestanteAPagar}" : String.Empty;
            });
            Map(r => r.TotalPagarMes).Name("Total a pagar del mes:").Convert(fila =>
            {
                return fila.Value.TotalPagarMes.HasValue ? $"${fila.Value.TotalPagarMes}" : String.Empty;
            });
            Map(r => r.RestanteAPagarPorMes).Name("Restante a pagar del mes:").Convert(fila =>
            {
                return fila.Value.RestanteAPagarPorMes.HasValue ? $"${fila.Value.RestanteAPagarPorMes}" : String.Empty;
            });
            Map(r => r.DeudaExtra).Name("Deuda extra total:").Convert(fila =>
            {
                return fila.Value.DeudaExtra.HasValue ? $"${fila.Value.DeudaExtra}" : String.Empty;
            });


            Map(r => r.Folio).Name("Número de cobro:");
            Map(r => r.Fecha).Name("Fecha:");
            Map(r => r.Importe).Name("Importe:");
        }
    }
}
