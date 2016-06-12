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



namespace HiveDisplay
{
    public class FutureMoveDrawing : HexagonDrawing
    {
        protected FutureMoveDrawing() { }
        public static FutureMoveDrawing GetFutureMoveDrawing(Hex hex, double size, Point offsetPoint)
        {
            HexagonDrawing hexDrawing = GetHexagonDrawing(hex, size, offsetPoint);
            FutureMoveDrawing drawing = new FutureMoveDrawing();
            drawing._center = hexDrawing.center;
            drawing._piece = hexDrawing.piece;
            drawing._polygon = hexDrawing.polygon;
            drawing._image = hexDrawing.image;
            drawing.height = hexDrawing.height;
            drawing.width = hexDrawing.width;
            Canvas.SetZIndex(drawing._polygon, 99);
            drawing._polygon.Fill = Brushes.Tan;
            return drawing;
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

        protected Piece _piece;
        public Piece piece { get { return _piece; } }

        protected Point _center;
        public Point center { get { return _center; } }

        protected HexagonDrawing() { }

        public static Point GetOffsetPointFromCenter(Point centerPoint, double size)
        {
            HexagonDrawing calculatedCenter = GetHexagonDrawing(new Hex(24, 24), size, new Point(0,0));
            return new Point(calculatedCenter._center.X - centerPoint.X, calculatedCenter._center.Y - centerPoint.Y);
        }

        public static HexagonDrawing GetHexagonDrawing(Hex hex, double size, Point offsetPoint)
        {
            HexagonDrawing drawing = new HexagonDrawing();
            drawing._polygon = new Polygon();
            drawing._center = HexCoordToCenterPoint(hex, size, offsetPoint);
            for (int i = 1; i <= 6; i++)
            {
                drawing._polygon.Points.Add(HexCorner(drawing.center, size, i));
            }
            drawing._polygon.Stroke = Brushes.Black;
            drawing._polygon.Fill = Brushes.Transparent;
            drawing.height = size * 2;
            drawing.width = Math.Sqrt(3) / 2 * drawing.height;
            return drawing;
        }

        public static HexagonDrawing GetHexagonDrawing(Hex hex, double size, Piece piece, Point offsetPoint)
        {
            HexagonDrawing drawing = GetHexagonDrawing(hex, size, offsetPoint);
            drawing._piece = piece;
            drawing._image = PieceToImage(piece);
            double imageXOffset = drawing._image.Source.Width / 2;
            double imageYOffset = drawing._image.Source.Height / 2;
            double scale = drawing.width / drawing._image.Source.Width + .25;
            drawing._image.RenderTransform = new ScaleTransform(scale, scale, imageXOffset, imageYOffset);
            Canvas.SetZIndex(drawing._image, -1);
            Canvas.SetLeft(drawing._image, drawing.center.X - imageXOffset );
            Canvas.SetTop(drawing._image, drawing.center.Y - imageYOffset);
            return drawing;
        }

        public static Point HexCorner(Point center, double size, int corner_number)
        {
            double angle_deg = 60 * corner_number + 30;
            double angle_rad = Math.PI / 180 * angle_deg;
            return new Point(center.X + size * Math.Cos(angle_rad), center.Y + size * Math.Sin(angle_rad));
        }

        public static Point HexCoordToCenterPoint(Hex hex, double size, Point offsetPoint)
        {
            double height = size * 2;
            double width = Math.Sqrt(3) / 2 * height;
            var basis = new Matrix(size * Math.Sqrt(3), size * Math.Sqrt(3) / 2, 0, size * 3 / 2, 0, 0);
            Matrix xy = Matrix.Multiply(basis, new Matrix(hex.column, 0, hex.row, 0, 0, 0));

            return new Point(xy.M11 - offsetPoint.X, xy.M21 - offsetPoint.Y);
        }

        public static Image PieceToImage(Piece piece)
        {
            string imagePath = string.Format(@"images\{0}.png", NotationParser.GetNotationForPiece(piece));
            var bitmap = new System.Drawing.Bitmap(imagePath);
            Image image = new Image();
            image.Source = BitmapToImageSource(bitmap);
            return image;
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
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
