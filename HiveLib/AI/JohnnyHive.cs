using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveLib.Models;

namespace HiveLib.AI
{
    public class JohnnyHive : IHiveAI
    {
        private Board _board;
        private bool _playingWhite;

        public string MakeBestMove()
        {
            throw new NotImplementedException();
        }

        public bool TryAcceptMove(string notation, out string error)
        {
            Move move;
            if(!Move.TryGetMove("notation", out move))
            {
                error = "Error parsing move notation";
                return false;
            }
            if(!_board.TryMakeMove(move))
            {
                error = "Bad move for this board state";
                return false;
            }
            error = "";
            return true;
        }

        public void BeginNewGame(bool playingWhite)
        {
            _playingWhite = playingWhite;
            _board = Board.GetNewBoard();
        }

        //private void BreadthFirstSearchToCutoff()
        //{
        //    // breadth-first search of board states
        //    List<BoardMove> lastBoardMovesList = new List<BoardMove>();
        //    List<BoardMove> nextBoardMovesList = new List<BoardMove>();
        //    lastBoardMovesList.Add(initialBoardMove);
        //    List<Move> moves = new List<Move>();
        //    do
        //    {
        //        foreach (BoardMove currentBoardMove in lastBoardMovesList)
        //        {
        //            moves.AddRange(currentBoardMove.board.GetMoves());
        //            while (moves.Count > 0)
        //            {
        //                Board futureBoard = currentBoardMove.board.Clone();
        //                Move nextMove = GetRandomMove(moves);
        //                Assert.IsTrue(futureBoard.TryMakeMove(nextMove));
        //                var newBoardMove = new BoardMove() { board = futureBoard, move = nextMove, depth = _depth };
        //                moves.Remove(nextMove);
        //                _boardMovesGraph.AddVerticesAndEdge(new Edge<BoardMove>(currentBoardMove, newBoardMove));
        //                nextBoardMovesList.Add(newBoardMove);
        //            }
        //        }
        //        lastBoardMovesList = nextBoardMovesList;
        //        nextBoardMovesList = new List<BoardMove>();
        //        _depth++;
        //    } while (_depth < 4);
        //}

        //Move GetRandomMove(IList<Move> moves)
        //{
        //    return moves[_rand.Next(0, moves.Count - 1)];
        //}
    }
}
