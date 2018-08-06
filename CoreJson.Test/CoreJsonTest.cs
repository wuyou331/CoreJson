using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreJson.Test
{
    [TestClass]
    public  class CoreJsonTest
    {
        [TestMethod]
        public void DeserializationTest()
        {
            CoreJson core = new CoreJson();
            Assert.IsNotNull(core.Deserialization("{\n    \"name\" : \"晓明\",\n    \"age\": 18\n}"));
            Assert.IsNotNull(core.Deserialization("{\n    \"name\" : \"\u6653\u660E\",\n    \"age\": 18,\n    \"item\":{\n    \"name\" : \"\u6653\u660E\",\n    \"age\": 18\n}\n}"));
        }
    }
}
