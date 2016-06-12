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
using System.Runtime.Serialization.Formatters.Binary;


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

        double _drawSize = 30;
        Dictionary<Polygon, Piece> _imageToPieceMap = new Dictionary<Polygon, Piece>();
        Dictionary<Image, Piece> _unplayedImageToPieceMap = new Dictionary<Image, Piece>();
        Dictionary<Piece, List<UIElement>> _tempUIElements = new Dictionary<Piece, List<UIElement>>();
        Board _currentBoard;
        Game _game;
        Point _mainCanvasOffsetPoint;
        Point _unplayedCanvasOffsetPoint;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Point canvasCenterPoint = new Point(MainCanvas.ActualWidth / 2, MainCanvas.ActualHeight / 2);
            _mainCanvasOffsetPoint = HexagonDrawing.GetOffsetPointFromCenter(canvasCenterPoint, _drawSize);
            Point unplayedCanvasCenterPoint = new Point(UnplayedPiecesCanvas.ActualWidth / 2, UnplayedPiecesCanvas.ActualHeight / 2);
            _unplayedCanvasOffsetPoint = HexagonDrawing.GetOffsetPointFromCenter(unplayedCanvasCenterPoint, _drawSize);
            MovesListBox.SelectionChanged += MovesListBox_SelectionChanged;
        }

        void MovesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox movesListBox = (ListBox)e.Source;
            if (-1 != movesListBox.SelectedIndex)
            {
                string moveString = (string)movesListBox.Items[movesListBox.SelectedIndex];
                int boardNumber = GetListIndexFromMoveString(moveString);
                DrawBoard(_game.boards[boardNumber]);
                _currentBoard = _game.boards[boardNumber];
            }
        }

        int GetListIndexFromMoveString(string moveString)
        {
            string numString = moveString.Substring(0, moveString.IndexOf(':'));
            return int.Parse(numString);
        }

        void DrawBoard(Board board)
        {
            try
            {
                MainCanvas.Children.Clear();
                _imageToPieceMap.Clear();
                foreach (var kvp in board.playedPieces)
                {
                    HexagonDrawing hexWithImage = HexagonDrawing.GetHexagonDrawing(kvp.Value, _drawSize, kvp.Key, _mainCanvasOffsetPoint);
                    MainCanvas.Children.Add(hexWithImage.image);
                    MainCanvas.Children.Add(hexWithImage.polygon);
                    _imageToPieceMap[hexWithImage.polygon] = kvp.Key;
                    hexWithImage.polygon.MouseLeftButtonDown += polygon_MouseLeftButtonDown;
                }
                UnplayedPiecesCanvas.Children.Clear();
                _unplayedImageToPieceMap.Clear();
                HashSet<Tuple<PieceColor, string>> shown = new HashSet<Tuple<PieceColor, string>>();
                foreach (Piece unplayedPiece in board.unplayedPieces.OrderBy(p => p.number))
                {
                    var pieceTypeTuple = new Tuple<PieceColor, string>(unplayedPiece.color, unplayedPiece.GetPieceNotation());
                    if (shown.Contains(pieceTypeTuple)) continue;
                    AddUnplayedPieceToCanvas(unplayedPiece);
                    shown.Add(pieceTypeTuple);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AddUnplayedPieceToCanvas(Piece piece)
        {
            Hex hex = GetHexForUnplayedPiece(piece);
            HexagonDrawing hexWithImage = HexagonDrawing.GetHexagonDrawing(hex, _drawSize, piece, _unplayedCanvasOffsetPoint);
            UnplayedPiecesCanvas.Children.Add(hexWithImage.image);
            _unplayedImageToPieceMap[hexWithImage.image] = piece;
            hexWithImage.image.MouseLeftButtonDown += unplayedPieceImage_MouseLeftButtonDown;
        }

        private Hex GetHexForUnplayedPiece(Piece piece)
        {
            int column = 14;
            if (piece.color == PieceColor.Black) column = 24;
            switch (piece.GetPieceNotation())
            {
                case "Q":
                    column += 1;
                    break;
                case "A":
                    column += 2;
                    break;
                case "B":
                    column += 3;
                    break;
                case "G":
                    column += 4;
                    break;
                case "S":
                    column += 5;
                    break;
                default:
                    column += 6;
                    break;
            }
            return new Hex(column, 24);
        }

        void unplayedPieceImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RemoveAllFutureMoveDrawing();
            Piece piece = _unplayedImageToPieceMap[((Image)e.Source)];
            AddPlacementMoveDrawing(piece);
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
            Piece piece = _imageToPieceMap[((Polygon)e.Source)];
            AddFutureMoveDrawing(piece);
        }

        private void AddPlacementMoveDrawing(Piece piece)
        {
            List<Hex> hexes;
            _currentBoard.RefreshDependantBoardData();
            if(piece.color == PieceColor.White)
                hexes = _currentBoard.hivailableHexes.Where(kvp => kvp.Value.WhiteCanPlace).Select(kvp => kvp.Key).ToList();
            else
                hexes = _currentBoard.hivailableHexes.Where(kvp => kvp.Value.BlackCanPlace).Select(kvp => kvp.Key).ToList();

            List<UIElement> elementList = new List<UIElement>();
            foreach (Hex hex in hexes)
            {
                FutureMoveDrawing hexWithImage = FutureMoveDrawing.GetFutureMoveDrawing(hex, _drawSize, _mainCanvasOffsetPoint);
                MainCanvas.Children.Add(hexWithImage.polygon);
                elementList.Add(hexWithImage.polygon);
            }
            _tempUIElements.Add(piece, elementList);
        }

        public void RemoveAllFutureMoveDrawing()
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

        public void RemoveFutureMoveDrawing(Piece piece)
        {
            if (!_tempUIElements.ContainsKey(piece)) return;
            foreach (UIElement element in _tempUIElements[piece])
            {
                MainCanvas.Children.Remove(element);
            }
            _tempUIElements.Remove(piece);
        }

        public void AddFutureMoveDrawing(Piece piece)
        {

            List<UIElement> elementList = new List<UIElement>();
            foreach (Move move in _currentBoard.GenerateAllMovementMoves().Where(m => m.pieceToMove == piece))
            {
                FutureMoveDrawing hexWithImage = FutureMoveDrawing.GetFutureMoveDrawing(move.hex, _drawSize, _mainCanvasOffsetPoint);
                MainCanvas.Children.Add(hexWithImage.polygon);
                elementList.Add(hexWithImage.polygon);
            }
            _tempUIElements.Add(piece, elementList);
        }

        public bool TryMakeMove(Move move)
        {
            if (!_game.TryMakeMove(move))
            {
                MovesListBox.Items.Add("Invalid move");
                return false;
            }
            else
            {
                MovesListBox.Items.Add(string.Format("{0}: {1}", MovesListBox.Items.Count, move.notation));
                return true;
            }
        }

        private void MovesTextBlock_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                MovesListBox.Items.Clear();
                _currentBoard = null;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filename = files[0];

                switch(System.IO.Path.GetExtension(filename))
                {
                    case ".txt":
                        _game = Game.GetNewGame("player1", "player2");
                        TryLoadTextTranscript(filename);
                        break;
                    case ".bin":
                        TryLoadGameBinary(filename);
                        break;
                    default:
                        MovesListBox.Items.Add("Invalid file");
                        return;
                }

                DrawBoard(_game.boards[0]);
                _currentBoard = _game.boards[0];
            }
        }

        private bool TryLoadGameBinary(string filename)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                _game = (Game)formatter.Deserialize(stream);
            }
            foreach (Move move in _game.movesMade)
            {
                MovesListBox.Items.Add(string.Format("{0}: {1}", MovesListBox.Items.Count, move.notation));
            }
            return true;
        }

        private bool TryLoadTextTranscript(string filename)
        {
            using (StreamReader stream = new StreamReader(filename))
            {
                string currentLine = stream.ReadLine();
                while (!stream.EndOfStream)
                {
                    if (!TryAddMoveListItem(currentLine)) return false;
                    currentLine = stream.ReadLine();
                }
                if (!TryAddMoveListItem(currentLine)) return false;
            }
            return true;
        }

        private bool TryAddMoveListItem(string currentLine)
        {
            Move move;
            if (!Move.TryGetMove(currentLine, out move))
            {
                MovesListBox.Items.Add("Invalid file");
                return false;
            }
            else
            {
                return TryMakeMove(move);
            }
        }

    }
}
