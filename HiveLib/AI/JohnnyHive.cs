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

        public JohnnyHive(BoardAnalysisWeights weights)
        {
            _weights = weights;
        }

        public JohnnyHive(BoardAnalysisWeights weights, string name)
            : this(weights)
        {
            _name = name;
        }

        private BoardAnalysisWeights _weights;
        public static BoardAnalysisWeights _blockingWeights = new BoardAnalysisWeights()
        {
            articulationPointDiffWeight = 1.5,
            hivailableSpaceDiffWeight = 0.5,
            possibleMovesDiffWeight = 1.0,
            queenBreathingSpaceDiffWeight = 2.0,
            unplayedPiecesDiffWeight = 1.0,
            queenPlacementDiffWeight = 100.0,
        };

        public static BoardAnalysisWeights _winningWeights = new BoardAnalysisWeights()
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
            IOrderedEnumerable<KeyValuePair<Move, BoardAnalysisData>> orderedAnalysis;
            if (_playingWhite)
            {
                orderedAnalysis = movesData.OrderByDescending(m => m.Value.whiteAdvantage);
            }
            else
            {
                orderedAnalysis = movesData.OrderBy(m => m.Value.whiteAdvantage);
            }
            orderedMoves = orderedAnalysis.Select<KeyValuePair<Move, BoardAnalysisData>, Move>(m => m.Key).ToList();

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
                localMovesData[nextMove] = BoardAnalysisData.GetBoardAnalysisData(futureBoard, _weights);
                localDataDiffs[nextMove] = BoardAnalysisData.Diff(board, futureBoard, _weights);
            });
            movesData = localMovesData;
            dataDiffs = localDataDiffs;
        }

        private string _name;
        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? "JohnnyHive" : _name; }
        }

    }
}
