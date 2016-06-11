using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using System.Threading.Tasks;
using HiveLib.Helpers;
using HiveLib.Models.Pieces;
using PieceColor = HiveLib.Models.Pieces.PieceColor;
using QuickGraph.Algorithms.Search;
using System.Runtime.Serialization;
namespace HiveLib.Models
{
    public enum GameResult { Incomplete, WhiteWin, BlackWin, Draw };

    [Serializable]
    public class Board : IDeserializationCallback
    {
        private Board() { }

        public static readonly int columns = 50;
        public static readonly int rows = 50;
        public static Hex invalidHex = new Hex(-1, -1);

        [NonSerialized]
        private bool _movesDirty = true;

        [NonSerialized]
        private Dictionary<Hex, Hivailability> _hivailableHexes = new Dictionary<Hex, Hivailability>();
        public IReadOnlyList<Hex> hivailableSpaces { get { return _hivailableHexes.Keys.ToList().AsReadOnly(); }}

        [NonSerialized]
        private List<Move> _moves = new List<Move>();
        [NonSerialized]
        private List<Move> _allMoves = new List<Move>();
        public IReadOnlyList<Move> AllMoves { get { GetMoves(); return _allMoves.AsReadOnly(); } }

        [NonSerialized]
        private UndirectedGraph<Piece, UndirectedEdge<Piece>> _adjacencyGraph = new UndirectedGraph<Piece, UndirectedEdge<Piece>>();
        
        [NonSerialized]
        private HashSet<Piece> _articulationPoints = new HashSet<Piece>();
        public ReadOnlySet<Piece> articulationPoints { get { return _articulationPoints.AsReadOnly(); } }

        // serialize this stuff
        private HashSet<Piece> _unplayedPieces = new HashSet<Piece>();
        public ReadOnlySet<Piece> unplayedPieces { get { return _unplayedPieces.AsReadOnly(); } }
        
        private Dictionary<Piece, Hex> _playedPieces = new Dictionary<Piece, Hex>();
        public Dictionary<Piece, Hex> playedPieces { get { return new Dictionary<Piece,Hex>(_playedPieces); } }

        private Piece [,] _boardPieceArray = new Piece[columns, rows];

        private GameResult _gameResult = GameResult.Incomplete;
        public GameResult gameResult { get { return _gameResult; } }

        private bool _whiteToPlay = true;
        public bool whiteToPlay { get { return _whiteToPlay; } }

        private bool _whiteQueenPlaced = false;
        public bool whiteQueenPlaced { get { return _whiteQueenPlaced; } }

        private bool _blackQueenPlaced = false;
        public bool blackQueenPlaced { get { return _blackQueenPlaced; } }

        private int _turnNumber = 1;
        public int turnNumber { get { return _turnNumber; } }

        private string _lastError;
        public string lastError { get { return _lastError; } }
        // end serialize this stuff

        public int BlackQueenBreathingSpaces() { return BreathingSpaces(new QueenBee(PieceColor.Black, 1)); }
        public int WhiteQueenBreathingSpaces() { return BreathingSpaces(new QueenBee(PieceColor.White, 1)); }

        public int possibleMoves { get { return GetMoves().Count; } }
        public int blackUnplayedPieces { get { return _unplayedPieces.Count(p => p.color == PieceColor.Black); } }
        public int whiteUnplayedPieces { get { return _unplayedPieces.Count(p => p.color == PieceColor.White); } }
        public int blackHivailableSpaces { get { return _hivailableHexes.Count(kvp => kvp.Value.BlackCanPlace); } }
        public int whiteHivailableSpaces { get { return _hivailableHexes.Count(kvp => kvp.Value.WhiteCanPlace); } }
        public int blackMoveablePieces { get { return AllMoves.GroupBy(m => NotationParser.GetNotationForPiece(m.pieceToMove)).Count(m => m.Key[0] == 'b'); } }
        public int whiteMoveablePieces { get { return AllMoves.GroupBy(m => NotationParser.GetNotationForPiece(m.pieceToMove)).Count(m => m.Key[0] == 'w'); } }

        public int blackOwnedBeetleStacks { get { return _playedPieces.Where(kvp => kvp.Key is BeetleStack && kvp.Key.color == PieceColor.Black).Count(); } }
        public int whiteOwnedBeetleStacks { get { return _playedPieces.Where(kvp => kvp.Key is BeetleStack && kvp.Key.color == PieceColor.White).Count(); } }

        public bool whiteCanMoveAnt 
        { 
            get 
            {
                return  CanMovePiece(new Ant(PieceColor.White, 1)) ||
                        CanMovePiece(new Ant(PieceColor.White, 2)) ||
                        CanMovePiece(new Ant(PieceColor.White, 3));
            } 
        }
        public bool blackCanMoveAnt
        {
            get
            {
                return CanMovePiece(new Ant(PieceColor.Black, 1)) ||
                        CanMovePiece(new Ant(PieceColor.Black, 2)) ||
                        CanMovePiece(new Ant(PieceColor.Black, 3));
            }
        }
        public bool blackCanMoveQueen { get { return CanMovePiece(new QueenBee(PieceColor.Black, 1)); } }
        public bool whiteCanMoveQueen { get { return CanMovePiece(new QueenBee(PieceColor.White, 1)); } }

        public bool CanMovePiece(Piece checkPiece)
        {
            Hex hex;
            if (TryGetHexOfPlayedPiece(checkPiece, out hex))
            {
                if (!_articulationPoints.Contains(checkPiece))
                {
                    Piece piece;
                    TryGetPieceAtHex(hex, out piece);
                    if (!(piece is BeetleStack)) return true;
                    return ((BeetleStack)piece).top.Equals(checkPiece);
                }
            }
            return false;
        }

        private void GeneratePlacementMoves()
        {
            foreach (KeyValuePair<Hex, Hivailability> kvp in _hivailableHexes)
            {
                if (kvp.Value.BlackCanPlace && !_whiteToPlay)
                {
                    // on turn 1 you cannot place a bee
                    if (!(turnNumber == 2))
                    {
                        QueenBee queenBee = _unplayedPieces.OfType<QueenBee>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                        if (null != queenBee) _moves.Add(Move.GetMove(queenBee, kvp.Key));
                        // on turn 4 you must place a bee if you haven't already
                        if (!blackQueenPlaced && turnNumber == 8) continue;
                    }

                    Beetle beetle = _unplayedPieces.OfType<Beetle>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    Ant ant = _unplayedPieces.OfType<Ant>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    Spider spider = _unplayedPieces.OfType<Spider>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    Hopper hopper = _unplayedPieces.OfType<Hopper>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    if (null != beetle) _moves.Add(Move.GetMove(beetle, kvp.Key));
                    if (null != ant) _moves.Add(Move.GetMove(ant, kvp.Key));
                    if (null != spider) _moves.Add(Move.GetMove(spider, kvp.Key));
                    if (null != hopper) _moves.Add(Move.GetMove(hopper, kvp.Key));
                }
                if (kvp.Value.WhiteCanPlace && _whiteToPlay)
                {
                    // on turn 1 you cannot place a bee
                    if (!(turnNumber == 1))
                    {
                        QueenBee queenBee = _unplayedPieces.OfType<QueenBee>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                        if (null != queenBee) _moves.Add(Move.GetMove(queenBee, kvp.Key));
                        // on turn 4 you must place a bee if you haven't already
                        if (!whiteQueenPlaced && turnNumber == 7) continue;
                    }

                    Beetle beetle = _unplayedPieces.OfType<Beetle>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    Ant ant = _unplayedPieces.OfType<Ant>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    Spider spider = _unplayedPieces.OfType<Spider>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    Hopper hopper = _unplayedPieces.OfType<Hopper>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    if (null != beetle) _moves.Add(Move.GetMove(beetle, kvp.Key));
                    if (null != ant) _moves.Add(Move.GetMove(ant, kvp.Key));
                    if (null != spider) _moves.Add(Move.GetMove(spider, kvp.Key));
                    if (null != hopper) _moves.Add(Move.GetMove(hopper, kvp.Key));
                }
            }
        }

        private void RefreshDependantBoardData(bool forceBlackCanPlace = false)
        {
            RefreshAllHivailability(forceBlackCanPlace);
            RefreshAdjacencyList();
            FindArticulationPoints();
        }

        /// <summary>
        /// An articulation point is a hex with a piece whose removal
        /// causes a violation of the one-hive rule.
        /// </summary>
        private void FindArticulationPoints()
        {
            _articulationPoints.Clear();
            _adjacencyGraph.RemoveEdgeIf(e => e.Source == e.Target);
            var dfs = new UndirectedDepthFirstSearchAlgorithm<Piece, UndirectedEdge<Piece>>(_adjacencyGraph);
            var articulationPointObserver = new HiveLib.Helpers.UndirectedArticulationPointObserver<Piece, UndirectedEdge<Piece>>(_articulationPoints);

            using (articulationPointObserver.Attach(dfs))
            {
                if (_playedPieces.Count > 2)
                {
                    Piece root = _playedPieces.Keys.First();
                    dfs.Compute(root);
                }
            }
        }

        private void RefreshAllHivailability(bool forceBlackCanPlace = false)
        {
            _hivailableHexes.Clear();
            foreach(Hex hex in _playedPieces.Values)
            {
                foreach (Hex directionHex in Neighborhood.neighborDirections)
                {
                    // don't do the center
                    if (directionHex.Equals(new Hex(0, 0))) continue;

                    Hex adjacentHex = hex + directionHex;
                    Piece adjacentPiece;
                    if (!TryGetPieceAtHex(adjacentHex, out adjacentPiece))
                    {
                        // empty space, add/update hivailability
                        if (!_hivailableHexes.ContainsKey(adjacentHex))
                        {
                            _hivailableHexes.Add(adjacentHex, Hivailability.GetHivailability(this, adjacentHex, false, forceBlackCanPlace));
                        }
                    }
                }
            }
        }

        private void RefreshAdjacencyList()
        {
            _adjacencyGraph.Clear();
            var finishedVertices = new HashSet<Piece>();
            foreach (KeyValuePair<Piece, Hex> kvp in _playedPieces)
            {
                foreach (Hex directionHex in Neighborhood.neighborDirections)
                {
                    // don't do the center
                    if (directionHex.Equals(new Hex(0, 0))) continue;

                    Hex adjacentHex = kvp.Value + directionHex;
                    Piece adjacentPiece;
                    if (TryGetPieceAtHex(adjacentHex, out adjacentPiece))
                    {
                        if (!finishedVertices.Contains(adjacentPiece))
                        {
                            _adjacencyGraph.AddVerticesAndEdge(new UndirectedEdge<Piece>(kvp.Key, adjacentPiece));
                        }
                    }
                }
                finishedVertices.Add(kvp.Key);
            }
        }

        private void GenerateMovementMoves()
        {
            PieceColor colorToMove = _whiteToPlay ? PieceColor.White : PieceColor.Black;
            if ( (_whiteToPlay && whiteQueenPlaced) || (!_whiteToPlay && blackQueenPlaced))
            {
                foreach (Move move in GenerateAllMovementMoves())
                {
                    _allMoves.Add(move);
                    if(move.pieceToMove.color == colorToMove) _moves.Add(move);
                }
            }
        }

        public IReadOnlyList<Move> GenerateAllMovementMoves()
        {
            List<Move> moves = new List<Move>();
            if ((_whiteToPlay && whiteQueenPlaced) || (!_whiteToPlay && blackQueenPlaced))
            {
                foreach (var kvp in _playedPieces)
                {
                    if (!_articulationPoints.Contains(kvp.Key) || kvp.Key is BeetleStack)
                    {
                        moves.AddRange(kvp.Key.GetMoves(kvp.Value, this));
                    }
                }
            }
            return moves.AsReadOnly();
        }

        public IReadOnlyList<Move> GetMoves()
        {
            if (_gameResult != GameResult.Incomplete) return new List<Move>();
            if (_movesDirty)
            {
                _allMoves.Clear();
                _moves.Clear();
                GeneratePlacementMoves();
                GenerateMovementMoves();
                _movesDirty = false;
            }
            return _moves.AsReadOnly();
        }

        /// <summary>
        /// Make a move.  Validates the move or fails.
        /// </summary>
        /// <param name="move"></param>
        public bool TryMakeMove(Move move)
        {
            if (_gameResult != GameResult.Incomplete) { _lastError = "This game is over"; return false; }
            if (move.pieceToMove.color == PieceColor.White && !_whiteToPlay) { _lastError = "Cannot move a white piece on black's turn"; return false; }
            if (move.pieceToMove.color == PieceColor.Black && _whiteToPlay) { _lastError = "Cannot move a black piece on white's turn"; return false; }

            bool placement = true;
            Hex pieceToMoveHex = Board.invalidHex;
            Piece actualPiece = move.pieceToMove;
            if (!_unplayedPieces.Contains(move.pieceToMove))
            {
                // the target piece is already played on the board
                placement = false;
                if (!TryGetHexOfPlayedPiece(move.pieceToMove, out pieceToMoveHex)) { _lastError = "Piece is played but Board can't find it"; return false; }
                if (!TryGetPieceAtHex(pieceToMoveHex, out actualPiece)) { _lastError = "Piece is played but Board can't find it, again"; return false; }
            }

            if(move.hex.Equals(invalidHex))
            {
                Hex referencePieceHex;
                if (!TryGetHexOfPlayedPiece(move.referencePiece, out referencePieceHex)) { _lastError = "Cannot find reference piece on the board"; return false; }
                move = Move.GetMove(move, Neighborhood.GetNeighborHex(referencePieceHex, move.targetPosition));
            }

            if (!GetMoves().Contains(move)) { _lastError = "Not a valid move"; return false; };

            if (placement)
            {
                Hivailability hivailability;
                if (!_hivailableHexes.TryGetValue(move.hex, out hivailability)) { _lastError = "Target hex is not hivailable"; return false; }
                if (!hivailability.CanPlace(actualPiece.color)) { _lastError = "Target hivailable hex is not playable by your color"; return false; }
                PlacePiece(actualPiece, move.hex);
            }
            else
            {
                // check that movement doesn't violate the one-hive rule
                if (_articulationPoints.Contains(actualPiece) && !(actualPiece is BeetleStack)) { _lastError = "Move violates one-hive rule"; return false; }
                MovePiece(actualPiece, pieceToMoveHex, move.hex);
            }
            IncrementTurn();
            return true;
        }

        private void IncrementTurn()
        {
            int blackBreathingSpaces = BlackQueenBreathingSpaces();
            int whiteBreathingSpaces = WhiteQueenBreathingSpaces();
            bool gameOver = (blackBreathingSpaces * whiteBreathingSpaces == 0);
            bool draw = (blackBreathingSpaces + whiteBreathingSpaces == 0);
            if (gameOver)
            {
                if (draw)
                {
                    _gameResult = GameResult.Draw;
                }
                else if (blackBreathingSpaces == 0)
                {
                    _gameResult = GameResult.WhiteWin;
                }
                else
                {
                    _gameResult = GameResult.BlackWin;
                }
            }
            else
            {
                _gameResult = GameResult.Incomplete;
                _whiteToPlay = !_whiteToPlay;
                _turnNumber++;
            }

            // check for automatic pass
            if(GetMoves().Count == 0)
            {
                _movesDirty = true;
                _whiteToPlay = !_whiteToPlay;
                _turnNumber++;
            }
        }

        /// <summary>
        /// Move a piece as opposed to placing it.  Does not validate the move.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        internal void MovePiece(Piece piece, Hex from, Hex to)
        {
            _movesDirty = true;
            if (piece is Beetle || piece is BeetleStack)
            {
                DispatchBeetleMove(piece, from, to);
            }
            else
            {
                // just a regular move
                _boardPieceArray[from.column, from.row] = null;
                _boardPieceArray[to.column, to.row] = piece;
                _playedPieces.Remove(piece);
                _playedPieces.Add(piece, to);
            }

            RefreshDependantBoardData();
        }

        private void DispatchBeetleMove(Piece piece, Hex from, Hex to)
        {
            Piece actualPiece = piece;
            if (actualPiece is BeetleStack)
            {
                actualPiece = HandleBeetleFromStackMove((BeetleStack)piece, from, to);
            }
            else
            {
                _boardPieceArray[from.column, from.row] = null;
                _playedPieces.Remove(actualPiece);
            }

            Piece referencePiece;
            if (TryGetPieceAtHex(to, out referencePiece))
            {
                HandleBeetleToStackMove((Beetle)actualPiece, referencePiece, from, to);
            }
            else
            {
                // beetle moving down
                HandleBeetleToEmptySpaceMove(actualPiece, from, to);
            }
        }

        private void HandleBeetleToEmptySpaceMove(Piece beetle, Hex from, Hex to)
        {
            // just a regular move to
            _boardPieceArray[to.column, to.row] = beetle;
            _playedPieces.Add(beetle, to);
        }

        private Piece HandleBeetleFromStackMove(BeetleStack stack, Hex from, Hex to)
        {
            Piece actualPiece = stack.top;
            Piece newPiece = BeetleStack.PopBeetleStack(stack);
            _boardPieceArray[from.column, from.row] = newPiece;
            _playedPieces.Remove(stack);
            _playedPieces.Add(newPiece, from);
            return actualPiece;
        }

        private void HandleBeetleToStackMove(Beetle movingPiece, Piece referencePiece, Hex from, Hex to)
        {
            // beetle climbing
            BeetleStack newStack;
            if (referencePiece is BeetleStack)
            {
                newStack = new BeetleStack(movingPiece, (BeetleStack)referencePiece);
            }
            else
            {
                // new beetle stack
                newStack = new BeetleStack(referencePiece, movingPiece);
            }
            _playedPieces.Remove(referencePiece);
            _boardPieceArray[to.column, to.row] = newStack;
            _playedPieces.Add(newStack, to);
        }

        /// <summary>
        /// Place a piece (as opposed to moving one)
        /// Assumes all validation is already done, or that you don't care about validity.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="hex"></param>
        internal void PlacePiece(Piece piece, Hex hex)
        {
            // if this is the first placement on the board
            bool forceBlackCanPlace = (_playedPieces.Count == 0);

            _movesDirty = true;
            _unplayedPieces.Remove(piece);
            _playedPieces.Add(piece, hex);
            _boardPieceArray[hex.column, hex.row] = piece;

            RefreshDependantBoardData(forceBlackCanPlace);

            if (piece is QueenBee)
            {
                if (piece.color == PieceColor.White)
                    _whiteQueenPlaced = true;
                else
                    _blackQueenPlaced = true;
            }
        }

        public bool TryGetPieceAtHex(Hex hex, out Piece piece)
        {
            piece = _boardPieceArray[hex.column, hex.row];
            return (null != piece);
        }

        public bool TryGetHexOfPlayedPiece(Piece piece, out Hex hex)
        {
            // look for it non-stacked
            if (_playedPieces.TryGetValue(piece, out hex))
            {
                return true;
            }
            else
            {
                // look in all the beetle stacks
                BeetleStack beetleStack = _playedPieces.Keys
                                                      .OfType<BeetleStack>()
                                                      .FirstOrDefault(bs => bs.Contains(piece));
                if (null != beetleStack)
                {
                    hex = _playedPieces[beetleStack];
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool PiecePlayed(Piece piece)
        {
            return !_unplayedPieces.Contains(piece);
        }


        /// <summary>
        /// returns the number of empty neighbors for a piece on the board
        /// returns .int.MaxValue if the given piece is unplayed
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public int BreathingSpaces(Piece piece)
        {
            Hex hex;
            if (TryGetHexOfPlayedPiece(piece, out hex))
            {
                var hivailability = Hivailability.GetHivailability(this, hex);
                return hivailability.EmptyNeighborHexes(hex).Count;
            }
            else
            {
                return 6;
            }
        }

        public Board Clone()
        {
            Board board = new Board();
            board._whiteQueenPlaced = this.whiteQueenPlaced;
            board._blackQueenPlaced = this.blackQueenPlaced;
            board._whiteToPlay = this._whiteToPlay;
            board._turnNumber = this.turnNumber;
            foreach (KeyValuePair<Hex, Hivailability> kvp in this._hivailableHexes)
            {
                board._hivailableHexes.Add(kvp.Key, kvp.Value);
            }
            foreach (Piece piece in this._articulationPoints)
            {
                board._articulationPoints.Add(piece);
            }
            board._adjacencyGraph.AddVerticesAndEdgeRange(this._adjacencyGraph.Edges);
            board._unplayedPieces.Clear();
            foreach (Piece piece in this._unplayedPieces) board._unplayedPieces.Add(piece);
            foreach (KeyValuePair<Piece, Hex> kvp in this._playedPieces)
            {
                board._playedPieces.Add(kvp.Key, kvp.Value);
                board._boardPieceArray[kvp.Value.column, kvp.Value.row] = kvp.Key;
            }
            return board;
        }

        internal Board CloneAndRemove(Piece piece)
        {
            Board board = this.Clone();
            Hex hex;
            if(!board.TryGetHexOfPlayedPiece(piece, out hex)) throw new Exception("piece is not played");
            board._playedPieces.Remove(piece);
            board._boardPieceArray[hex.column, hex.row] = null;
            return board;
        }

        public static bool BoardPositionsEqual(Board x, Board y)
        {
            if (x.playedPieces.Count != y.playedPieces.Count) return false;
            foreach (var kvp in x.playedPieces)
            {
                if (!y._boardPieceArray[kvp.Value.column, kvp.Value.row].Equals(kvp.Key)) return false;
            }
            return true;
        }

        public static Board GetNewBoard()
        {
            Board board = new Board();
            board._unplayedPieces.Add(new QueenBee(PieceColor.White, 1));
            board._unplayedPieces.Add(new Beetle(PieceColor.White, 1));
            board._unplayedPieces.Add(new Beetle(PieceColor.White, 2));
            board._unplayedPieces.Add(new Spider(PieceColor.White, 1));
            board._unplayedPieces.Add(new Spider(PieceColor.White, 2));
            board._unplayedPieces.Add(new Hopper(PieceColor.White, 1));
            board._unplayedPieces.Add(new Hopper(PieceColor.White, 2));
            board._unplayedPieces.Add(new Hopper(PieceColor.White, 3));
            board._unplayedPieces.Add(new Ant(PieceColor.White, 1));
            board._unplayedPieces.Add(new Ant(PieceColor.White, 2));
            board._unplayedPieces.Add(new Ant(PieceColor.White, 3));

            board._unplayedPieces.Add(new QueenBee(PieceColor.Black, 1));
            board._unplayedPieces.Add(new Beetle(PieceColor.Black, 1));
            board._unplayedPieces.Add(new Beetle(PieceColor.Black, 2));
            board._unplayedPieces.Add(new Spider(PieceColor.Black, 1));
            board._unplayedPieces.Add(new Spider(PieceColor.Black, 2));
            board._unplayedPieces.Add(new Hopper(PieceColor.Black, 1));
            board._unplayedPieces.Add(new Hopper(PieceColor.Black, 2));
            board._unplayedPieces.Add(new Hopper(PieceColor.Black, 3));
            board._unplayedPieces.Add(new Ant(PieceColor.Black, 1));
            board._unplayedPieces.Add(new Ant(PieceColor.Black, 2));
            board._unplayedPieces.Add(new Ant(PieceColor.Black, 3));
            board._hivailableHexes.Add(new Hex(24, 24), Hivailability.GetHivailability(board, new Hex(24, 24), true));
            return board;
        }

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            _movesDirty = true;
            _moves = new List<Move>();
            _allMoves = new List<Move>();
            _hivailableHexes = new Dictionary<Hex, Hivailability>();
            _adjacencyGraph = new UndirectedGraph<Piece, UndirectedEdge<Piece>>();
            _articulationPoints = new HashSet<Piece>();
            RefreshDependantBoardData();
        }

        #endregion
    }
}
