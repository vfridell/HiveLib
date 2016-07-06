using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.Models.Pieces;
using HiveLib.ViewModels;
using System.Threading;

namespace HiveLib.AI
{
    public class JohnnyDeep : IHiveAI
    {
        private bool _playingWhite;
        private int _depth;

        public JohnnyDeep(BoardAnalysisWeights weights, int depth)
        {
            _weights = weights;
            _depth = depth;
        }

        public JohnnyDeep(BoardAnalysisWeights weights, int depth, string name)
            : this(weights, depth)
        {
            _name = name;
        }

        private BoardAnalysisWeights _weights;

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
            Move bestMove;
            var cancelSource = new CancellationTokenSource();
            double score = AnalyzeNextMoves(board, double.MinValue, double.MaxValue, _depth, playingWhite ? 1 : -1, cancelSource.Token, out bestMove);
            return bestMove;
        }

        public Task<Move> PickBestMoveAsync(Board board, CancellationToken aiCancelToken)
        {
            return Task.Run( () => {
                Move bestMove;
                double score = AnalyzeNextMoves(board, double.MinValue, double.MaxValue, _depth, playingWhite ? 1 : -1, aiCancelToken, out bestMove);
                return bestMove;
            });
        }

        public void BeginNewGame(bool playingWhite)
        {
            _playingWhite = playingWhite;
        }

        private double AnalyzeNextMoves(Board board, double alpha, double beta, int depth, int color, CancellationToken aiCancelToken, out Move bestMove)
        {
            aiCancelToken.ThrowIfCancellationRequested();
            if (depth == 0 || board.GetMoves().Count == 0)
            {
                bestMove = null;
                return BoardAnalysisData.GetBoardAnalysisData(board, _weights).whiteAdvantage * color;
            }

            var localMovesData = new ConcurrentDictionary<Move, Tuple<BoardAnalysisData,Board>>();
            IOrderedEnumerable<KeyValuePair<Move, Tuple<BoardAnalysisData,Board>>> orderedAnalysis;

            GetSortedMoves(board, aiCancelToken, out orderedAnalysis);
            double bestScore = double.MinValue;
            Move localBestMove = orderedAnalysis.First().Key;
            object _lock = new object();

            foreach (var kvp in orderedAnalysis)
            {
                Move subBestMove;
                double score = -AnalyzeNextMoves(kvp.Value.Item2, -beta, -alpha, depth - 1, -color, aiCancelToken, out subBestMove);
                double oldBestScore = bestScore;
                bestScore = Math.Max(score, bestScore);
                if (oldBestScore != bestScore) localBestMove = kvp.Key;
                alpha = Math.Max(alpha, bestScore);
                if (alpha >= beta)
                    break;
            };

            bestMove = localBestMove;
            return bestScore;
        }

        private void GetSortedMoves(Board board, CancellationToken aiCancelToken, out IOrderedEnumerable<KeyValuePair<Move, Tuple<BoardAnalysisData, Board>>> orderedAnalysis)
        {
            var localMovesData = new ConcurrentDictionary<Move, Tuple<BoardAnalysisData, Board>>();

            // I think this parallelism allows for randomness when picking multiple moves that all analyze to the same advantage number
            Parallel.ForEach(board.GetMoves(), (nextMove, parallelLoopState) =>
            {
                if (parallelLoopState.ShouldExitCurrentIteration) parallelLoopState.Stop();
                if (aiCancelToken.IsCancellationRequested) parallelLoopState.Stop();
                Board futureBoard = board.Clone();
                if (!futureBoard.TryMakeMove(nextMove)) throw new Exception("Oh noe!  Bad move.");
                localMovesData[nextMove] = new Tuple<BoardAnalysisData, Board>(BoardAnalysisData.GetBoardAnalysisData(futureBoard, _weights), futureBoard);
            });

            aiCancelToken.ThrowIfCancellationRequested();
            
            if (board.whiteToPlay)
            {
                orderedAnalysis = localMovesData.OrderByDescending(m => m.Value.Item1.whiteAdvantage);
            }
            else
            {
                orderedAnalysis = localMovesData.OrderBy(m => m.Value.Item1.whiteAdvantage);
            }
        }

        private string _name;
        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? string.Format("Johnny{0}Deep", _depth) : _name; }
        }

    }
}
