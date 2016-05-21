using System.Linq;
using System.Collections.Generic;
using HiveLib.Models;
using HiveLib.Models.Pieces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HiveLib.Tests
{
    [TestClass]
    public class MoveToNotationTests
    {
        IReadOnlyList<Move> _firstMoves;
        IReadOnlyList<Move> _secondMoves;
        IReadOnlyList<Move> _thirdMoves;
        Board _board;

        [TestInitialize]
        public void Setup()
        {
            _board = Board.GetNewBoard();
            _firstMoves = _board.GetMoves();
            Move antPlacementMove = _firstMoves.Where(m => m.pieceToMove is Ant).FirstOrDefault();
            Assert.IsTrue(_board.TryMakeMove(antPlacementMove));
            Assert.IsFalse(_board.whiteQueenPlaced);
            Assert.IsFalse(_board.blackQueenPlaced);
            _secondMoves = _board.GetMoves();
            // six spots with five pieces each for the second move
            Assert.AreEqual(_secondMoves.Count, 30);

            Move beetlePlaceMove = _secondMoves.Where(m => m.pieceToMove is Beetle)
                                               .Where(m => m.hex == Neighborhood.GetNeighborHex(new Hex(24,24), Position.right))
                                               .FirstOrDefault();

            Assert.IsTrue(_board.TryMakeMove(beetlePlaceMove));
            _thirdMoves = _board.GetMoves();
            // if bee is not the first move, there are three spots with five pieces each for the third move
            Assert.AreEqual(_thirdMoves.Count, 15);
        }

        [TestMethod]
        public void CheckNotationForMove1()
        {
            Move move = Move.GetMove(new Ant(PieceColor.Black, 1), Board.invalidHex);
            string notation = NotationParser.GetNotationForMove(move, _board);
            Assert.AreEqual<string>(@"bA1 .", notation);
        }

        [TestMethod]
        public void CheckNotationForMove2()
        {
            Move move = _thirdMoves.Where(m => m.pieceToMove is QueenBee)
                                   .Where(m => m.hex == Neighborhood.GetNeighborHex(new Hex(24, 24), Position.left))
                                   .FirstOrDefault();
            string notation = NotationParser.GetNotationForMove(move, _board);
            Assert.AreEqual<string>(@"wQ1 -wA1", notation);
        }
    }
}
