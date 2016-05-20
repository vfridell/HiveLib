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
    public class BoardTests
    {
        [TestMethod]
        public void CheckBoardCreation()
        {
            Board board = Board.GetNewBoard();
            Assert.IsFalse(board.whiteQueenPlaced);
            Assert.IsFalse(board.blackQueenPlaced);
            Assert.IsTrue(board.whiteToPlay);
            IList<Move> placementMoves = board.GetMoves();
            Assert.AreEqual(4, placementMoves.Count);
        }

        [TestMethod]
        public void CheckBeeFirstMove()
        {
            Board board = Board.GetNewBoard();
            Assert.IsFalse(board.TryMakeMove(Move.GetMove(@"wQ .")));// 
        }

        [TestMethod]
        public void CheckNonBeeFirstMove()
        {
            Board board = Board.GetNewBoard();
            Move antPlacementMove = board.GetMoves().Where(m => m.pieceToMove is Ant).FirstOrDefault();
            Assert.IsTrue(board.TryMakeMove(antPlacementMove));
            Assert.IsFalse(board.whiteQueenPlaced);
            Assert.IsFalse(board.blackQueenPlaced);
            IList<Move> secondMoves = board.GetMoves();
            // six spots with five pieces each for the second move
            Assert.AreEqual(30, secondMoves.Count);

            Assert.IsTrue(board.TryMakeMove(secondMoves[0]));
            IList<Move> thirdMoves = board.GetMoves();
            // if bee is not the first move, there are three spots with five pieces each for the third move
            Assert.AreEqual(15, thirdMoves.Count);
        }
    }
}
