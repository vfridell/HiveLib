using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;
using HiveLib.ViewModels;
using System.Threading;

namespace HiveLib.AI
{
    public class RandomAI : IHiveAI
    {
        private Random _rand = new Random();
        private bool _playingWhite;
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

        public void BeginNewGame(bool playingWhite)
        {
            _playingWhite = playingWhite;
        }

        Move GetRandomMove(IReadOnlyList<Move> moves)
        {
            return moves[_rand.Next(0, moves.Count - 1)];
        }

        public string Name
        {
            get { return "RandomAI"; }
        }

        public Move PickBestMove(Board board)
        {
            IReadOnlyList<Move> moves = board.GetMoves();
            return GetRandomMove(moves);
        }

        public Task<Move> PickBestMoveAsync(Board board, CancellationToken aiCancelToken)
        {
            IReadOnlyList<Move> moves = board.GetMoves();
            return Task.FromResult(GetRandomMove(moves));
        }
    }
}
