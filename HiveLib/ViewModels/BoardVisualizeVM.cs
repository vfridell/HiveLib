using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.Models.Pieces;

namespace HiveLib.ViewModels
{
    public class BoardVisualizeVM
    {
        public HashSet<PieceVM> unplayedPieces = new HashSet<PieceVM>();
        public Dictionary<PieceVM, Hex> playedPieces = new Dictionary<PieceVM, Hex>();
        public List<Move> moves = new List<Move>();
    }
}
