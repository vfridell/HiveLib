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
        Dictionary<Piece, List<UIElement>> _tempUIElements = new Dictionary<Piece, List<UIElement>>();
        Board _currentBoard;
        Game _game;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Point canvasCenterPoint = new Point(MainCanvas.ActualWidth / 2, MainCanvas.ActualHeight / 2);
            HexagonDrawing.SetCenterPoint(canvasCenterPoint, _drawSize);
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
                foreach (var kvp in board.playedPieces)
                {
                    HexagonDrawing hexWithImage = HexagonDrawing.GetHexagonDrawing(kvp.Value, _drawSize, kvp.Key);
                    MainCanvas.Children.Add(hexWithImage.image);
                    MainCanvas.Children.Add(hexWithImage.polygon);
                    _imageToPieceMap[hexWithImage.polygon] = kvp.Key;
                    hexWithImage.polygon.MouseLeftButtonDown += polygon_MouseLeftButtonDown;
                }
            }
            catch (Exception ex)
            {
                throw;
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
            Piece piece = _imageToPieceMap[((Polygon)e.Source)];
            AddFutureMoveDrawing(piece);
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
                FutureMoveDrawing hexWithImage = FutureMoveDrawing.GetFutureMoveDrawing(move.hex, _drawSize);
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
                _game = Game.GetNewGame("player1", "player2");
                _currentBoard = null;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filename = files[0];

                // HERE deserialize bin file of Game and display details on canvas
                switch(System.IO.Path.GetExtension(filename))
                {
                    case ".txt":
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
