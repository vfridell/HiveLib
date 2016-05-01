using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveLib.Models.Pieces
{
    class Piece
    {
        public enum PieceColor {White, Black};

        internal Piece(PieceColor color, int number)
        {
            _number = number;
            _color = color;
        }

        private readonly int _number;
        internal virtual int number { get { return _number; } }

        private readonly PieceColor _color;
        internal virtual PieceColor color { get { return _color; } }
    }
}
