using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiveLib.Models.Pieces;
using HiveLib.Services;
using System.Collections.Generic;

namespace HiveLib.Tests
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void CheckPieceEquality()
        {
            Ant ant = new Ant(Piece.PieceColor.Black, 1);
            Ant ant2 = new Ant(Piece.PieceColor.Black, 1);
            Ant ant3 = new Ant(Piece.PieceColor.Black, 2);
            Ant ant4 = new Ant(Piece.PieceColor.White, 1);
            Beetle beetle = new Beetle(Piece.PieceColor.Black, 1);

            Assert.IsTrue(ant.Equals(ant2));
            Assert.IsFalse(ant.Equals(beetle));
            Assert.IsFalse(ant.Equals(ant3));
            Assert.IsFalse(ant.Equals(ant4));
        }

        [TestMethod]
        public void CheckBeetleStackContains()
        {
            Ant ant = new Ant(Piece.PieceColor.Black, 1);
            Beetle beetle = new Beetle(Piece.PieceColor.Black, 1);
            Beetle beetle2 = new Beetle(Piece.PieceColor.Black, 2);
            Beetle beetle3 = new Beetle(Piece.PieceColor.White, 1);
            Beetle beetle4 = new Beetle(Piece.PieceColor.White, 2);
            BeetleStack bs = new BeetleStack(beetle3, new BeetleStack(ant, beetle));

            Assert.IsTrue(bs.Contains(beetle3));
            Assert.IsTrue(bs.Contains(ant));
            Assert.IsFalse(bs.Contains(beetle4));
        }

        [TestMethod]
        public void CheckBeetleStackEquality()
        {
            Ant ant = new Ant(Piece.PieceColor.Black, 1);
            Beetle beetle = new Beetle(Piece.PieceColor.Black, 1);
            Beetle beetle2 = new Beetle(Piece.PieceColor.Black, 2);
            Beetle beetle3 = new Beetle(Piece.PieceColor.White, 1);
            Beetle beetle4 = new Beetle(Piece.PieceColor.White, 2);
            BeetleStack bs = new BeetleStack(beetle3, new BeetleStack(ant, beetle));

            BeetleStack bs2 = new BeetleStack(beetle3, new BeetleStack(ant, beetle));

            BeetleStack bs3 = new BeetleStack(ant, beetle4);

            Assert.IsTrue(bs.Equals(bs2));
            Assert.IsFalse(bs3.Equals(bs2));
            Assert.IsFalse(bs2.Equals(beetle3));
        }

        [TestMethod]
        public void CheckBeetleStackCloning()
        {
            Ant ant = new Ant(Piece.PieceColor.Black, 1);
            Beetle beetle = new Beetle(Piece.PieceColor.Black, 1);
            Beetle beetle2 = new Beetle(Piece.PieceColor.Black, 2);
            Beetle beetle3 = new Beetle(Piece.PieceColor.White, 1);
            Beetle beetle4 = new Beetle(Piece.PieceColor.White, 2);
            BeetleStack bs = new BeetleStack(beetle3, new BeetleStack(ant, beetle));
            BeetleStack bs2 = new BeetleStack(beetle4, bs);

            Assert.IsFalse(bs.Equals(bs2));
            Assert.IsTrue(bs.Contains(beetle3));
            Assert.IsTrue(bs2.Contains(beetle3));
        }
    }
}
