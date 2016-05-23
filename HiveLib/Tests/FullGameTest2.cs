using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiveLib.Models.Pieces;
using HiveLib.Services;
using System.Collections.Generic;
using HiveLib.Models;
using HiveLib.AI;

namespace HiveLib.Tests
{
    [TestClass]
    public class FullGameTest2
    {
        Board _board;

        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public void CheckHivailabilityAndArticulationPoints()
        {
            IHiveAI AI = new JohnnyHive();
            AI.BeginNewGame(true);
            Game game = Game.GetNewGame(AI.Name, "test");
            _board = Board.GetNewBoard();

            List<Move> moves = new List<Move>();

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB1 .")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bS1 wB1-")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB2 -wB1")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bQ bS1\")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA1 /wB1")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bG1 bS1/")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wQ wB2\")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bG1 wQ-")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA2 -wA1")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bA1 \bQ")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA2 bA1\")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bS1 /wA1")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA3 \wQ")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bG2 bQ-")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA3 \bS1")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bG2 -wQ")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wQ bG2\")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 -bG2")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wS1 \wQ")));
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 bG2")));
            Move move = AI.PickBestMove(_board);
            //Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wS1 \wS1")));


        }

    }
}
