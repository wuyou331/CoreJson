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
            JsonItem item = null;
            switch (reader.Current.TokenType)
            {
                case TokenType.String:
                    item = new JsonValue()
                    {
                        ValueType = JsonValueType.String,
                        Value = reader.Current.Value
                    };
                    break;
                case TokenType.Number:
                    item = new JsonValue()
                    {
                        ValueType = JsonValueType.Number,
                        Value = reader.Current.Value
                    };
                    break;
                case TokenType.Bool:
                    item = new JsonValue()
                    {
                        ValueType = JsonValueType.Bool,
                        Value = reader.Current.Value
                    };
                    break;
                case TokenType.Null:
                    item = new JsonValue()
                    {
                        ValueType = JsonValueType.Null
                    };
                    break;
                case TokenType.BeginObject:
                    item = GetObject(reader);
                    break;
                case TokenType.BeginArray:

                    break;
            }
            return item;
        }
    }
}