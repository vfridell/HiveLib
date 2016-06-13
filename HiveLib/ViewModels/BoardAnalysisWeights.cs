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
        public double movementPlacementDiffWeight;
        public double ownedBeetleStacksWeight;

        public static BoardAnalysisWeights blockingWeights = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 1.5,
            hivailableSpaceDiffWeight = 0.5,
            possibleMovesDiffWeight = 1.0,
            queenBreathingSpaceDiffWeight = 2.0,
            unplayedPiecesDiffWeight = 1.0,
            queenPlacementDiffWeight = 100.0,
            movementPlacementDiffWeight = 0,
        };

        public static BoardAnalysisWeights winningWeights = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 0.1,
            hivailableSpaceDiffWeight = 0.1,
            possibleMovesDiffWeight = 0.1,
            queenBreathingSpaceDiffWeight = 14.0,
            unplayedPiecesDiffWeight = 1.0,
            queenPlacementDiffWeight = 100.0,
            movementPlacementDiffWeight = 0,
            ownedBeetleStacksWeight = 10,
        };

        public static BoardAnalysisWeights newWeights = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 0,
            hivailableSpaceDiffWeight = 0,
            possibleMovesDiffWeight = 0,
            queenBreathingSpaceDiffWeight = 0,
            unplayedPiecesDiffWeight = 0,
            queenPlacementDiffWeight = 100.0,
            movementPlacementDiffWeight = 15.0,
            ownedBeetleStacksWeight = 10,
        };

    }
}
