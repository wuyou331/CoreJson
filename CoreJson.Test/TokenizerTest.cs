using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreJson.Test
{
    [TestClass]
    public class TokenizerTest
    {
        [TestMethod]
        public void ReadNumberTest()
        {
            Assert.AreEqual(TestNumber("123").Value, "123");
            Assert.AreEqual(TestNumber("123.0").Value, "123.0");
            Assert.AreEqual(TestNumber("123.01").Value, "123.01");
            Assert.AreEqual(TestNumber("-123.01").Value, "-123.01");
            Assert.AreEqual(TestNumber("-123.01.012").Value, "-123.01");
            Assert.ThrowsException<JsonParseException>(() =>TestNumber("-"));
            Assert.ThrowsException<JsonParseException>(() => TestNumber("-0"));
            Assert.ThrowsException<JsonParseException>(() => TestNumber("-."));
        }

        private Token TestNumber(string expr)
        {
            var i = 1;
            return Tokenizer.ReadNumber(expr.AsSpan(), ref i);
        }
    }
}