using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiveLib.Models;

namespace UnitTestProject
{
    [TestClass]
    public class CheckNotationStringParsing
    {
        [TestMethod]
        public void CheckValidNotationString1()
        {
            Assert.IsTrue(Move.IsValidNotationString(@"wA3 bQ-"));
        }
        [TestMethod]
        public void CheckValidNotationString2()
        {
            Assert.IsTrue(Move.IsValidNotationString(@"bQ wQ\"));
        }
        [TestMethod]
        public void CheckValidNotationString3()
        {
            Assert.IsTrue(Move.IsValidNotationString(@"wG1 \wG2"));
        }
        [TestMethod]
        public void CheckValidNotationString4()
        {
            Assert.IsTrue(Move.IsValidNotationString(@"wB2 -bQ"));
        }
        [TestMethod]
        public void CheckValidNotationString5()
        {
            Assert.IsTrue(Move.IsValidNotationString(@"wS2 /bA3"));
        }
        [TestMethod]
        public void CheckValidNotationString6()
        {
            Assert.IsTrue(Move.IsValidNotationString(@"bB1 bS1/"));
        }
        [TestMethod]
        public void CheckValidNotationString7()
        {
            // do not need a postion for beetle move
            Assert.IsTrue(Move.IsValidNotationString(@"bB1 bS1"));
        }

        
        [TestMethod]
        public void CheckBadNotationString1()
        {
            // cannot be missing a position indicator 
            // for non-beetle move
            Assert.IsFalse(Move.IsValidNotationString("wA2 bQ"));
        }
        [TestMethod]
        public void CheckBadNotationString2()
        {
            // cannot have two position indicators on target
            Assert.IsFalse(Move.IsValidNotationString("wA2 /bQ-"));
        }
        [TestMethod]
        public void CheckBadNotationString3()
        {
            // there is no spider three
            Assert.IsFalse(Move.IsValidNotationString("bS3 -bA1"));
        }
        [TestMethod]
        public void CheckBadNotationString4()
        {
            // must have a number
            Assert.IsFalse(Move.IsValidNotationString(@"bB bA1-"));
        }
        [TestMethod]
        public void CheckBadNotationString5()
        {
            // reference and target cannot be the same piece
            Assert.IsFalse(Move.IsValidNotationString(@"bB1 bB1-"));
        }
        [TestMethod]
        public void CheckBadNotationString6()
        {
            // queen must not have a number
            Assert.IsFalse(Move.IsValidNotationString(@"bQ1 bB1-"));
        }
        [TestMethod]
        public void CheckBadNotationString7()
        {
            // valid letters only
            Assert.IsFalse(Move.IsValidNotationString(@"bJ1 bB1-"));
            Assert.IsFalse(Move.IsValidNotationString(@"bB1 bO1-"));
        }
        [TestMethod]
        public void CheckBadNotationString8()
        {
            // must have color indicator
            Assert.IsFalse(Move.IsValidNotationString(@"G1 bB1-"));
            Assert.IsFalse(Move.IsValidNotationString(@"wG1 B1-"));
        }
    }
}
