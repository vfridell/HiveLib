using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;

namespace HiveLib.AI
{
    public class JohnnyHive : IHiveAI
    {
        private Random _rand = new Random();
        private bool _playingWhite;
        private BoardAnalysisWeights blockingWeights = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 1.5,
            hivailableSpaceDiffWeight = 0.5,
            possibleMovesDiffWeight = 1.0,
            queenBreathingSpaceDiffWeight = 2.0,
            unplayedPiecesDiffWeight = 1.0,
            queenPlacementDiffWeight = 100.0,
        };

        private BoardAnalysisWeights weights = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 0.1,
            hivailableSpaceDiffWeight = 0.1,
            possibleMovesDiffWeight = 0.1,
            queenBreathingSpaceDiffWeight = 14.0,
            unplayedPiecesDiffWeight = 1.0,
            queenPlacementDiffWeight = 100.0,
        };

        public bool playingWhite { get { return _playingWhite; } }

        public Move MakeBestMove(Game game)
        {
            Board board = game.GetCurrentBoard();
            Move move;
            if ( (_playingWhite && board.whiteToPlay) || (!_playingWhite && !board.whiteToPlay))
            {
                move = PickBestMove(board);
                game.TryMakeMove(move);
            }
            else
            {
                throw new Exception("It is not my move :(");
            }
            return move;
        }

        public Move PickBestMove(Board board)
        {
            IDictionary<Move, BoardAnalysisData> movesData;
            IDictionary<Move, BoardAnalysisDataDiff> dataDiffs;
            AnalyzeNextMoves(board, out movesData, out dataDiffs);

            List<Move> orderedMoves;
            if (_playingWhite)
            {
                orderedMoves = movesData.OrderByDescending(m => m.Value.whiteAdvantage).Select<KeyValuePair<Move, BoardAnalysisData>, Move>(m => m.Key).ToList();
            }
            else
            {
                orderedMoves = movesData.OrderBy(m => m.Value.whiteAdvantage).Select<KeyValuePair<Move, BoardAnalysisData>, Move>(m => m.Key).ToList();
            }

            return orderedMoves[0];
        }

        public void BeginNewGame(bool playingWhite)
        {
            _playingWhite = playingWhite;
        }

        private void AnalyzeNextMoves(Board board, out IDictionary<Move, BoardAnalysisData> movesData, out IDictionary<Move, BoardAnalysisDataDiff> dataDiffs)
        {
            var localMovesData = new ConcurrentDictionary<Move, BoardAnalysisData>();
            var localDataDiffs = new ConcurrentDictionary<Move, BoardAnalysisDataDiff>();

            var moves = new ConcurrentQueue<Move>();
            foreach(Move move in board.GetMoves())
            {
                moves.Enqueue(move);
            }

            Parallel.ForEach(moves, (nextMove) =>
            {
                Board futureBoard = board.Clone();
                if (!futureBoard.TryMakeMove(nextMove)) throw new Exception("Oh noe!  Bad move.");
                localMovesData[nextMove] = BoardAnalysisData.GetBoardAnalysisData(futureBoard, weights);
                localDataDiffs[nextMove] = BoardAnalysisData.Diff(board, futureBoard, weights);
            });
            movesData = localMovesData;
            dataDiffs = localDataDiffs;
        }

        public string Name
        {
            get { return "JohnnyHive"; }
        }

    }
}
