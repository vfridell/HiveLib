using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models.Pieces;

namespace HiveLib.Models
{

    class Board
    {
        public static readonly int columns = 50;
        public static readonly int rows = 50;
        
        private List<Piece> unplayedPieces = new List<Piece>();
        private Dictionary<Piece, Hex> playedPieces = new Dictionary<Piece, Hex>();
        private Piece [,] boardArray = new Piece[columns, rows];

        internal bool whiteToPlay = true;
        internal bool whiteQueenPlaced { get; set; }
        internal bool blackQueenPlaced { get; set; }

        private Board() { }

        internal void PlaceFirstPiece(Piece piece)
        {
            throw new NotImplementedException();
        }

        internal void PlaceSecondPiece(Piece piece)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a move.  Validates the move or fails.
        /// </summary>
        /// <param name="move"></param>
        internal void MakeMove(Move move)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// For arbitrary setting up board.
        /// Doesn't validate board
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="hex"></param>
        internal void SetPiece(Piece piece, Hex hex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// For arbitrary clearing a hex.
        /// Doesn't validate board
        /// </summary>
        /// <param name="hex"></param>
        internal void ClearHex(Hex hex)
        {
            throw new NotImplementedException();
        }

        internal bool TryGetPiece(Hex hex, out Piece piece)
        {
            piece = boardArray[hex.column, hex.row];
            return (null == piece);
        }

        internal bool TryGetHex(Piece piece, out Hex hex)
        {
            // look for it non-stacked
            if (playedPieces.TryGetValue(piece, out hex))
            {
                return true;
            }
            else
            {
                // look in all the beetle stacks
                BeetleStack beetleStack = playedPieces.Keys
                                                      .OfType<BeetleStack>()
                                                      .FirstOrDefault(bs => bs.Contains(piece));
                if (null != beetleStack)
                {
                    hex = playedPieces[beetleStack];
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal Board Clone()
        {
            Board board = new Board();
            board.whiteQueenPlaced = this.whiteQueenPlaced;
            board.blackQueenPlaced = this.blackQueenPlaced;
            board.whiteToPlay = this.whiteToPlay;
            this.unplayedPieces.ForEach(p => board.unplayedPieces.Add(p));
            foreach (KeyValuePair<Piece, Hex> kvp in this.playedPieces)
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
                board.playedPieces.Add(p, kvp.Value);
                board.boardArray[kvp.Value.column, kvp.Value.row] = p;
            }
            return board;
        }

        internal static Board GetNewBoard()
        {
            Board board = new Board();
            board.unplayedPieces.Add(new QueenBee(Piece.PieceColor.White, 1));
            board.unplayedPieces.Add(new Beetle(Piece.PieceColor.White, 1));
            board.unplayedPieces.Add(new Beetle(Piece.PieceColor.White, 2));
            board.unplayedPieces.Add(new Spider(Piece.PieceColor.White, 1));
            board.unplayedPieces.Add(new Spider(Piece.PieceColor.White, 2));
            board.unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 1));
            board.unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 2));
            board.unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 3));
            board.unplayedPieces.Add(new Ant(Piece.PieceColor.White, 1));
            board.unplayedPieces.Add(new Ant(Piece.PieceColor.White, 2));
            board.unplayedPieces.Add(new Ant(Piece.PieceColor.White, 3));

            board.unplayedPieces.Add(new QueenBee(Piece.PieceColor.Black, 1));
            board.unplayedPieces.Add(new Beetle(Piece.PieceColor.Black, 1));
            board.unplayedPieces.Add(new Beetle(Piece.PieceColor.Black, 2));
            board.unplayedPieces.Add(new Spider(Piece.PieceColor.Black, 1));
            board.unplayedPieces.Add(new Spider(Piece.PieceColor.Black, 2));
            board.unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 1));
            board.unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 2));
            board.unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 3));
            board.unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 1));
            board.unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 2));
            board.unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 3));
            return board;
        }
    }
}
