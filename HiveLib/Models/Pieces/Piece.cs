﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveLib.Models.Pieces
{
    class Piece
    {
        public enum PieceColor {White, Black};
        public int number { get; set; }
        public PieceColor color { get; set; }
    }
}
