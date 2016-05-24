using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.Models.Pieces;

namespace HiveLib.ViewModels
{
    public class BoardAnalysisData
    {
        public static int [] turnQueenPlayedPts = {0, 25, 35, 41, 0};

        public int blackArticulationPoints;
        public int whiteArticulationPoints;
        // fewer is better
        public int articulationPointDiff { get { return blackArticulationPoints - whiteArticulationPoints; } }

        public int blackPossibleMovementMoves;
        public int whitePossibleMovementMoves;
        // more is better
        public int possibleMovesDiff { get { return whitePossibleMovementMoves - blackPossibleMovementMoves; } }

        public int blackUnplayedPieces;
        public int whiteUnplayedPieces;
        public int unplayedPiecesDiff
        {
            get
            {
                int bTurn = blackTurnNumber > 16 ? bTurn = 16 : blackTurnNumber;
                int wTurn = whiteTurnNumber > 16 ? wTurn = 16 : whiteTurnNumber;
                int whitePoints = unplayedPointMap[new Tuple<int, int>(wTurn, whiteUnplayedPieces)];
                int blackPoints = unplayedPointMap[new Tuple<int, int>(bTurn, blackUnplayedPieces)];
                return whitePoints - blackPoints;
            }
        }
        
        public int blackHivailableSpaces;
        public int whiteHivailableSpaces;
        // more is better
        public int hivailableSpaceDiff { get { return whiteHivailableSpaces - blackHivailableSpaces; } }
        
        public bool whiteToPlay;

        public bool whiteQueenPlaced;
        public bool blackQueenPlaced;
        public int turnNumber;
        public int blackTurnNumber { get { return (int)Math.Floor((double)turnNumber / 2); } }
        public int whiteTurnNumber { get { return (int)Math.Ceiling((double)turnNumber / 2); } }

        // make early queen placement a good thing
        public int queenPlacementDiff
        {
            get
            {
                int whiteQueenPlacementPts = 0;
                int blackQueenPlacementPts = 0;
                if (whiteTurnNumber < 4 && whiteQueenPlaced) whiteQueenPlacementPts = turnQueenPlayedPts[whiteTurnNumber];
                if (blackTurnNumber < 4 && blackQueenPlaced) blackQueenPlacementPts = turnQueenPlayedPts[blackTurnNumber];
                return whiteQueenPlacementPts - blackQueenPlacementPts;
            }
        }

        public int blackQueenBreathingSpaces;
        public int whiteQueenBreathingSpaces;
        // more is better
        public int queenBreathingSpaceDiff { get { return whiteQueenBreathingSpaces - blackQueenBreathingSpaces; } }

        public bool whiteCanMoveAnt;
        public bool whiteCanMoveQueen;
        public bool blackCanMoveAnt;
        public bool blackCanMoveQueen;

        public GameResult gameResult;

        private BoardAnalysisWeights _weights;
        public double whiteAdvantage
        {
            get
            {
                return (articulationPointDiff * _weights.articulationPointDiffWeight) +
                        (hivailableSpaceDiff * _weights.hivailableSpaceDiffWeight) +
                        (possibleMovesDiff * _weights.possibleMovesDiffWeight) +
                        (queenBreathingSpaceDiff * _weights.queenBreathingSpaceDiffWeight) +
                        (unplayedPiecesDiff * _weights.unplayedPiecesDiffWeight) +
                        (queenPlacementDiff * _weights.queenPlacementDiffWeight);
            }
        }

        private BoardAnalysisData() { }

        public static BoardAnalysisData GetBoardAnalysisData(Board board, BoardAnalysisWeights weights)
        {
            BoardAnalysisData d = new BoardAnalysisData();
            d._weights = weights;
            d.blackArticulationPoints = board.articulationPoints.Count(p => p.color == PieceColor.Black);
            d.whiteArticulationPoints = board.articulationPoints.Count(p => p.color == PieceColor.White);

            IReadOnlyList<Move> moves = board.GenerateAllMovementMoves();
            d.blackPossibleMovementMoves = moves.Count(m => m.pieceToMove.color == PieceColor.Black);
            d.whitePossibleMovementMoves = moves.Count(m => m.pieceToMove.color == PieceColor.White);
            
            d.blackUnplayedPieces = board.blackUnplayedPieces;
            d.whiteUnplayedPieces = board.whiteUnplayedPieces;
            d.blackHivailableSpaces = board.blackHivailableSpaces;
            d.whiteHivailableSpaces = board.whiteHivailableSpaces;
            d.gameResult = board.gameResult;
            d.whiteToPlay = board.whiteToPlay;
            d.whiteQueenPlaced = board.whiteQueenPlaced;
            d.blackQueenPlaced = board.blackQueenPlaced;
            d.turnNumber = board.turnNumber;
            d.blackQueenBreathingSpaces = board.BlackQueenBreathingSpaces();
            d.whiteQueenBreathingSpaces = board.WhiteQueenBreathingSpaces();
            d.whiteCanMoveAnt = board.whiteCanMoveAnt;
            d.blackCanMoveAnt = board.blackCanMoveAnt;
            d.whiteCanMoveQueen = board.whiteCanMoveQueen;
            d.blackCanMoveQueen = board.blackCanMoveQueen;

            return d;
        }

        public static BoardAnalysisDataDiff Diff(Board board1, Board board2, BoardAnalysisWeights weights)
        {
            BoardAnalysisData earlierBoardData;
            BoardAnalysisData laterBoardData;
            if (board1.turnNumber > board2.turnNumber)
            {
                earlierBoardData = BoardAnalysisData.GetBoardAnalysisData(board2, weights);
                laterBoardData = BoardAnalysisData.GetBoardAnalysisData(board1, weights);
            }
            else
            {
                earlierBoardData = BoardAnalysisData.GetBoardAnalysisData(board1, weights);
                laterBoardData = BoardAnalysisData.GetBoardAnalysisData(board2, weights);
            }

            var diff = new BoardAnalysisDataDiff();
            // el - early/late: comparing the earlier board with the later one on the same measure for the same color
            // bw - black/white: comparing the black versus the white on all four measures
            
            diff.el_articulationPointAdvantage = laterBoardData.articulationPointDiff - earlierBoardData.articulationPointDiff;
            diff.el_hivailableAdvantage = laterBoardData.hivailableSpaceDiff - earlierBoardData.hivailableSpaceDiff;
            diff.el_possibleMovesAdvantage = laterBoardData.possibleMovesDiff - earlierBoardData.possibleMovesDiff;
            diff.el_queenBreathingSpacgeAdvantage = laterBoardData.queenBreathingSpaceDiff - earlierBoardData.queenBreathingSpaceDiff;
            diff.el_queenPlacementAdvantage = laterBoardData.queenPlacementDiff - earlierBoardData.queenPlacementDiff;
            diff.el_unplayedPiecesAdvantage = laterBoardData.unplayedPiecesDiff - earlierBoardData.unplayedPiecesDiff;

            return diff;
        }

        internal readonly static Dictionary<Tuple<int, int>, int> unplayedPointMap = new Dictionary<Tuple<int, int>, int> 
        { 
            { new Tuple<int, int>(0, 11), 5 },
            { new Tuple<int, int>(1, 11), 5 },
            { new Tuple<int, int>(1, 10), 5 },
            { new Tuple<int, int>(2, 10), 0 },
            { new Tuple<int, int>(2, 9), 5 },
            { new Tuple<int, int>(3, 9), 0 },
            { new Tuple<int, int>(3, 8), 5 },
            { new Tuple<int, int>(4, 9), -5 },
            { new Tuple<int, int>(4, 8), 3 },
            { new Tuple<int, int>(4, 7), 5 },
            { new Tuple<int, int>(5, 9), -10 },
            { new Tuple<int, int>(5, 8), -5 },
            { new Tuple<int, int>(5, 7), 3 },
            { new Tuple<int, int>(5, 6), 3 },
            { new Tuple<int, int>(6, 9), -10 },
            { new Tuple<int, int>(6, 8), -5 },
            { new Tuple<int, int>(6, 7), 3 },
            { new Tuple<int, int>(6, 6), 5 },
            { new Tuple<int, int>(6, 5), 0 },
            { new Tuple<int, int>(7, 9), -10 },
            { new Tuple<int, int>(7, 8), -10 },
            { new Tuple<int, int>(7, 7), -5 },
            { new Tuple<int, int>(7, 6), 3 },
            { new Tuple<int, int>(7, 5), 3 },
            { new Tuple<int, int>(7, 4), 0 },
            { new Tuple<int, int>(8, 9), -10 },
            { new Tuple<int, int>(8, 8), -10 },
            { new Tuple<int, int>(8, 7), -5 },
            { new Tuple<int, int>(8, 6), 3 },
            { new Tuple<int, int>(8, 5), 3 },
            { new Tuple<int, int>(8, 4), 0 },
            { new Tuple<int, int>(8, 3), -5 },
            { new Tuple<int, int>(9, 9), -10 },
            { new Tuple<int, int>(9, 8), -10 },
            { new Tuple<int, int>(9, 7), -5 },
            { new Tuple<int, int>(9, 6), 0 },
            { new Tuple<int, int>(9, 5), 3 },
            { new Tuple<int, int>(9, 4), 3 },
            { new Tuple<int, int>(9, 3), -5 },
            { new Tuple<int, int>(9, 2), -10 },
            { new Tuple<int, int>(10, 9), -10 },
            { new Tuple<int, int>(10, 8), -10 },
            { new Tuple<int, int>(10, 7), -5 },
            { new Tuple<int, int>(10, 6), 3 },
            { new Tuple<int, int>(10, 5), 3 },
            { new Tuple<int, int>(10, 4), 3 },
            { new Tuple<int, int>(10, 3), -5 },
            { new Tuple<int, int>(10, 2), -10 },
            { new Tuple<int, int>(10, 1), -10 },
            { new Tuple<int, int>(11, 9), -10 },
            { new Tuple<int, int>(11, 8), -10 },
            { new Tuple<int, int>(11, 7), -10 },
            { new Tuple<int, int>(11, 6), -5 },
            { new Tuple<int, int>(11, 5), 3 },
            { new Tuple<int, int>(11, 4), 3 },
            { new Tuple<int, int>(11, 3), 0 },
            { new Tuple<int, int>(11, 2), -5 },
            { new Tuple<int, int>(11, 1), -10 },
            { new Tuple<int, int>(11, 0), -10 },
            { new Tuple<int, int>(12, 9), -10 },
            { new Tuple<int, int>(12, 8), -10 },
            { new Tuple<int, int>(12, 7), -10 },
            { new Tuple<int, int>(12, 6), -5 },
            { new Tuple<int, int>(12, 5), 0 },
            { new Tuple<int, int>(12, 4), 3 },
            { new Tuple<int, int>(12, 3), 3 },
            { new Tuple<int, int>(12, 2), -5 },
            { new Tuple<int, int>(12, 1), -5 },
            { new Tuple<int, int>(12, 0), -10 },
            { new Tuple<int, int>(13, 9), -10 },
            { new Tuple<int, int>(13, 8), -10 },
            { new Tuple<int, int>(13, 7), -5 },
            { new Tuple<int, int>(13, 6), -5 },
            { new Tuple<int, int>(13, 5), 3 },
            { new Tuple<int, int>(13, 4), 3 },
            { new Tuple<int, int>(13, 3), 0 },
            { new Tuple<int, int>(13, 2), 0 },
            { new Tuple<int, int>(13, 1), 0 },
            { new Tuple<int, int>(13, 0), -5 },
            { new Tuple<int, int>(14, 9), -10 },
            { new Tuple<int, int>(14, 8), -10 },
            { new Tuple<int, int>(14, 7), -10 },
            { new Tuple<int, int>(14, 6), -5 },
            { new Tuple<int, int>(14, 5), -5 },
            { new Tuple<int, int>(14, 4), 0 },
            { new Tuple<int, int>(14, 3), 3 },
            { new Tuple<int, int>(14, 2), 3 },
            { new Tuple<int, int>(14, 1), 0 },
            { new Tuple<int, int>(14, 0), -5 },
            { new Tuple<int, int>(15, 9), -10 },
            { new Tuple<int, int>(15, 8), -10 },
            { new Tuple<int, int>(15, 7), -5 },
            { new Tuple<int, int>(15, 6), -5 },
            { new Tuple<int, int>(15, 5), 0 },
            { new Tuple<int, int>(15, 4), 3 },
            { new Tuple<int, int>(15, 3), 3 },
            { new Tuple<int, int>(15, 2), 3 },
            { new Tuple<int, int>(15, 1), 0 },
            { new Tuple<int, int>(15, 0), 0 },
            { new Tuple<int, int>(16, 9), -10 },
            { new Tuple<int, int>(16, 8), -10 },
            { new Tuple<int, int>(16, 7), -10 },
            { new Tuple<int, int>(16, 6), -5 },
            { new Tuple<int, int>(16, 5), -5 },
            { new Tuple<int, int>(16, 4), -5 },
            { new Tuple<int, int>(16, 3), 3 },
            { new Tuple<int, int>(16, 2), 3 },
            { new Tuple<int, int>(16, 1), 0 },
            { new Tuple<int, int>(16, 0), 0 },
        };

    }
}
