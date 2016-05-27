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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using HiveLib.Models;
using HiveLib.Models.Pieces;


namespace HiveDisplay
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

        HexagonDrawing _centerHex = new HexagonDrawing(new Hex(24,24), 10); // x: 1247, y: 720 height: 40
        Dictionary<Polygon, Piece> _imageToPieceMap = new Dictionary<Polygon, Piece>();
        Board _board = Board.GetNewBoard();
        Dictionary<Piece, List<UIElement>> _tempUIElements = new Dictionary<Piece, List<UIElement>>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Add(_centerHex.polygon);

            _board.TryMakeMove(Move.GetMove(@"wG1 ."));
            _board.TryMakeMove(Move.GetMove(@"bS1 wG1-"));
            _board.TryMakeMove(Move.GetMove(@"wQ -wG1"));
            _board.TryMakeMove(Move.GetMove(@"bQ bS1/"));
            _board.TryMakeMove(Move.GetMove(@"wA1 \wG1"));
            _board.TryMakeMove(Move.GetMove(@"bQ /bS1"));
            _board.TryMakeMove(Move.GetMove(@"wA1 bQ/"));
            _board.TryMakeMove(Move.GetMove(@"bB1 bS1\"));
            _board.TryMakeMove(Move.GetMove(@"wS1 wQ\"));
            _board.TryMakeMove(Move.GetMove(@"bB1 wS1-"));
            _board.TryMakeMove(Move.GetMove(@"wB1 \wS1"));
            _board.TryMakeMove(Move.GetMove(@"bB2 bB1-"));
            foreach (var kvp in _board.playedPieces)
            {
                HexagonDrawing hexWithImage = new HexagonDrawing(kvp.Value, 10, kvp.Key);
                MainCanvas.Children.Add(hexWithImage.image);
                MainCanvas.Children.Add(hexWithImage.polygon);
                _imageToPieceMap[hexWithImage.polygon] = kvp.Key;
                hexWithImage.polygon.MouseLeftButtonDown += polygon_MouseLeftButtonDown;
            }
        }

        void polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RemoveAllFutureMoveDrawing();
            Piece piece = _imageToPieceMap[((Polygon)e.Source)];
            AddFutureMoveDrawing(piece);
        }

        void image_MouseLeave(object sender, MouseEventArgs e)
        {
            Piece piece = _imageToPieceMap[((Polygon)e.Source)];
            RemoveFutureMoveDrawing(piece);
        }

        void image_MouseEnter(object sender, MouseEventArgs e)
        {
            //((Image)e.Source).Visibility = System.Windows.Visibility.Hidden;
            Piece piece = _imageToPieceMap[((Polygon)e.Source)];
            AddFutureMoveDrawing(piece);
        }

        private void RemoveAllFutureMoveDrawing()
        {
            foreach (var kvp in _tempUIElements)
            {
                foreach (UIElement element in kvp.Value)
                {
                    MainCanvas.Children.Remove(element);
                }
            }
            _tempUIElements.Clear();
        }

        private void RemoveFutureMoveDrawing(Piece piece)
        {
            if (!_tempUIElements.ContainsKey(piece)) return;
            foreach (UIElement element in _tempUIElements[piece])
            {
                MainCanvas.Children.Remove(element);
            }
            _tempUIElements.Remove(piece);
        }

        private void AddFutureMoveDrawing(Piece piece)
        {

            List<UIElement> elementList = new List<UIElement>();
            foreach (Move move in _board.GenerateAllMovementMoves().Where(m => m.pieceToMove == piece))
            {
                FutureMoveDrawing hexWithImage = new FutureMoveDrawing(move.hex, 20);
                MainCanvas.Children.Add(hexWithImage.polygon);
                elementList.Add(hexWithImage.polygon);
            }
            _tempUIElements.Add(piece, elementList);
        }

        private void MovesTextBlock_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filename = files[0];

                using(StreamReader stream = new StreamReader(filename))
                {
                    string currentLine = stream.ReadLine();
                    Move move;
                    int turn = 1;
                    if (!Move.TryGetMove(currentLine, out move))
                    {
                        MovesListBox.Items.Add("Invalid file");
                    }
                    else
                    {
                        MovesListBox.Items.Add(string.Format("{0}: {1}", turn, currentLine));
                        currentLine = stream.ReadLine();
                        while (!stream.EndOfStream)
                        {
                            turn++;
                            currentLine = stream.ReadLine();
                            MovesListBox.Items.Add(string.Format("{0}: {1}", turn, currentLine));
                        }
                    }
                }
            }
        }

    }
}
