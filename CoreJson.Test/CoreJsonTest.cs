using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreJson.Test
{
    [TestClass]
    public class CoreJsonTest
    {
        [TestMethod]
        public void DeserializationTest()
        {
            CoreJson core = new CoreJson();
            //测试单层Json
            Assert.IsNotNull(core.Deserialization("{\n    \"name\" : \"晓明\",\n    \"age\": 18\n}"));
            //测试嵌套Json
            Assert.IsNotNull(core.Deserialization(
                "{\n    \"name\" : \"晓明\",\n    \"age\": 18,\n    \"item\":{\n    \"name\" : \"晓明\",\n    \"age\": 18\n}\n}"));

            //测试Array解析
            var item = core.Deserialization(
                "{\n    \"arr\":[\n        1,\n        -1,\n        1.2,\n        -1.2,\n        \"abc\",\n        true,\n        false,\n        {\n            \"a\":1\n        },\n        null\n    ]\n}");
            Assert.IsNotNull(item);
            var array = item.Properties[0] as JsonArray;
            Assert.IsNotNull(array);
            Assert.AreEqual(array.Values.Count, 9);
        }
    }
}