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
        Board _board;

        //[TestInitialize]
        //public void Setup()
        //{
        //    Move.GetMove(new Ant(Piece.PieceColor.White, 1), new Hex(24, 24));

        //    _board = Board.GetNewBoard();
        //    _firstMoves = _board.GetMoves();
        //    Move antPlacementMove = _firstMoves.Where(m => m.pieceToMove is Ant).FirstOrDefault();
        //    Assert.IsTrue(_board.TryMakeMove(antPlacementMove));
        //    Assert.IsFalse(_board.whiteQueenPlaced);
        //    Assert.IsFalse(_board.blackQueenPlaced);
        //    _secondMoves = _board.GetMoves();
        //    // six spots with five pieces each for the second move
        //    Assert.AreEqual(_secondMoves.Count, 30);

        //    Move beetlePlaceMove = _secondMoves.Where(m => m.pieceToMove is Beetle)
        //                                       .Where(m => m.hex == Neighborhood.GetNeighborHex(new Hex(24, 24), Neighborhood.Position.right))
        //                                       .FirstOrDefault();

        //    Assert.IsTrue(_board.TryMakeMove(beetlePlaceMove));
        //    _thirdMoves = _board.GetMoves();
        //    // if bee is not the first move, there are three spots with five pieces each for the third move
        //    Assert.AreEqual(_thirdMoves.Count, 15);

        //    Move queenPlaceMove = _thirdMoves.Where(m => m.pieceToMove is QueenBee)
        //                           .Where(m => m.hex == Neighborhood.GetNeighborHex(new Hex(24, 24), Neighborhood.Position.left))
        //                           .FirstOrDefault();
        //    Assert.IsTrue(_board.TryMakeMove(queenPlaceMove));
        //    _fourthMoves = _board.GetMoves();
        //    Assert.IsTrue(_board.whiteQueenPlaced);
        //}

        //[TestMethod]
        //public void CheckQueenMove()
        //{
        //    Piece queen;
        //    Assert.IsTrue(_board.TryGetPieceAtHex(new Hex(23, 24), out queen));
        //    Assert.IsInstanceOfType(queen, typeof(QueenBee));
        //    IList<Move> moves = ((QueenBee)queen).GetMoves(new Hex(23, 24), _board);
        //    Assert.AreEqual(2, moves.Count);
        //    Assert.IsNotNull(moves.Where(m => m.hex.Equals(new Hex(24, 23))).FirstOrDefault());
        //    Assert.IsNotNull(moves.Where(m => m.hex.Equals(new Hex(23, 25))).FirstOrDefault());
        //}
    }
}
