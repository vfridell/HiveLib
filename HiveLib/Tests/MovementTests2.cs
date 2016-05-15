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
    public class MovementTests2
    {
        Board _board;

        [TestInitialize]
        public void Setup()
        {
            _board = Board.GetNewBoard();

            List<Move> moves = new List<Move>();
            moves.Add(Move.GetMove("wA1 ."));
            moves.Add(Move.GetMove("bB1 wA1-"));
            moves.Add(Move.GetMove("wQ -wA1"));
            moves.Add(Move.GetMove("bQ bB1-"));
            moves.Add(Move.GetMove(@"wB1 \wA1"));
            foreach (Move move in moves)
            {
                Assert.IsTrue(_board.TryMakeMove(move));
            }
        }

        [TestMethod]
        public void CheckArticulationPoints()
        {
            Assert.IsTrue(_board.articulationPoints.Count == 2);
        }
    }
}
