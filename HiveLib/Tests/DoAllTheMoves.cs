using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiveLib.Models.Pieces;
using HiveLib.Services;
using System.Collections.Generic;
using HiveLib.Models;

namespace HiveLib.Tests
{
    [TestClass]
    public class DoAllTheMoves
    {
        Board _initialBoard;
        int _cutoff = 3;
        int _depth = 1;
        Random _rand = new Random();

        [TestInitialize]
        public void Setup()
        {
            _initialBoard = Board.GetNewBoard();
            List<Move> moves = new List<Move>();
            Assert.IsTrue(_initialBoard.TryMakeMove(Move.GetMove(@"wG1 .")));// 
            Assert.AreEqual(6, _initialBoard.hivailableSpaces.Count);
            Assert.IsTrue(_initialBoard.TryMakeMove(Move.GetMove(@"bS1 wG1-")));// 
            Assert.AreEqual(8, _initialBoard.hivailableSpaces.Count);
            Assert.IsTrue(_initialBoard.TryMakeMove(Move.GetMove(@"wQ -wG1")));// 
            Assert.AreEqual(10, _initialBoard.hivailableSpaces.Count);
            Assert.AreEqual(1, _initialBoard.articulationPoints.Count);
            Assert.IsTrue(_initialBoard.TryMakeMove(Move.GetMove(@"bQ bS1/")));// 
            Assert.AreEqual(12, _initialBoard.hivailableSpaces.Count);
            Assert.AreEqual(2, _initialBoard.articulationPoints.Count);
        }

        class BoardMove { public Board board; public Move move; }

        [TestMethod]
        public void BreadthFirstSearchToCutoff()
        {
            var allTheBoards = new Dictionary<int, IList<BoardMove>>();
            allTheBoards[0] = new List<BoardMove>();
            allTheBoards[0].Add(new BoardMove() { board = _initialBoard, move = null });

            // breadth-first search of board states
            while (_depth < 10)
            {
                foreach (BoardMove boardMove in allTheBoards[_depth - 1])
                {
                    List<Move> moves = new List<Move>();
                    moves.AddRange(boardMove.board.GetMoves());
                    List<BoardMove> boardMoves = new List<BoardMove>();
                    while (moves.Count > 0)
                    {
                        Board futureBoard = boardMove.board.Clone();
                        Move nextMove = GetRandomMove(moves);
                        Assert.IsTrue(futureBoard.TryMakeMove(nextMove));
                        var newBoardMove = new BoardMove() { board = futureBoard, move = nextMove };
                        moves.Remove(nextMove);
                        boardMoves.Add(newBoardMove);
                    }
                    allTheBoards[_depth] = boardMoves;
                }
                _depth++;
            }
        }

        Move GetRandomMove(IList<Move> moves)
        {
            return moves[_rand.Next(0, moves.Count - 1)];
        }
    }
}
