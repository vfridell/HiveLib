﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;
using System.Threading;

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
            var cancelSource = new CancellationTokenSource();
            return PickBestMoveAsync(board, cancelSource.Token).Result;
        }

        public Task<Move> PickBestMoveAsync(Board board, CancellationToken aiCancelToken)
        {
            return Task.Run(() =>
            {
                IDictionary<Move, BoardAnalysisData> movesData;
                IDictionary<Move, BoardAnalysisDataDiff> dataDiffs;
                AnalyzeNextMoves(board, aiCancelToken, out movesData, out dataDiffs);

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
            });
        }


        public void BeginNewGame(bool playingWhite)
        {
            _playingWhite = playingWhite;
        }

        private void AnalyzeNextMoves(Board board, CancellationToken aiCancelToken, out IDictionary<Move, BoardAnalysisData> movesData, out IDictionary<Move, BoardAnalysisDataDiff> dataDiffs)
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
                aiCancelToken.ThrowIfCancellationRequested();
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
