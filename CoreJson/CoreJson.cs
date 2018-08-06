using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoreJson
{
    public class CoreJson
    {
        public JsonObject Deserialization(string json)
        {
            var span = json.AsSpan();
            return Deserialization(span);
        }

        private JsonObject Deserialization(ReadOnlySpan<char> span)
        {
            var tokens = Tokenizer.Tokenize(span);
            var tokenReader = new TokenReader(tokens);
            var item = GetObject(tokenReader);
            return item;
        }


        public JsonObject GetObject(TokenReader reader)
        {
            var result = new JsonObject();
            if (reader.Current.TokenType == TokenType.EOF)
            {
                result.ValueType = JsonValueType.Null;
                return result;
            }

            if (reader.Current.TokenType != TokenType.BeginObject)
                throw new JsonParseException(reader.Current.Postion);
            result.Properties = new List<JsonItem>();
            do
            {
                reader.NextStep();
                string key = string.Empty;
                if (reader.Current.TokenType != TokenType.String)
                    throw new JsonParseException(reader.Current.Postion);
                key = reader.Current.Value;

                reader.NextStep();
                if (reader.Current.TokenType != TokenType.Colon)
                    throw new JsonParseException(reader.Current.Postion);

                reader.NextStep();
                JsonItem item = GetJsonItem(reader);
                item.Key = key;
                result.Properties.Add(item);

                reader.NextStep();
                if (reader.Current.TokenType == TokenType.Comma)
                {
                    continue;
                }
                else if (reader.Current.TokenType == TokenType.EndObject)
                {
                    break;
                }
                else
                {
                    throw new JsonParseException(reader.Current.Postion);
                }
            } while (reader.HasMore());

            return result;
        }

        private JsonItem GetJsonItem(TokenReader reader)
        {
            switch (reader.Current.TokenType)
            {
                case TokenType.String:
                    return new JsonValue()
                    {
                        ValueType = JsonValueType.String,
                        Value = reader.Current.Value
                    };
                case TokenType.Number:
                    return new JsonValue()
                    {
                        ValueType = JsonValueType.Number,
                        Value = reader.Current.Value
                    };
                case TokenType.Bool:
                    return new JsonValue()
                    {
                        ValueType = JsonValueType.Bool,
                        Value = reader.Current.Value
                    };
                case TokenType.Null:
                    return new JsonValue()
                    {
                        ValueType = JsonValueType.Null
                    };
                case TokenType.BeginObject:
                    return GetObject(reader);
                case TokenType.BeginArray:
                    return GetJsonArray(reader);
            }
            return null;
        }

        private JsonArray GetJsonArray(TokenReader reader)
        {
            JsonArray item = new JsonArray();
            item.Values = new List<JsonItem>();
            reader.NextStep();
            do
            {
                if (reader.Current.TokenType == TokenType.EndArray)
                    break;

                var value = GetJsonItem(reader);
                if (value == null)
                    throw new JsonParseException(reader.Current.Postion);
                item.Values.Add(value);
                reader.NextStep();

                if (reader.Current.TokenType == TokenType.EndArray)
                    break;
                else if (reader.Current.TokenType == TokenType.Comma)
                {
                    reader.NextStep();
                    var type = reader.Current.TokenType;
                    if (!(type == TokenType.String
                          || type == TokenType.Bool
                          || type == TokenType.Number
                          || type == TokenType.Null
                          || type == TokenType.BeginObject
                          || type == TokenType.BeginArray))
                    {
                        throw new JsonParseException(reader.Current.Postion);
                    }
                }
            } while (reader.HasMore());
            return item;
        }
    }
}