using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UtilLibTest.Shared;

namespace UtilLibTest
{
    [TestClass]
    public class SerialisationTests
    {
        [TestMethod]
        public void TestSerialisation()
        {
            var original = new SerialisationDummy { Message = "HalloWelt" };
            var copy1 = original.DeepCopy();
            var copy2 = original.SafeDeepCopy();

            "".ToHash().ToHexString();

            Assert.AreNotEqual(original, copy1);
            Assert.AreNotEqual(original, copy2);
            Assert.AreNotEqual(copy1, copy2);

            Assert.AreEqual(original.Message, copy1.Message);
            Assert.AreEqual(original.Message, copy2.Message);
            Assert.AreEqual(copy1.Message, copy2.Message);
        }
    }
}