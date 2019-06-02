using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TAUtil.Tdf;

namespace TAUtilTest
{
    [TestClass]
    public class TdfParserTest
    {
        /// <summary>
        /// When the TDF file contains an empty statement
        /// (statement consisting only of ';')
        /// the parser shouldn't crash.
        /// </summary>
        [TestMethod]
        public void TestEmptyStatement()
        {
            var input = @"[GlobalHeader]
    {
    missionhint=; ;
    }";
            var adapter = new TdfNodeAdapter();
            new TdfParser(new MemoryStream(Encoding.UTF8.GetBytes(input)), adapter).Load();
            var globalHeader = new TdfNode("GlobalHeader");
            globalHeader.Entries.Add("missionhint", "");
            var expected = new TdfNode();
            expected.Keys.Add("GlobalHeader", globalHeader);
            Assert.IsTrue(expected.ContentsEqual(adapter.RootNode));
        }
    }
}

