using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using System.Threading.Tasks;
using HiveLib.Models.Pieces;
using PieceColor = HiveLib.Models.Pieces.Piece.PieceColor;
using QuickGraph.Algorithms.Search;
namespace HiveLib.Models
{

    class Board
    {
        public static readonly int columns = 50;
        public static readonly int rows = 50;
        public static Hex invalidHex = new Hex(-1, -1);

        private Dictionary<Hex, Hivailability> _hivailableHexes = new Dictionary<Hex, Hivailability>();
        private HashSet<Piece> _unplayedPieces = new HashSet<Piece>();
        private Dictionary<Piece, Hex> _playedPieces = new Dictionary<Piece, Hex>();
        private Piece [,] _boardPieceArray = new Piece[columns, rows];
        private List<Move> _moves = new List<Move>();
        private UndirectedGraph<Piece, UndirectedEdge<Piece>> _adjacencyGraph = new UndirectedGraph<Piece, UndirectedEdge<Piece>>();
        private HashSet<Piece> _articulationPoints = new HashSet<Piece>();
        private bool _movesDirty = true;

        internal bool whiteToPlay = true;
        internal bool whiteQueenPlaced = false;
        internal bool blackQueenPlaced = false;
        internal int turnNumber = 0;
        internal HashSet<Piece> articulationPoints { get { return _articulationPoints; } }

        private Board() { }

        private void GeneratePlacementMoves()
        {
            foreach (KeyValuePair<Hex, Hivailability> kvp in _hivailableHexes)
            {
                if (kvp.Value.BlackCanPlace && !whiteToPlay)
                {
                    Beetle beetle = _unplayedPieces.OfType<Beetle>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    Ant ant = _unplayedPieces.OfType<Ant>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    Spider spider = _unplayedPieces.OfType<Spider>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    QueenBee queenBee = _unplayedPieces.OfType<QueenBee>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    Hopper hopper = _unplayedPieces.OfType<Hopper>().Where(p => p.color == PieceColor.Black).FirstOrDefault();
                    if (null != beetle) _moves.Add(Move.GetMove(beetle, kvp.Key));
                    if (null != ant) _moves.Add(Move.GetMove(ant, kvp.Key));
                    if (null != spider) _moves.Add(Move.GetMove(spider, kvp.Key));
                    if (null != queenBee) _moves.Add(Move.GetMove(queenBee, kvp.Key));
                    if (null != hopper) _moves.Add(Move.GetMove(hopper, kvp.Key));
                }
                if (kvp.Value.WhiteCanPlace && whiteToPlay)
                {
                    Beetle beetle = _unplayedPieces.OfType<Beetle>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    Ant ant = _unplayedPieces.OfType<Ant>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    Spider spider = _unplayedPieces.OfType<Spider>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    QueenBee queenBee = _unplayedPieces.OfType<QueenBee>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    Hopper hopper = _unplayedPieces.OfType<Hopper>().Where(p => p.color == PieceColor.White).FirstOrDefault();
                    if (null != beetle) _moves.Add(Move.GetMove(beetle, kvp.Key));
                    if (null != ant) _moves.Add(Move.GetMove(ant, kvp.Key));
                    if (null != spider) _moves.Add(Move.GetMove(spider, kvp.Key));
                    if (null != queenBee) _moves.Add(Move.GetMove(queenBee, kvp.Key));
                    if (null != hopper) _moves.Add(Move.GetMove(hopper, kvp.Key));
                }
            }
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
                if (_playedPieces.Count > 0)
                {
                    Piece root = _playedPieces.Keys.First();
                    dfs.Compute(root);
                }
            }
        }

        private void GenerateMovementMoves()
        {
            foreach (var kvp in _playedPieces)
            {
                _moves.AddRange(kvp.Key.GetMoves(kvp.Value, this));
            }
        }

        internal IList<Move> GetMoves()
        {
            if (_movesDirty)
            {
                _moves.Clear();
                GeneratePlacementMoves();
                GenerateMovementMoves();
                _movesDirty = false;
            }
            return _moves;
        }

        /// <summary>
        /// Make a move.  Validates the move or fails.
        /// </summary>
        /// <param name="move"></param>
        internal bool TryMakeMove(Move move)
        {
            if (move.pieceToMove.color == PieceColor.White && !whiteToPlay) return false;
            if (move.pieceToMove.color == PieceColor.Black && whiteToPlay) return false;

            bool placement = true;
            Hex pieceToMoveHex = Board.invalidHex;
            if (!_unplayedPieces.Contains(move.pieceToMove))
            {
                // the target piece is already played on the board
                placement = false;
                if (!TryGetHexOfPlayedPiece(move.pieceToMove, out pieceToMoveHex)) return false;
            }

            if(move.hex.Equals(invalidHex))
            {
                Hex referencePieceHex;
                if (!TryGetHexOfPlayedPiece(move.referencePiece, out referencePieceHex)) return false;
                move.hex = Neighborhood.GetNeighborHex(referencePieceHex, move.targetPosition);
            }

            if (!GetMoves().Contains(move)) return false;

            if (placement)
            {
                Hivailability hivailability;
                if(!_hivailableHexes.TryGetValue(move.hex, out hivailability)) return false;
                if(!hivailability.CanPlace(move.pieceToMove.color)) return false;
                PlacePiece(move.pieceToMove, move.hex);
            }
            else
            {
                // check that movement doesn't violate the one-hive rule
                if (_articulationPoints.Contains(move.pieceToMove)) return false;
                MovePiece(move.pieceToMove, pieceToMoveHex, move.hex);
            }
            IncrementTurn();
            return true;
        }

        private void IncrementTurn()
        {
            whiteToPlay = !whiteToPlay;
            turnNumber++;
            // what else?
        }

        internal void MovePiece(Piece piece, Hex from, Hex to)
        {
            _movesDirty = true;
            _boardPieceArray[from.column, from.row] = null;
            _boardPieceArray[to.column, to.row] = null;
            _playedPieces.Remove(piece);
            _playedPieces.Add(piece, to);

            RefreshHivailability(piece, from, false);
            RefreshHivailability(piece, to, false);
            FindArticulationPoints();
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
            _hivailableHexes.Remove(hex);
            _unplayedPieces.Remove(piece);
            _playedPieces.Add(piece, hex);
            _boardPieceArray[hex.column, hex.row] = piece;

            //// update the data about the board
            //foreach (Hex directionHex in Neighborhood.neighborDirections)
            //{
            //    // don't do the center
            //    if(directionHex.Equals(hex)) continue;

            //    Piece adjacentPiece;
            //    Hex adjacentHex = hex + directionHex;
            //    if (!TryGetPieceAtHex(adjacentHex, out adjacentPiece))
            //    {
            //        // empty space, add/update hivailability
            //        _hivailableHexes.Remove(adjacentHex);
            //        _hivailableHexes.Add(adjacentHex, Hivailability.GetHivailability(this, adjacentHex, false, forceBlackCanPlace));
            //    }
            //    else
            //    {
            //        // contains a piece.  Update the adjacency graph
            //        _adjacencyGraph.AddVerticesAndEdge(new UndirectedEdge<Piece>(piece, adjacentPiece));
            //    }
            //}
            RefreshHivailability(piece, hex, forceBlackCanPlace);
            FindArticulationPoints();

            if (piece is QueenBee)
            {
                if (piece.color == PieceColor.White)
                    whiteQueenPlaced = true;
                else
                    blackQueenPlaced = true;
            }
        }

        private void RefreshHivailability(Piece piece, Hex hex, bool forceBlackCanPlace)
        {
            foreach (Hex directionHex in Neighborhood.neighborDirections)
            {
                // don't do the center
                if (directionHex.Equals(hex)) continue;

                Piece adjacentPiece;
                Hex adjacentHex = hex + directionHex;
                if (!TryGetPieceAtHex(adjacentHex, out adjacentPiece))
                {
                    // empty space, add/update hivailability
                    _hivailableHexes.Remove(adjacentHex);
                    _hivailableHexes.Add(adjacentHex, Hivailability.GetHivailability(this, adjacentHex, false, forceBlackCanPlace));
                }
                else
                {
                    // contains a piece.  Update the adjacency graph
                    _adjacencyGraph.AddVerticesAndEdge(new UndirectedEdge<Piece>(piece, adjacentPiece));
                }
            }
        }

        internal bool TryGetPieceAtHex(Hex hex, out Piece piece)
        {
            piece = _boardPieceArray[hex.column, hex.row];
            return (null != piece);
        }

        internal bool TryGetHexOfPlayedPiece(Piece piece, out Hex hex)
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

        internal bool PiecePlayed(Piece piece)
        {
            return !_unplayedPieces.Contains(piece);
        }

        internal Board Clone()
        {
            Board board = new Board();
            board.whiteQueenPlaced = this.whiteQueenPlaced;
            board.blackQueenPlaced = this.blackQueenPlaced;
            board.whiteToPlay = this.whiteToPlay;
            foreach (KeyValuePair<Hex, Hivailability> kvp in this._hivailableHexes)
            {
                board._hivailableHexes.Add(kvp.Key, kvp.Value);
            }
            foreach (Piece piece in this._articulationPoints) board._articulationPoints.Add(piece);
            foreach (Piece piece in this._unplayedPieces) board._unplayedPieces.Add(piece);
            foreach (KeyValuePair<Piece, Hex> kvp in this._playedPieces)
            {
                Piece p;
                if (kvp.Key is BeetleStack)
                {
                    p = ((BeetleStack)kvp.Key).Clone();
                }
                else
                {
                    p = kvp.Key;
                }
                board._playedPieces.Add(p, kvp.Value);
                board._boardPieceArray[kvp.Value.column, kvp.Value.row] = p;
            }
            return board;
        }

        internal static Board GetNewBoard()
        {
            Board board = new Board();
            board._unplayedPieces.Add(new QueenBee(Piece.PieceColor.White, 1));
            board._unplayedPieces.Add(new Beetle(Piece.PieceColor.White, 1));
            board._unplayedPieces.Add(new Beetle(Piece.PieceColor.White, 2));
            board._unplayedPieces.Add(new Spider(Piece.PieceColor.White, 1));
            board._unplayedPieces.Add(new Spider(Piece.PieceColor.White, 2));
            board._unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 1));
            board._unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 2));
            board._unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 3));
            board._unplayedPieces.Add(new Ant(Piece.PieceColor.White, 1));
            board._unplayedPieces.Add(new Ant(Piece.PieceColor.White, 2));
            board._unplayedPieces.Add(new Ant(Piece.PieceColor.White, 3));

            board._unplayedPieces.Add(new QueenBee(Piece.PieceColor.Black, 1));
            board._unplayedPieces.Add(new Beetle(Piece.PieceColor.Black, 1));
            board._unplayedPieces.Add(new Beetle(Piece.PieceColor.Black, 2));
            board._unplayedPieces.Add(new Spider(Piece.PieceColor.Black, 1));
            board._unplayedPieces.Add(new Spider(Piece.PieceColor.Black, 2));
            board._unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 1));
            board._unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 2));
            board._unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 3));
            board._unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 1));
            board._unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 2));
            board._unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 3));
            board._hivailableHexes.Add(new Hex(24, 24), Hivailability.GetHivailability(board, new Hex(24, 24), true));
            return board;
        }
    }
}
