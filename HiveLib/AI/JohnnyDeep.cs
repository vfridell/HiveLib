﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;

namespace HiveLib.AI
{
    public class JohnnyDeep : IHiveAI
    {
        private Random _rand = new Random();
        private bool _playingWhite;

        public JohnnyDeep(BoardAnalysisWeights weights)
        {
            _weights = weights;
        }

        public JohnnyDeep(BoardAnalysisWeights weights, string name)
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
            Move bestMove;
            double score = AnalyzeNextMoves(board, double.MinValue, double.MaxValue, 3, playingWhite ? 1 : -1, out bestMove);
            return bestMove;
        }

        public void BeginNewGame(bool playingWhite)
        {
            _playingWhite = playingWhite;
        }

        private double AnalyzeNextMoves(Board board, double alpha, double beta, int depth, int color, out Move bestMove)
        {
            if (depth == 0 || board.GetMoves().Count == 0)
            {
                bestMove = null;
                return BoardAnalysisData.GetBoardAnalysisData(board, _weights).whiteAdvantage * color;
            }

            var localMovesData = new ConcurrentDictionary<Move, Tuple<BoardAnalysisData,Board>>();
            IOrderedEnumerable<KeyValuePair<Move, Tuple<BoardAnalysisData,Board>>> orderedAnalysis;

            GetSortedMoves(board, out orderedAnalysis);
            double bestScore = double.MinValue;
            Move localBestMove = orderedAnalysis.First().Key;
            object _lock = new object();
            //Parallel.ForEach(orderedAnalysis, (kvp) =>
            //{
            //    Move subBestMove;
            //    double score = -AnalyzeNextMoves(kvp.Value.Item2, -beta, -alpha, depth - 1, -color, out subBestMove);
            //    lock (_lock)
            //    {
            //        double oldBestScore = bestScore;
            //        bestScore = Math.Max(score, bestScore);
            //        if (oldBestScore != bestScore) localBestMove = kvp.Key;
            //        alpha = Math.Max(alpha, bestScore);
            //        if (alpha >= beta)
            //            return;
            //    }
            //});

            foreach (var kvp in orderedAnalysis)
            {
                Move subBestMove;
                double score = -AnalyzeNextMoves(kvp.Value.Item2, -beta, -alpha, depth - 1, -color, out subBestMove);
                lock (_lock)
                {
                    double oldBestScore = bestScore;
                    bestScore = Math.Max(score, bestScore);
                    if (oldBestScore != bestScore) localBestMove = kvp.Key;
                    alpha = Math.Max(alpha, bestScore);
                    if (alpha >= beta)
                        break;
                }
            };

            bestMove = localBestMove;
            return bestScore;
        }

        private void GetSortedMoves(Board board, out IOrderedEnumerable<KeyValuePair<Move, Tuple<BoardAnalysisData, Board>>> orderedAnalysis)
        {
            var localMovesData = new ConcurrentDictionary<Move, Tuple<BoardAnalysisData, Board>>();

            Parallel.ForEach(board.GetMoves(), (nextMove) =>
            {
                Board futureBoard = board.Clone();
                if (!futureBoard.TryMakeMove(nextMove)) throw new Exception("Oh noe!  Bad move.");
                localMovesData[nextMove] = new Tuple<BoardAnalysisData, Board>(BoardAnalysisData.GetBoardAnalysisData(futureBoard, _weights), futureBoard);
            });

            //foreach(Move nextMove in board.GetMoves())
            //{
            //    Board futureBoard = board.Clone();
            //    if (!futureBoard.TryMakeMove(nextMove)) throw new Exception("Oh noe!  Bad move.");
            //    localMovesData[nextMove] = new Tuple<BoardAnalysisData, Board>(BoardAnalysisData.GetBoardAnalysisData(futureBoard, _weights), futureBoard);
            //};

            //List<Move> orderedMoves;
            if (board.whiteToPlay)
            {
                orderedAnalysis = localMovesData.OrderByDescending(m => m.Value.Item1.whiteAdvantage);
            }
            else
            {
                orderedAnalysis = localMovesData.OrderBy(m => m.Value.Item1.whiteAdvantage);
            }
            //orderedMoves = orderedAnalysis.Select<KeyValuePair<Move, BoardAnalysisData>, Move>(m => m.Key).ToList();
        }

        private string _name;
        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? "JohnnyDeep" : _name; }
        }

    }
}