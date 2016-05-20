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

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wG1 .")));// 
            Assert.AreEqual(6, _board.hivailableSpaces.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bS1 wG1-")));// 
            Assert.AreEqual(8, _board.hivailableSpaces.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wQ -wG1")));// 
            Assert.AreEqual(10, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bQ bS1/")));// 
            Assert.AreEqual(12, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA1 \wG1")));// 
            Assert.AreEqual(13, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bQ /bS1"))); // queen move
            Assert.AreEqual(12, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA1 bQ/"))); // ant move
            Assert.AreEqual(13, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 bS1\"))); // 
            Assert.AreEqual(15, _board.hivailableSpaces.Count);
            Assert.AreEqual(3, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wS1 wQ\"))); // 
            Assert.AreEqual(16, _board.hivailableSpaces.Count);
            Assert.AreEqual(3, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 wS1-"))); //
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB1 \wS1"))); // 
            Assert.AreEqual(16, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);
            
            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB2 bB1-"))); //
            Assert.AreEqual(17, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB1 wS1"))); // beetle climb
            Assert.AreEqual(15, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);
            
            Move beetleGateClimb = Move.GetMove(new Beetle(PieceColor.Black, 1), new Hex(24, 24));
            Assert.IsTrue(_board.GetMoves().Contains(beetleGateClimb));

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB2 bS1"))); // beetle climb
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsFalse(_board.GetMoves().Contains(beetleGateClimb));

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB2 -wB1"))); // last beetle 
            Assert.AreEqual(15, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB2 wG1"))); // beetle climb
            Assert.AreEqual(15, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB2 wQ"))); // beetle climb
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.GetMoves().Contains(beetleGateClimb));

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 bB2"))); // beetle climb
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB1 bB1"))); // beetle climb
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bA1 bS1-"))); // 
            Assert.AreEqual(16, _board.hivailableSpaces.Count);
            Assert.AreEqual(3, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB2 wB1"))); // beetle climb
            Assert.AreEqual(16, _board.hivailableSpaces.Count);
            Assert.AreEqual(3, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bA1 -bQ"))); // 
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Move beetleInternalStackMove = Move.GetMove(new Beetle(PieceColor.White, 1), new Hex(25, 23));
            Assert.IsFalse(_board.GetMoves().Contains(beetleInternalStackMove));

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB2 bQ"))); // 
            Assert.AreEqual(14, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bG1 bS1-"))); // 
            Assert.AreEqual(16, _board.hivailableSpaces.Count);
            Assert.AreEqual(2, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB2 bQ-"))); // 
            Assert.AreEqual(15, _board.hivailableSpaces.Count);
            Assert.AreEqual(0, _board.articulationPoints.Count);

            Assert.AreEqual(1, _board.BlackQueenBreathingSpaces());

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bS2 bG1-"))); // 
            Assert.AreEqual(17, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wB1 wS1-"))); // 
            Assert.AreEqual(17, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 wQ"))); // 
            Assert.AreEqual(17, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wA1 /bQ"))); // 
            Assert.AreEqual(17, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB2 wS1"))); // 
            Assert.AreEqual(17, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"wG1 bQ/"))); // 
            Assert.AreEqual(18, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Move antGateMove = Move.GetMove(new Ant(PieceColor.Black, 1), new Hex(24, 24));
            Assert.IsFalse(_board.GetMoves().Contains(antGateMove));

            Assert.IsTrue(_board.TryMakeMove(Move.GetMove(@"bB1 \bQ"))); // 
            Assert.AreEqual(17, _board.hivailableSpaces.Count);
            Assert.AreEqual(1, _board.articulationPoints.Count);

            Assert.AreEqual(GameResult.WhiteWin, _board.gameResult);
            Assert.AreEqual(0, _board.GetMoves().Count);
        }

    }
}
