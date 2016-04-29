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

        public Board() { }

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
