using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiveLib.Models.Pieces;
using HiveLib.Services;
using System.Collections.Generic;
using HiveLib.Models;
using QuickGraph;

namespace HiveLib.Tests
{
    [TestClass]
    public class DoAllTheMoves
    {
        Board _initialBoard;
        int _depth = 1;
        Random _rand = new Random();
        AdjacencyGraph<BoardMove, Edge<BoardMove>> _boardMovesGraph = new AdjacencyGraph<BoardMove, Edge<BoardMove>>();

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


        [TestMethod]
        public void BreadthFirstSearchToCutoff()
        {
            BoardMove initialBoardMove = new BoardMove() { board = _initialBoard, move = Move.GetMove(@"bQ bS1/"), depth = 0 };
            _boardMovesGraph.AddVertex(initialBoardMove);

            // breadth-first search of board states
            List<BoardMove> lastBoardMovesList = new List<BoardMove>();
            List<BoardMove> nextBoardMovesList = new List<BoardMove>();
            lastBoardMovesList.Add(initialBoardMove);
            List<Move> moves = new List<Move>();
            do
            {
                foreach (BoardMove currentBoardMove in lastBoardMovesList)
                {
                    moves.AddRange(currentBoardMove.board.GetMoves());
                    while (moves.Count > 0)
                    {
                        Board futureBoard = currentBoardMove.board.Clone();
                        Move nextMove = GetRandomMove(moves);
                        Assert.IsTrue(futureBoard.TryMakeMove(nextMove));
                        var newBoardMove = new BoardMove() { board = futureBoard, move = nextMove, depth = _depth };
                        moves.Remove(nextMove);
                        _boardMovesGraph.AddVerticesAndEdge(new Edge<BoardMove>(currentBoardMove, newBoardMove));
                        nextBoardMovesList.Add(newBoardMove);
                    }
                }
                lastBoardMovesList = nextBoardMovesList;
                nextBoardMovesList = new List<BoardMove>();
                _depth++;
            } while (_depth < 4);
        }

        Move GetRandomMove(IList<Move> moves)
        {
            return moves[_rand.Next(0, moves.Count - 1)];
        }
    }
}
