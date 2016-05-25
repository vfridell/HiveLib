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
using HiveLib.Models;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<Hexagon> hexagons = new List<Hexagon>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hex hex = new Hex(24, 24);
            Hex hex2 = new Hex(25, 24);
            Hex hex3 = new Hex(0, 0);

            hexagons.Add(new Hexagon(HexCoordToCenterPoint(hex, 10), 10));
            hexagons.Add(new Hexagon(HexCoordToCenterPoint(hex2, 10), 10));
            hexagons.Add(new Hexagon(HexCoordToCenterPoint(hex3, 10), 10));
            hexagons.Add(new Hexagon(HexCoordToCenterPoint(new Hex(1,0), 10), 10));
            hexagons.Add(new Hexagon(HexCoordToCenterPoint(new Hex(1,1), 10), 10));
            hexagons.Add(new Hexagon(HexCoordToCenterPoint(new Hex(0,1), 10), 10));
            foreach(Hexagon hexagon in hexagons)
            {
                //Canvas.SetLeft(hexagon.polygon, 50);
                //Canvas.SetTop(hexagon.polygon, 50);
                MainCanvas.Children.Add(hexagon.polygon);
            }

            MainCanvas.BringIntoView(new Rect(HexCoordToCenterPoint(new Hex(0, 0), 10), HexCoordToCenterPoint(new Hex(25, 26), 10)));
        }

        private Point HexCoordToCenterPoint(Hex hex, double size)
        {
            double x = size * Math.Sqrt(3) * (hex.column + hex.row / 2);
            double y = size * 3 / 2 * hex.row;
            return new Point(x, y);
        }

        const double _scaleRate = 1.1;
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                scaleTransform.ScaleX *= _scaleRate;
                scaleTransform.ScaleY *= _scaleRate;
            }
            else
            {
                scaleTransform.ScaleX /= _scaleRate;
                scaleTransform.ScaleY /= _scaleRate;
            }
        }
    }
}
