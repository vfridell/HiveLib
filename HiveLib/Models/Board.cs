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
        public static readonly int columns = 50;
        public static readonly int rows = 50;
        
        internal readonly List<Piece> unplayedPieces = new List<Piece>();
        internal readonly Piece [,] boardArray = new Piece[columns, rows];

        internal Board() 
        {
            unplayedPieces.Add(new Bee(Piece.PieceColor.White, 1));
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

            unplayedPieces.Add(new Bee(Piece.PieceColor.Black, 1));
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

        internal Piece GetPieceByCoordinate(int column, int row)
        {
            return boardArray[column, row];
        }

        internal Neighborhood GetNeighborhood(int column, int row)
        {
            Neighborhood neighborhood = new Neighborhood();
            neighborhood.center = boardArray[column, row];
            
            // Axial coordinates
            // ref: http://www.redblobgames.com/grids/hexagons/
            neighborhood.topleft = boardArray[column, row - 1];
            neighborhood.topright = boardArray[column + 1, row - 1];
            neighborhood.right = boardArray[column + 1, row];
            neighborhood.bottomright = boardArray[column, row + 1];
            neighborhood.bottomleft = boardArray[column - 1, row + 1];
            neighborhood.left = boardArray[column - 1, row];

            return neighborhood;
        }

    }
}
