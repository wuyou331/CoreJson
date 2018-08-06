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

    public class JsonNull:JsonItem
    {
        
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
        public IList<string> Values { get; set; }
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