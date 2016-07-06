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
using HiveLib.ViewModels;
using HiveLib.AI;
using System.Threading;


namespace HiveDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class HiveGameWindow : Window
    {
        public HiveGameWindow()
        {
            InitializeComponent();
        }

        double _drawSize = 30;
        Dictionary<Polygon, Piece> _imageToPieceMap = new Dictionary<Polygon, Piece>();
        Dictionary<Image, Piece> _unplayedImageToPieceMap = new Dictionary<Image, Piece>();
        Dictionary<Piece, List<UIElement>> _tempUIElements = new Dictionary<Piece, List<UIElement>>();
        Dictionary<UIElement, Hex> _moveToUIElementHexes = new Dictionary<UIElement, Hex>();
        Board _currentBoard;
        Game _game;
        Point _mainCanvasOffsetPoint;
        Point _unplayedCanvasOffsetPoint;
        Piece _selectedPiece;
        CancellationTokenSource _cancelSource;
        static readonly object _aiLoopLock = new object();

        DisplayState _displayState;
        IHiveAI _player1AI;
        IHiveAI _player2AI;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Point canvasCenterPoint = new Point(MainCanvas.ActualWidth / 2, MainCanvas.ActualHeight / 2);
            _mainCanvasOffsetPoint = HexagonDrawing.GetOffsetPointFromCenter(canvasCenterPoint, _drawSize);
            Point unplayedCanvasCenterPoint = new Point(UnplayedPiecesCanvas.ActualWidth / 2, UnplayedPiecesCanvas.ActualHeight / 2);
            _unplayedCanvasOffsetPoint = HexagonDrawing.GetOffsetPointFromCenter(unplayedCanvasCenterPoint, _drawSize);
            MovesListBox.SelectionChanged += MovesListBox_SelectionChanged;
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewGameExecute, NewGameCanExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Stop, StopExecute, StopCanExecute));
        }

        private void StopCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StopExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CancelAIMove();
        }

        private void NewGameExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var newGameWindow = new NewGameWindow();
            newGameWindow.ShowDialog();
            if (newGameWindow.DialogResult.HasValue && newGameWindow.DialogResult.Value)
            {
                ClearDisplay();
                if (newGameWindow.Parameters.player1 != null)
                {
                    _player1AI = newGameWindow.Parameters.player1;
                    _player1AI.BeginNewGame(true);
                }
                else
                {
                    _player1AI = null;
                }

                if (newGameWindow.Parameters.player2 != null)
                {
                    _player2AI = newGameWindow.Parameters.player2;
                    _player2AI.BeginNewGame(false);
                }
                else
                {
                    _player2AI = null;
                }

                _game = Game.GetNewGame(_player1AI == null ? newGameWindow.Parameters.player1Name : _player1AI.Name, 
                                        _player2AI == null ? newGameWindow.Parameters.player2Name : _player2AI.Name );
                _currentBoard = _game.GetCurrentBoard();
                DrawBoard(_currentBoard);

                if (_player1AI != null && _player2AI != null)
                {
                    RunAIOnlyGame();
                }
                else
                {
                    MakeAIMoveIfNecessary();
                }
            }
        }

        private async void RunAIOnlyGame()
        {
            while (_game.gameResult == GameResult.Incomplete && !_game.ThreeFoldRepetition())
            {
                IHiveAI AI;
                if (TryGetAIForTurn(out AI))
                {
                    try
                    {
                        using (_cancelSource = new CancellationTokenSource())
                        {
                            SetUIStatus(false);
                            Move move = AI.PickBestMoveAsync(_game.GetCurrentBoard(), _cancelSource.Token).Result;
                            if (null == move) return;
                            if (!TryMakeMove(move))
                            {
                                throw new Exception("Bad AI move!");
                            }
                            _currentBoard = _game.GetCurrentBoard();
                            DrawBoard(_currentBoard);
                            await Task.Delay(1000);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        MessageBox.Show("Move Cancelled");
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        SetUIStatus(true);
                    }
                }
            }
        }

        private void CancelAIMove()
        {
            if(_cancelSource != null)
            {
                _cancelSource.Cancel();
            }
        }

        private async void MakeAIMoveIfNecessary()
        {
            IHiveAI AI;
            if (TryGetAIForTurn(out AI))
            {
                try
                {
                    SetUIStatus(false);
                    Move move;
                    using (_cancelSource = new CancellationTokenSource())
                    {
                        move = await AI.PickBestMoveAsync(_game.GetCurrentBoard(), _cancelSource.Token);
                    }
                    if (null == move) return;
                    if (!TryMakeMove(move))
                    {
                        throw new Exception("Bad AI move!");
                    }
                    _currentBoard = _game.GetCurrentBoard();
                    DrawBoard(_currentBoard);
                }
                catch(OperationCanceledException)
                {
                    MessageBox.Show("Move Cancelled");
                }
                catch(Exception ex)
                {
                    throw;
                }
                finally
                {
                    SetUIStatus(true);
                }

            }
        }

        private void SetUIStatus(bool enabled)
        {
            MainCanvas.IsEnabled = enabled;
            UnplayedPiecesCanvas.IsEnabled = enabled;
            MovesListBox.IsEnabled = enabled;

            if(!enabled)
            {
                //Mouse.OverrideCursor = Cursors.Wait;
            }
            else
            {
                //Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        void NewGameCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
                DrawGameInfo(board);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void DrawGameInfo(Board board)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("White: '{0}' vs. Black: '{1}'\n", _game.whitePlayerName, _game.blackPlayerName);
            sb.AppendFormat("{0}\n", board.whiteToPlay ? "White Turn" : "Black Turn");
            if(board.hivailableHexes.Count == 0) board.RefreshDependantBoardData();
            var analysisData = BoardAnalysisData.GetBoardAnalysisData(board, BoardAnalysisWeights.winningWeights);
            sb.AppendFormat("Advantage: {0}\n", analysisData.whiteAdvantage);
            sb.Append(analysisData.ToString());

            TextBlock textBlock = new TextBlock();
            textBlock.Text = sb.ToString();
            textBlock.Foreground = Brushes.Black;
            Canvas.SetLeft(textBlock, 5);
            Canvas.SetTop(textBlock, 5);
            MainCanvas.Children.Add(textBlock);
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
            _selectedPiece = piece;
            AddPlacementMoveDrawing(piece);
        }

        void polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RemoveAllFutureMoveDrawing();
            Piece piece = _imageToPieceMap[((Polygon)e.Source)];
            if (piece is BeetleStack) piece = new Beetle(piece.color, piece.number);
            _selectedPiece = piece;
            AddFutureMoveDrawing(piece);
        }

        private void AddPlacementMoveDrawing(Piece piece)
        {
            List<Hex> hexes;
            if(_currentBoard.hivailableHexes.Count == 0) _currentBoard.RefreshDependantBoardData();
            if(piece.color == PieceColor.White)
                hexes = _currentBoard.hivailableHexes.Where(kvp => kvp.Value.WhiteCanPlace).Select(kvp => kvp.Key).ToList();
            else
                hexes = _currentBoard.hivailableHexes.Where(kvp => kvp.Value.BlackCanPlace).Select(kvp => kvp.Key).ToList();

            List<UIElement> elementList = new List<UIElement>();
            _moveToUIElementHexes.Clear();
            foreach (Hex hex in hexes)
            {
                FutureMoveDrawing hexWithImage = FutureMoveDrawing.GetFutureMoveDrawing(hex, _drawSize, _mainCanvasOffsetPoint);
                MainCanvas.Children.Add(hexWithImage.polygon);
                elementList.Add(hexWithImage.polygon);
                hexWithImage.polygon.MouseLeftButtonDown += makeMovepolygon_MouseLeftButtonDown;
                _moveToUIElementHexes[hexWithImage.polygon] = hex;
            }
            _tempUIElements.Add(piece, elementList);
        }

        void makeMovepolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hex hex = _moveToUIElementHexes[((Polygon)e.Source)];
            Move move = Move.GetMove(_selectedPiece, hex);
            if (!TryMakeMove(move))
            {
                MessageBox.Show("Invalid Move", "Error", MessageBoxButton.OK);
            }
            else
            {
                DrawBoard(_game.GetCurrentBoard());
                _currentBoard = _game.GetCurrentBoard();
                MakeAIMoveIfNecessary();
            }
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
            _moveToUIElementHexes.Clear();
            foreach (Move move in _currentBoard.GetMoves(false).Where(m => m.pieceToMove.Equals(piece)))
            {
                FutureMoveDrawing hexWithImage = FutureMoveDrawing.GetFutureMoveDrawing(move.hex, _drawSize, _mainCanvasOffsetPoint);
                MainCanvas.Children.Add(hexWithImage.polygon);
                elementList.Add(hexWithImage.polygon);
                hexWithImage.polygon.MouseLeftButtonDown += makeMovepolygon_MouseLeftButtonDown;
                _moveToUIElementHexes[hexWithImage.polygon] = move.hex;
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
                ClearDisplay();
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

        private void ClearDisplay()
        {
            MovesListBox.Items.Clear();
            _currentBoard = null;
            _game = null;
            MainCanvas.Children.Clear();
            _imageToPieceMap.Clear();
            _unplayedImageToPieceMap.Clear();
            _tempUIElements.Clear();
            _player1AI = null;
            _player2AI = null;
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

        internal bool TryGetAIForTurn(out IHiveAI ai)
        {
            ai = null;
            if (_game.GetCurrentBoard().gameResult == GameResult.Incomplete && !_game.ThreeFoldRepetition())
            {
                if (_currentBoard.whiteToPlay)
                {
                    if (_player1AI != null && _player1AI.playingWhite) { ai = _player1AI; return true; }
                    else if (_player2AI != null && _player2AI.playingWhite) { ai = _player2AI; return true; }
                    else { return false; }
                }
                else
                {
                    if (_player1AI != null && !_player1AI.playingWhite) { ai = _player1AI; return true; }
                    else if (_player2AI != null && !_player2AI.playingWhite) { ai = _player2AI; return true; }
                    else { return false; }
                }
            }
            else
            {
                return false;
            }
        }

    }
}
