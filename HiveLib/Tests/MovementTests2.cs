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
        IList<Move> _firstMoves;
        IList<Move> _secondMoves;
        IList<Move> _thirdMoves;
        IList<Move> _fourthMoves;
        IList<Move> _fifthMoves;
        IList<Move> _sixthMoves;
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
                                               .Where(m => m.hex == Neighborhood.GetNeighborHex(new Hex(24, 24), Neighborhood.Position.right))
                                               .FirstOrDefault();

            Assert.IsTrue(_board.TryMakeMove(beetlePlaceMove));
            _thirdMoves = _board.GetMoves();
            // if bee is not the first move, there are three spots with five pieces each for the third move
            Assert.AreEqual(_thirdMoves.Count, 15);

            Move queenPlaceMove = _thirdMoves.Where(m => m.pieceToMove is QueenBee)
                                   .Where(m => m.hex == Neighborhood.GetNeighborHex(new Hex(24, 24), Neighborhood.Position.left))
                                   .FirstOrDefault();
            Assert.IsTrue(_board.TryMakeMove(queenPlaceMove));
            _fourthMoves = _board.GetMoves();
            Assert.IsTrue(_board.whiteQueenPlaced);
            Move bQueenPlaceMove = _thirdMoves.Where(m => m.pieceToMove is QueenBee)
                                   .Where(m => m.hex == Neighborhood.GetNeighborHex(new Hex(25, 24), Neighborhood.Position.right))
                                   .FirstOrDefault();
            Assert.IsTrue(_board.TryMakeMove(bQueenPlaceMove));
            _fifthMoves = _board.GetMoves();
            Assert.IsTrue(_board.blackQueenPlaced);

            Ant wA1 = new Ant(Piece.PieceColor.White, 1);
            Hex hex;
            _board.TryGetHexOfPlayedPiece(wA1, out hex);
            Move wBeetlePlaceMove = _thirdMoves.Where(m => m.pieceToMove is Beetle)
                                   .Where(m => m.hex == Neighborhood.GetNeighborHex(hex, Neighborhood.Position.bottomleft))
                                   .FirstOrDefault();
            Assert.IsTrue(_board.TryMakeMove(wBeetlePlaceMove));
            _sixthMoves = _board.GetMoves();
            Assert.IsTrue(_board.blackQueenPlaced);
        }

        [TestMethod]
        public void CheckDFS()
        {
            _board.RunDFS();
        }
    }
}
