using System;
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
            Assert.AreEqual(placementMoves.Count, 5);
        }

        [TestMethod]
        public void CheckFirstMove()
        {
            Board board = Board.GetNewBoard();
            IList<Move> placementMoves = board.GetMoves();
            Assert.IsTrue(board.TryMakeMove(placementMoves[0]));
        }

        //[TestMethod]
        //public void CheckSecondMove()
        //{
        //    Board board = Board.GetNewBoard();
        //    IList<Move> placementMoves = board.GetMoves();
        //    Assert.IsTrue(board.TryMakeMove(placementMoves[0]));
        //}
    }
}
