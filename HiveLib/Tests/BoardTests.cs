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
            Assert.AreEqual(placementMoves.Count, 5);
        }

        [TestMethod]
        public void CheckBeeFirstMove()
        {
            Board board = Board.GetNewBoard();
            Move queenPlacementMove = board.GetMoves().Where(m => m.pieceToMove is QueenBee).FirstOrDefault();
            Assert.IsTrue(board.TryMakeMove(queenPlacementMove));
            Assert.IsTrue(board.whiteQueenPlaced);
            Assert.IsFalse(board.blackQueenPlaced);
            IList<Move> placementMoves = board.GetMoves();
            // six spots with five pieces each for the second move
            Assert.AreEqual(placementMoves.Count, 30);
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
            Assert.AreEqual(secondMoves.Count, 30);

            Assert.IsTrue(board.TryMakeMove(secondMoves[0]));
            IList<Move> thirdMoves = board.GetMoves();
            // if bee is not the first move, there are three spots with five pieces each for the third move
            Assert.AreEqual(thirdMoves.Count, 15);
        }
    }
}
