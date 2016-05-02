using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models.Pieces;

namespace HiveLib.ViewModels
{
    class PieceVM
    {
        //public enum PieceType { Blank, Ant, Bee, Beetle, Hopper, Spider, BeetleStack };
        // TODO should probably subclass just like model

        //public PieceType pieceType { get; set; }
        public int number { get; set; }
        public Piece.PieceColor color { get; set; }
        //TODO do something that works for a beetlestack
    }
}
