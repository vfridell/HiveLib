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
    public class FullGameTest 
    {
        Board _board;

        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public void CheckHivailabilityAndArticulationPoints()
        {
            _board = Board.GetNewBoard();

            List<Move> moves = new List<Move>();

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wG1 .")));// placement
            Assert.AreEqual(6, _board.hivailableSpaces.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bS1 wG1-")));// placement
            Assert.AreEqual(8, _board.hivailableSpaces.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wQ -wG1")));// placement
            Assert.AreEqual(10, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bQ bS1/")));// placement
            Assert.AreEqual(12, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA1 \wG1")));// placement
            Assert.AreEqual(13, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bQ /bS1"))); // movement
            Assert.AreEqual(12, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA1 bQ/"))); // movement
            Assert.AreEqual(15, _board.hivailableSpaces.Count);
            Assert.AreEqual(3, _board.articulationPoints.Count);

            /*
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 bS1"))); // beetle climb
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(3, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB1 wA1-"))); // placement
            Assert.AreEqual(16, _board.hivailableSpaces.Count);
            Assert.AreEqual(4, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 wS1"))); // beetle from stack to new stack move
            Assert.AreEqual(16, _board.hivailableSpaces.Count);
            Assert.AreEqual(4, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB2 -wQ"))); // placement
            Assert.AreEqual(18, _board.hivailableSpaces.Count);
            Assert.AreEqual(5, _board.articulationPoints.Count);
            
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 wQ-"))); // beetle from stack to empty space
            Assert.AreEqual(18, _board.hivailableSpaces.Count);
            Assert.AreEqual(4, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wG1 wB1\"))); // hopper
            Assert.AreEqual(20, _board.hivailableSpaces.Count);
            Assert.AreEqual(5, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bG1 bS1-"))); // hopper
            Assert.AreEqual(21, _board.hivailableSpaces.Count);
            Assert.AreEqual(5, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wG1 bQ-"))); // hopper
            Assert.AreEqual(18, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bG1 -wS1"))); // hopper
            Assert.AreEqual(18, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wS1 -bG1"))); // spider
            Assert.AreEqual(20, _board.hivailableSpaces.Count);
            Assert.AreEqual(4, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB2 -bQ"))); // 
            Assert.AreEqual(20, _board.hivailableSpaces.Count);
            Assert.AreEqual(4, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wS1 \bB2"))); // spider across gate
            Assert.AreEqual(21, _board.hivailableSpaces.Count);
            Assert.AreEqual(5, _board.articulationPoints.Count);
             */
        }

    }
}
