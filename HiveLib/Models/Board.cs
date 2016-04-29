using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models.Pieces;

namespace HiveLib.Models
{

    class Board
    {
        public static readonly int rows = 50;
        public static readonly int columns = 50;
        
        internal readonly List<Piece> unplayedPieces = new List<Piece>();
        internal readonly Piece [,] boardArray = new Piece[rows,columns];

        public Board() 
        {
            unplayedPieces.Add(new Bee { color = Piece.PieceColor.White, number = 1 });
            unplayedPieces.Add(new Beetle { color = Piece.PieceColor.White, number = 1 });
            unplayedPieces.Add(new Beetle { color = Piece.PieceColor.White, number = 2 });
            unplayedPieces.Add(new Spider { color = Piece.PieceColor.White, number = 1 });
            unplayedPieces.Add(new Spider { color = Piece.PieceColor.White, number = 2 });
            unplayedPieces.Add(new Hopper { color = Piece.PieceColor.White, number = 1 });
            unplayedPieces.Add(new Hopper { color = Piece.PieceColor.White, number = 2 });
            unplayedPieces.Add(new Hopper { color = Piece.PieceColor.White, number = 3 });
            unplayedPieces.Add(new Ant { color = Piece.PieceColor.White, number = 1 });
            unplayedPieces.Add(new Ant { color = Piece.PieceColor.White, number = 2 });
            unplayedPieces.Add(new Ant { color = Piece.PieceColor.White, number = 3 });

            unplayedPieces.Add(new Bee { color = Piece.PieceColor.Black, number = 1 });
            unplayedPieces.Add(new Beetle { color = Piece.PieceColor.Black, number = 1 });
            unplayedPieces.Add(new Beetle { color = Piece.PieceColor.Black, number = 2 });
            unplayedPieces.Add(new Spider { color = Piece.PieceColor.Black, number = 1 });
            unplayedPieces.Add(new Spider { color = Piece.PieceColor.Black, number = 2 });
            unplayedPieces.Add(new Hopper { color = Piece.PieceColor.Black, number = 1 });
            unplayedPieces.Add(new Hopper { color = Piece.PieceColor.Black, number = 2 });
            unplayedPieces.Add(new Hopper { color = Piece.PieceColor.Black, number = 3 });
            unplayedPieces.Add(new Ant { color = Piece.PieceColor.Black, number = 1 });
            unplayedPieces.Add(new Ant { color = Piece.PieceColor.Black, number = 2 });
            unplayedPieces.Add(new Ant { color = Piece.PieceColor.Black, number = 3 });
        }

        public void PlaceFirstPiece(Piece piece)
        {
            throw new NotImplementedException();
        }

        public void PlaceSecondPiece(Piece piece)
        {
            throw new NotImplementedException();
        }

        public void PlacePiece(Point point, Piece piece)
        {
            throw new NotImplementedException();
        }

        public void MovePiece(Point point, Piece piece)
        {
            throw new NotImplementedException();
        }

        private static void ConvertForArrayLookup(Point p)
        {
            throw new NotImplementedException();
        }

        private static void ConvertForCoordinateCalc(Point p)
        {
            throw new NotImplementedException();
        }
    }
}
