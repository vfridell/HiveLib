//using System;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using HiveLib.Models.Pieces;
//using HiveLib.Services;
//using System.Collections.Generic;
//using HiveLib.Models;

//namespace HiveLib.Tests
//{
//    [TestClass]
//    public class DoAllTheMoves
//    {
//        Board _board;
//        int _cutoff = 5000;
//        Random _rand = new Random();

//        [TestInitialize]
//        public void Setup()
//        {
//            _board = Board.GetNewBoard();
//            List<Move> moves = new List<Move>();
//            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wG1 .")));// 
//            Assert.AreEqual(6, _board.hivailableSpaces.Count);
//            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bS1 wG1-")));// 
//            Assert.AreEqual(8, _board.hivailableSpaces.Count);
//            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wQ -wG1")));// 
//            Assert.AreEqual(10, _board.hivailableSpaces.Count);
//            Assert.AreEqual(1, _board.articulationPoints.Count);
//            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bQ bS1/")));// 
//            Assert.AreEqual(12, _board.hivailableSpaces.Count);
//            Assert.AreEqual(2, _board.articulationPoints.Count);
//        }

//        class BoardMove { 

//        [TestMethod]
//        public void DoAllTheMoves()
//        {
//            Dictionary<Board, Move> allTheBoards = new Dictionary<Board, Move>();
//            // breadth-first search of board states
//            while (_cutoff > 0)
//            {
//                List<Move> moves = new List<Move>();
//                moves.AddRange(_board.GetMoves());
//                while (moves.Count > 0)
//                {
//                    Board futureBoard = _board.Clone();
//                    Move move = GetRandomMove(moves);
//                    Assert.IsTrue(futureBoard.TryMakeMove(move));
//                    moves.Remove(move);
//                    allTheBoards.Add(futureBoard);
//                }
//                _cutoff--;
//            }
//        }

//        Move GetRandomMove(IList<Move> moves)
//        {
//            return moves[_rand.Next(0, moves.Count - 1)];
//        }
//    }
//}
