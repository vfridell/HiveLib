using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models.Pieces;

namespace HiveLib.ViewModels
{
    public enum PieceType { Blank, Ant, Bee, Beetle, Hopper, Spider, BeetleStack };

    public class PieceVM
    {
        public PieceType pieceType { get; set; }
        public int number { get; set; }
        public PieceColor color { get; set; }

        public bool isBeetleStack { get; set; }
        public PieceVM bottom { get; set; }
        public PieceVM secondLevel { get ;set; }
        public PieceVM thirdLevel { get ;set; }
        public PieceVM fourthLevel { get ;set; }
        public PieceVM fifthLevel { get ;set; }
    }
}
