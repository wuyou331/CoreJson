using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJson
{
    public abstract class JsonItem
    {
        public JsonValueType ValueType { get; set; }
        public string Key { get; set; }

    }


    public class JsonObject:JsonItem
    {
        public JsonObject()
        {
            this.ValueType = JsonValueType.Object;
        }
        public IList<JsonItem> Properties { get; set; }
    }

    public class JsonValue: JsonItem
    {
        public string Value { get; set; }
    }

    public class JsonArray: JsonItem
    {
        public JsonArray()
        {
            this.ValueType = JsonValueType.Array;
        }
        public IList<JsonItem> Values { get; set; }
    }

    public enum JsonValueType
    {
        Array,
        String,
        Bool,
        Null,
        Number,
        Object
    }
}