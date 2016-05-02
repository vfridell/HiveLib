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
        
        internal readonly List<Piece> unplayedPieces = new List<Piece>();
        internal readonly Dictionary<Piece, Hex> playedPieces = new Dictionary<Piece, Hex>();
        internal readonly Piece [,] boardArray = new Piece[columns, rows];

        internal bool whiteToPlay = true;
        internal bool whiteQueenPlaced { get; set; }
        internal bool blackQueenPlaced { get; set; }

        internal Board() 
        {
            unplayedPieces.Add(new QueenBee(Piece.PieceColor.White, 1));
            unplayedPieces.Add(new Beetle(Piece.PieceColor.White, 1));
            unplayedPieces.Add(new Beetle(Piece.PieceColor.White, 2 ));
            unplayedPieces.Add(new Spider(Piece.PieceColor.White, 1));
            unplayedPieces.Add(new Spider(Piece.PieceColor.White, 2));
            unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 1));
            unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 2));
            unplayedPieces.Add(new Hopper(Piece.PieceColor.White, 3));
            unplayedPieces.Add(new Ant(Piece.PieceColor.White, 1));
            unplayedPieces.Add(new Ant(Piece.PieceColor.White, 2));
            unplayedPieces.Add(new Ant(Piece.PieceColor.White, 3));

            unplayedPieces.Add(new QueenBee(Piece.PieceColor.Black, 1));
            unplayedPieces.Add(new Beetle(Piece.PieceColor.Black, 1));
            unplayedPieces.Add(new Beetle(Piece.PieceColor.Black, 2));
            unplayedPieces.Add(new Spider(Piece.PieceColor.Black, 1));
            unplayedPieces.Add(new Spider(Piece.PieceColor.Black, 2));
            unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 1));
            unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 2));
            unplayedPieces.Add(new Hopper(Piece.PieceColor.Black, 3));
            unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 1));
            unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 2));
            unplayedPieces.Add(new Ant(Piece.PieceColor.Black, 3));
        }

        internal void PlaceFirstPiece(Piece piece)
        {
            throw new NotImplementedException();
        }

        internal void PlaceSecondPiece(Piece piece)
        {
            throw new NotImplementedException();
        }

        internal void MakeMove(Move move)
        {
            throw new NotImplementedException();
        }

        internal Piece GetPiece(Hex hex)
        {
            return boardArray[hex.column, hex.row];
        }

        internal Hex GetHex(Piece piece)
        {
            Hex hex;
            if (!playedPieces.TryGetValue(piece, out hex))
                return null;
            else
                return hex;
        }

    }
}
