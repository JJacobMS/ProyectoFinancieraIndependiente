using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using WpfFinanciera.Utilidades;

namespace WpfFinanciera.Vistas
{
    /// <summary>
    /// Lógica de interacción para Grafico.xaml
    /// </summary>
    public partial class Grafico : Page
    {
        private List<CategoriasGraficoPastel> Categorias { get; set; }


        public Grafico()
        {
            InitializeComponent();

            float ancho = 650, largo = 650, centroX = ancho / 2, centroY = largo / 2, radio = ancho / 2;
            mainCanvas.Width = ancho;
            mainCanvas.Height = largo;

            Categorias = new List<CategoriasGraficoPastel>()
            {
                new CategoriasGraficoPastel
                {
                    Titulo = "Porcentaje de Eficiencia",
                    Porcentaje = 99.5,
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#146081")),
                },

                new CategoriasGraficoPastel
                {
                    Titulo = "Porcentaje de Falta de Cobro",
                    Porcentaje = .5,
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E77032")),
                },
            };

            detailsItemsControl.ItemsSource = Categorias;
            double angulo = 0, anguloPrevio = 0;
            foreach (var categoria in Categorias)
            {
                double linea1X = (radio * Math.Cos(angulo * Math.PI / 180)) + centroX;
                double linea1Y = (radio * Math.Sin(angulo * Math.PI / 180)) + centroY;

                angulo = categoria.Porcentaje * (float)360 / 100 + anguloPrevio;
                Debug.WriteLine(angulo);

                double arcoX = (radio * Math.Cos(angulo * Math.PI / 180)) + centroX;
                double arcoY = (radio * Math.Sin(angulo * Math.PI / 180)) + centroY;

                var lineaSegmento1 = new LineSegment(new Point(linea1X, linea1Y), false);
                double arcWidth = radio, arcHeight = radio;
                bool isLargeArc = categoria.Porcentaje > 50;
                var segmento = new ArcSegment()
                {
                    Size = new Size(arcWidth, arcHeight),
                    Point = new Point(arcoX, arcoY),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = isLargeArc,
                };
                var line2Segment = new LineSegment(new Point(centroX, centroY), false);

                var figura = new PathFigure(
                    new Point(centroX, centroY),
                    new List<PathSegment>()
                    {
                        lineaSegmento1,
                        segmento,
                        line2Segment,
                    },
                    true);

                var figuras = new List<PathFigure>() { figura, };
                var geometria = new PathGeometry(figuras);
                var path = new System.Windows.Shapes.Path()
                {
                    Fill = categoria.Color,
                    Data = geometria,
                };
                mainCanvas.Children.Add(path);

                anguloPrevio = angulo;


                var linea1 = new Line()
                {
                    X1 = centroX,
                    Y1 = centroY,
                    X2 = lineaSegmento1.Point.X,
                    Y2 = lineaSegmento1.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 5,
                };
                var linea2 = new Line()
                {
                    X1 = centroX,
                    Y1 = centroY,
                    X2 = segmento.Point.X,
                    Y2 = segmento.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 5,
                };

                mainCanvas.Children.Add(linea1);
                mainCanvas.Children.Add(linea2);
            }
        }
    }

}

