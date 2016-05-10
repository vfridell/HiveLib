using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiveLib.Models;
using HiveLib.Services;
using System.Collections.Generic;
using HiveLib.Models.Pieces;

namespace HiveLib.Tests
{
    [TestClass]
    public class CheckNotationStringParsing
    {
        [TestMethod]
        public void CheckValidNotationString1()
        {
            Assert.IsTrue(HiveService.IsValidNotationString(@"wA3 bQ-"));
        }
        [TestMethod]
        public void CheckValidNotationString2()
        {
            Assert.IsTrue(HiveService.IsValidNotationString(@"bQ wQ\"));
        }
        [TestMethod]
        public void CheckValidNotationString3()
        {
            Assert.IsTrue(HiveService.IsValidNotationString(@"wG1 \wG2"));
        }
        [TestMethod]
        public void CheckValidNotationString4()
        {
            Assert.IsTrue(HiveService.IsValidNotationString(@"wB2 -bQ"));
        }
        [TestMethod]
        public void CheckValidNotationString5()
        {
            Assert.IsTrue(HiveService.IsValidNotationString(@"wS2 /bA3"));
        }
        [TestMethod]
        public void CheckValidNotationString6()
        {
            Assert.IsTrue(HiveService.IsValidNotationString(@"bB1 bS1/"));
        }
        [TestMethod]
        public void CheckValidNotationString7()
        {
            // do not need a postion for beetle move
            Assert.IsTrue(HiveService.IsValidNotationString(@"bB1 bS1"));
        }
        [TestMethod]
        public void CheckValidNotationString8()
        {
            Assert.IsTrue(HiveService.IsValidNotationString(@"wA1 ."));
        }
        
        [TestMethod]
        public void CheckBadNotationString1()
        {
            // cannot be missing a position indicator 
            // for non-beetle move
            Assert.IsFalse(HiveService.IsValidNotationString("wA2 bQ"));
        }
        [TestMethod]
        public void CheckBadNotationString2()
        {
            // cannot have two position indicators on target
            Assert.IsFalse(HiveService.IsValidNotationString("wA2 /bQ-"));
        }
        [TestMethod]
        public void CheckBadNotationString3()
        {
            // there is no spider three
            Assert.IsFalse(HiveService.IsValidNotationString("bS3 -bA1"));
        }
        [TestMethod]
        public void CheckBadNotationString4()
        {
            // must have a number
            Assert.IsFalse(HiveService.IsValidNotationString(@"bB bA1-"));
        }
        [TestMethod]
        public void CheckBadNotationString5()
        {
            // reference and target cannot be the same piece
            Assert.IsFalse(HiveService.IsValidNotationString(@"bB1 bB1-"));
        }
        [TestMethod]
        public void CheckBadNotationString6()
        {
            // queen must not have a number
            Assert.IsFalse(HiveService.IsValidNotationString(@"bQ1 bB1-"));
        }
        [TestMethod]
        public void CheckBadNotationString7()
        {
            // valid letters only
            Assert.IsFalse(HiveService.IsValidNotationString(@"bJ1 bB1-"));
            Assert.IsFalse(HiveService.IsValidNotationString(@"bB1 bO1-"));
        }
        [TestMethod]
        public void CheckBadNotationString8()
        {
            // must have color indicator
            Assert.IsFalse(HiveService.IsValidNotationString(@"G1 bB1-"));
            Assert.IsFalse(HiveService.IsValidNotationString(@"wG1 B1-"));
        }

        [TestMethod]
        public void CheckProperPiecesReturned1()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"wA3 bQ-", out move));
            
            Assert.IsInstanceOfType(move.pieceToMove, typeof(Ant));
            Assert.IsTrue(move.pieceToMove.number == 3);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.White);

            Assert.IsInstanceOfType(move.referencePiece, typeof(QueenBee));
            Assert.IsTrue(move.referencePiece.number == 1);
            Assert.IsTrue(move.referencePiece.color == Piece.PieceColor.Black);

            Assert.IsTrue(move.targetPosition == Neighborhood.Position.right);
        }

        [TestMethod]
        public void CheckProperPiecesReturned2()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"bQ wQ\", out move));

            Assert.IsInstanceOfType(move.pieceToMove, typeof(QueenBee));
            Assert.IsTrue(move.pieceToMove.number == 1);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.Black);

            Assert.IsInstanceOfType(move.referencePiece, typeof(QueenBee));
            Assert.IsTrue(move.referencePiece.number == 1);
            Assert.IsTrue(move.referencePiece.color == Piece.PieceColor.White);

            Assert.IsTrue(move.targetPosition == Neighborhood.Position.bottomright);
        }

        [TestMethod]
        public void CheckProperPiecesReturned3()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"wG1 \wG2", out move));

            Assert.IsInstanceOfType(move.pieceToMove, typeof(Hopper));
            Assert.IsTrue(move.pieceToMove.number == 1);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.White);

            Assert.IsInstanceOfType(move.referencePiece, typeof(Hopper));
            Assert.IsTrue(move.referencePiece.number == 2);
            Assert.IsTrue(move.referencePiece.color == Piece.PieceColor.White);

            Assert.IsTrue(move.targetPosition == Neighborhood.Position.bottomleft);
        }

        [TestMethod]
        public void CheckProperPiecesReturned4()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"wB2 -bQ", out move));

            Assert.IsInstanceOfType(move.pieceToMove, typeof(Beetle));
            Assert.IsTrue(move.pieceToMove.number == 2);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.White);

            Assert.IsInstanceOfType(move.referencePiece, typeof(QueenBee));
            Assert.IsTrue(move.referencePiece.number == 1);
            Assert.IsTrue(move.referencePiece.color == Piece.PieceColor.Black);

            Assert.IsTrue(move.targetPosition == Neighborhood.Position.left);
        }

        [TestMethod]
        public void CheckProperPiecesReturned5()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"wS2 /bA3", out move));

            Assert.IsInstanceOfType(move.pieceToMove, typeof(Spider));
            Assert.IsTrue(move.pieceToMove.number == 2);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.White);

            Assert.IsInstanceOfType(move.referencePiece, typeof(Ant));
            Assert.IsTrue(move.referencePiece.number == 3);
            Assert.IsTrue(move.referencePiece.color == Piece.PieceColor.Black);

            Assert.IsTrue(move.targetPosition == Neighborhood.Position.topleft);
        }

        [TestMethod]
        public void CheckProperPiecesReturned6()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"bB1 bS1/", out move));

            Assert.IsInstanceOfType(move.pieceToMove, typeof(Beetle));
            Assert.IsTrue(move.pieceToMove.number == 1);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.Black);

            Assert.IsInstanceOfType(move.referencePiece, typeof(Spider));
            Assert.IsTrue(move.referencePiece.number == 1);
            Assert.IsTrue(move.referencePiece.color == Piece.PieceColor.Black);

            Assert.IsTrue(move.targetPosition == Neighborhood.Position.topright);
        }

        [TestMethod]
        public void CheckProperPiecesReturned7()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"bB1 bS1", out move));

            Assert.IsInstanceOfType(move.pieceToMove, typeof(Beetle));
            Assert.IsTrue(move.pieceToMove.number == 1);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.Black);

            Assert.IsInstanceOfType(move.referencePiece, typeof(Spider));
            Assert.IsTrue(move.referencePiece.number == 1);
            Assert.IsTrue(move.referencePiece.color == Piece.PieceColor.Black);

            Assert.IsTrue(move.targetPosition == Neighborhood.Position.center);
        }

        [TestMethod]
        public void CheckProperPiecesReturned8()
        {
            Move move;
            Assert.IsTrue(Move.TryGetMove(@"wA1 .", out move));

            Assert.IsInstanceOfType(move.pieceToMove, typeof(Ant));
            Assert.IsTrue(move.pieceToMove.number == 1);
            Assert.IsTrue(move.pieceToMove.color == Piece.PieceColor.White);

            Assert.IsNull(move.referencePiece);
            Assert.IsTrue(move.hex != Board.invalidHex);
        }

        

    }
}
