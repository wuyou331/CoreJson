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
        public void AnalysisObjectTest()
        {
            //测试单层Json
            Assert.IsNotNull(Deserializer.AnalysisObject("{\n    \"name\" : \"晓明\",\n    \"age\": 18\n}"));
            //测试嵌套Json
            Assert.IsNotNull(Deserializer.AnalysisObject(
                "{\n    \"name\" : \"晓明\",\n    \"age\": 18,\n    \"item\":{\n    \"name\" : \"晓明\",\n    \"age\": 18\n}\n}"));

            //测试Array解析
            var item = Deserializer.AnalysisObject(
                "{\n    \"arr\":[\n        1,\n        -1,\n        1.2,\n        -1.2,\n        \"abc\",\n        true,\n        false,\n        {\n            \"a\":1\n        },\n        null\n    ]\n}");
            Assert.IsNotNull(item);
            var array = item.Properties[0] as JsonArray;
            Assert.IsNotNull(array);
            Assert.AreEqual(array.Values.Count, 9);
        }

        [TestMethod]
        public void DeserializerTest()
        {
            var item = Deserializer.ToObject<Student>("{\n    \"name\" : \"晓明\",\n    \"age\": 18\n}");
            Assert.AreEqual(item.Name, "晓明");
            Assert.AreEqual(item.Age, 18);

             item = Deserializer.ToObject<Student>("{\n    \"name\":\"晓明\",\n    \"age\":18,\n    \"attr\":[\n        \"18\",\n        \"张三\"\n    ]\n}");
            Assert.AreEqual(item.Name, "晓明");
            Assert.AreEqual(item.Age, 18);
        }
    }


    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[]Attr { get; set; }
    }
}