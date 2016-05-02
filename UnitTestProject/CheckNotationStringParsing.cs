using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiveLib.Models;
using HiveLib.Services;
using System.Collections.Generic;

namespace UnitTestProject
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
    }
}
