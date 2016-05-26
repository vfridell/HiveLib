using HiveLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using HiveLib.Models.Pieces;
using System.Windows.Media.Imaging;
using System.IO;
using Bitmap = System.Drawing.Bitmap;



namespace WpfApplication1
{
    public class FutureMoveDrawing : HexagonDrawing
    {
        public FutureMoveDrawing(Hex hex, double size)
            : base(hex, size)
        {
            Canvas.SetZIndex(this._polygon, 99);
            _polygon.Fill = Brushes.Tan;
        }
    }

    public class HexagonDrawing
    {
        public Polygon polygon { get { return _polygon; }  }
        public Image image { get { return _image; }  }
        public double height;
        public double width;

        protected Polygon _polygon;
        protected Image _image;
        public readonly Piece piece;
        public readonly Point center;

        public HexagonDrawing(Hex hex, double size)
        {
            _polygon = new Polygon();
            center = HexCoordToCenterPoint(hex, size);
            for(int i = 1; i<=6; i++)
            {
                _polygon.Points.Add(HexCorner(center, size, i));
            }
            _polygon.Stroke = Brushes.Black;
            _polygon.Fill = Brushes.Transparent;
            height = size * 2;
            width = Math.Sqrt(3) / 2 * height;
        }

        public HexagonDrawing(Hex hex, double size, Piece piece)
            : this(hex, size)
        {
            this.piece = piece;
            _image = PieceToImage(piece);
            Canvas.SetZIndex(_image, -1);
            Canvas.SetLeft(_image, center.X - 24);
            Canvas.SetTop(_image, center.Y - 24);
        }

        private Point HexCorner(Point center, double size, int corner_number)
        {
            double angle_deg = 60 * corner_number + 30;
            double angle_rad = Math.PI / 180 * angle_deg;
            return new Point(center.X + size * Math.Cos(angle_rad), center.Y + size * Math.Sin(angle_rad));
        }

        private Point HexCoordToCenterPoint(Hex hex, double size)
        {
            var basis = new Matrix(size * Math.Sqrt(3), size * Math.Sqrt(3) / 2, 0, size * 3 / 2, 0, 0);
            Matrix xy = Matrix.Multiply(basis, new Matrix(hex.column, 0, hex.row, 0, 0, 0));
            return new Point(xy.M11, xy.M21);
        }

        private Image PieceToImage(Piece piece)
        {
            string imagePath = string.Format(@"C:\Workroot\Projects\HiveLib\WpfApplication1\images\{0}.png", NotationParser.GetNotationForPiece(piece));
            var bitmap = new System.Drawing.Bitmap(imagePath);
            Image image = new Image();
            image.Source = BitmapToImageSource(bitmap);
            return image;
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
