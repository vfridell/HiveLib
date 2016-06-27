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
            IReadOnlyList<Move> placementMoves = board.GetMoves();
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
            IReadOnlyList<Move> secondMoves = board.GetMoves();
            // six spots with four (no queen first move) pieces each for the second move
            Assert.AreEqual(24, secondMoves.Count);

            Assert.IsTrue(board.TryMakeMove(secondMoves[0]));
            IReadOnlyList<Move> thirdMoves = board.GetMoves();
            // if bee is not the first move, there are three spots with five pieces each for the third move
            Assert.AreEqual(15, thirdMoves.Count);
        }

        [TestMethod]
        public void CheckThreeFoldRepetition()
        {
            Game game = Game.GetNewGame("player1", "player2");
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG1 .")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bS1 wG1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wQ wG1/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bQ bS1-")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA1 -wG1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bQ wQ\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA1 bQ-")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bS1 -wQ")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA1 bQ\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA1 -bS1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wQ bQ/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA1 wQ-")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA2 \\wG1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 bS1/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA2 /bA2")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA3 bA1/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA3 wA1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA3 wA3-")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wB1 \\wG1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB1 bA1/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wB1 wG1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB1 /bA1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA2 bA2/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB2 bA1/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wB2 \\wA1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bG1 -bA2")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wB2 wA1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB2 bB1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wB1 bQ")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA3 \\wB2")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wB2 bA3")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA1 \\wG1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 wB2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bG1 wA2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wQ bB2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB2 wQ")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG2 wA3/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA1 bB2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG2 wG1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bG2 bA1/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA3 bG2/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA1 -wA3")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA2 bG2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB2 \\wQ")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG1 -bA2")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bG3 /bB1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA3 bG3/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bS2 -bG3")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA2 bA1/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bS2 wA3/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA2 /bA2")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA1 wQ\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wA2 bS2/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bG2 bB2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG1 bS1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bG1 wQ/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG3 \\wG1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 bG1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG3 /wB1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB2 wQ")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wG2 /bS1")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB2 \\wQ")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS2 wG1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bB2 wQ")));

            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 bA1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 wA2/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 wB2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 wA2-")));

            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 bA1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 wA2/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 wB2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 wA2-")));

            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 bA1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 wA2/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 wB2\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 wA2-")));

            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 bA1\\")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("bA2 wA2/")));
            Assert.IsTrue(game.TryMakeMove(Move.GetMove("wS1 wB2\\")));
            Assert.IsTrue(game.ThreeFoldRepetition());
        }

    }
}
