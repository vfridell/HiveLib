using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveLib.ViewModels
{
    public struct BoardAnalysisWeights
    {
        public double articulationPointDiffWeight;
        public double hivailableSpaceDiffWeight;
        public double possibleMovesDiffWeight;
        public double queenBreathingSpaceDiffWeight;
        public double unplayedPiecesDiffWeight;
        public double queenPlacementDiffWeight;
    }
}
