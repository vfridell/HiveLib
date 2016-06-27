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
            ownedBeetleStacksWeight = 0,
        };

        public static BoardAnalysisWeights oldWinningWeights1 = new BoardAnalysisWeights()
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

        public static BoardAnalysisWeights winningWeights = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 0.3,
            hivailableSpaceDiffWeight = -0.1,
            possibleMovesDiffWeight = 0.3,
            queenBreathingSpaceDiffWeight = 13.4,
            unplayedPiecesDiffWeight = 0.8,
            queenPlacementDiffWeight = 100,
            ownedBeetleStacksWeight = 9.6,
            movementPlacementDiffWeight = 0,
        };

        public static BoardAnalysisWeights foundWeights1 = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 1.3,
            hivailableSpaceDiffWeight = -0.7,
            possibleMovesDiffWeight = 0.1,
            queenBreathingSpaceDiffWeight = 14,
            unplayedPiecesDiffWeight = 0.6,
            queenPlacementDiffWeight = 100,
            ownedBeetleStacksWeight = 9.2,
            movementPlacementDiffWeight = 0,
        };

        public static BoardAnalysisWeights foundWeights2 = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 1.3,
            hivailableSpaceDiffWeight = -1.1,
            possibleMovesDiffWeight = 0.1,
            queenBreathingSpaceDiffWeight = 14,
            unplayedPiecesDiffWeight = 1.0,
            queenPlacementDiffWeight = 100,
            ownedBeetleStacksWeight = 10.0,
            movementPlacementDiffWeight = 0,
        };


        public override string ToString()
        {
            return string.Format("articulationPointDiffWeight: {0} \n", articulationPointDiffWeight) +
                    string.Format("hivailableSpaceDiffWeight: {0} \n", hivailableSpaceDiffWeight) +
                    string.Format("possibleMovesDiffWeight: {0} \n", possibleMovesDiffWeight) +
                    string.Format("queenBreathingSpaceDiffWeight: {0} \n", queenBreathingSpaceDiffWeight) +
                    string.Format("unplayedPiecesDiffWeight: {0} \n", unplayedPiecesDiffWeight) +
                    string.Format("queenPlacementDiffWeight: {0} \n", queenPlacementDiffWeight) +
                    string.Format("ownedBeetleStacksDiffWeight: {0} \n", ownedBeetleStacksWeight) +
                    string.Format("movementPlacementAdvantageDiffWeight: {0} \n", movementPlacementDiffWeight);
        }
    }

    public struct BoardAnalysisWeightAdjustment
    {
        public double articulationPointAdj;
        public double hivailableSpaceAdj;
        public double possibleMovesAdj;
        public double queenBreathingAdj;
        public double unplayedPiecesAdj;
        public double ownedBeetleAdj;
    }
}
