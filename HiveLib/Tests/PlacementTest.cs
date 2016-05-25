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
    public class PlacementTest
    {
        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public void CheckClearingHivailabilityOfInitialPlacement()
        {
            var board = Board.GetNewBoard();
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wS1 .")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bB1 wS1\")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wB1 wS1/")));
            Assert.IsFalse(board.TryMakeMove(Move.GetMove(@"bQ -wS1")));
        }
    }
}
