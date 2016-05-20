using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.Models.Pieces;

namespace HiveLib.ViewModels
{
    public class BoardAnalysisDataVM
    {
        public int blackArticulationPoints;
        public int whiteArticulationPoints;
        public int possibleMoves;
        public int blackUnplayedPieces;
        public int whiteUnplayedPieces;
        public int blackHivailableSpaces;
        public int whiteHivailableSpaces;
        public bool whiteToPlay;
        public bool whiteQueenPlaced;
        public bool blackQueenPlaced;
        public int turnNumber;
        public int blackQueenBreathingSpaces;
        public int whiteQueenBreathingSpaces;
        public bool whiteCanMoveAnt;
        public bool whiteCanMoveQueen;
        public bool blackCanMoveAnt;
        public bool blackCanMoveQueen;

        public GameResult gameResult;
        public bool graded = false;
        public double whiteAdvantage;

        internal BoardAnalysisDataVM(Board board)
        {
            blackArticulationPoints = board.articulationPoints.Count(p => p.color == PieceColor.Black);
            whiteArticulationPoints = board.articulationPoints.Count(p => p.color == PieceColor.White);
            possibleMoves = board.possibleMoves;
            blackUnplayedPieces = board.blackUnplayedPieces;
            whiteUnplayedPieces = board.whiteUnplayedPieces;
            blackHivailableSpaces = board.blackHivailableSpaces;
            whiteHivailableSpaces = board.whiteHivailableSpaces;
            gameResult = board.gameResult;
            whiteToPlay = board.whiteToPlay;
            whiteQueenPlaced = board.whiteQueenPlaced;
            blackQueenPlaced = board.blackQueenPlaced;
            turnNumber = board.turnNumber;
            blackQueenBreathingSpaces = board.BlackQueenBreathingSpaces();
            whiteQueenBreathingSpaces = board.WhiteQueenBreathingSpaces();
            whiteCanMoveAnt = board.whiteCanMoveAnt;
            blackCanMoveAnt = board.blackCanMoveAnt;
            whiteCanMoveQueen = board.whiteCanMoveQueen;
            blackCanMoveQueen = board.blackCanMoveQueen;
        }
    }
}
