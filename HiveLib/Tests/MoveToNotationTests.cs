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
            // six spots with four (no queen first move) pieces each for the second move
            Assert.AreEqual(_secondMoves.Count, 24);

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
            Assert.AreEqual<string>(@"wQ -wA1", notation);
        }

        [TestMethod]
        public void CheckNotationForMove3()
        {
            Board board = Board.GetNewBoard();

            List<Move> moves = new List<Move>();

            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wB1 .")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bS1 wB1-")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wB2 -wB1")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bQ bS1\")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wA1 /wB1")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bG1 bS1/")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wQ wB2\")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bG1 wQ-")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wA2 -wA1")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bA1 \bQ")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wA2 bA1\")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bS1 /wA1")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wA3 \wQ")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bG2 bQ-")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wA3 \bS1")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bG2 -wQ")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wQ bG2\")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bB1 -bG2")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wS1 \wQ")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bB1 /bG2")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wS1 -wA2")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"bB1 bG2")));
            Assert.IsTrue(board.TryMakeMove(Move.GetMove(@"wA1 bQ-")));
            
            string notation = NotationParser.GetNotationForMove(Move.GetMove(new Beetle(PieceColor.Black, 1), new Hex(21,26)), board);
            
            Assert.AreNotEqual(@"bB1 \bB1", notation);
        }
        // TODO add tests for all types of moves.  especially beetle moves climbing
    }
}
