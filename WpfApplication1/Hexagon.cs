using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication1
{
    public class Hexagon
    {
        public Polygon polygon { get { return _polygon; }  }
        public double height;
        public double width;

        private Polygon _polygon;
        public readonly Point center;

        public Hexagon(Point center, double size)
        {
            _polygon = new Polygon();
            for(int i = 1; i<=6; i++)
            {
                _polygon.Points.Add(HexCorner(center, size, i));
            }
            _polygon.Stroke = Brushes.Black;
            height = size * 2;
            width = Math.Sqrt(3) / 2 * height;
        }

        private Point HexCorner(Point center, double size, int corner_number)
        {
            double angle_deg = 60 * corner_number + 30;
            double angle_rad = Math.PI / 180 * angle_deg;
            return new Point(center.X + size * Math.Cos(angle_rad), center.Y + size * Math.Sin(angle_rad));
        }


    }
}
