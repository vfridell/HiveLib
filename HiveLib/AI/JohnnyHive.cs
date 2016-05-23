using System;
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
            movesData = new Dictionary<Move, BoardAnalysisData>();
            dataDiffs = new Dictionary<Move, BoardAnalysisDataDiff>();

            List<Move> moves = new List<Move>();

            moves.AddRange(board.GetMoves());
            while (moves.Count > 0)
            {
                Board futureBoard = board.Clone();
                Move nextMove = GetRandomMove(moves);
                if (!futureBoard.TryMakeMove(nextMove)) throw new Exception("Oh noe!  Bad move.");
                movesData[nextMove] = BoardAnalysisData.GetBoardAnalysisData(futureBoard);
                dataDiffs[nextMove] = BoardAnalysisData.Diff(board, futureBoard);
                moves.Remove(nextMove);
            }
        }

        Move GetRandomMove(IReadOnlyList<Move> moves)
        {
            return moves[_rand.Next(0, moves.Count - 1)];
        }

        public string Name
        {
            get { return "JohnnyHive"; }
        }

    }
}
