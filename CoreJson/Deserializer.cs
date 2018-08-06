using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreJson
{
    /// <summary>
    /// 反序列化器
    /// </summary>
    public class Deserializer
    {
        public static T ToObject<T>(ReadOnlySpan<char> span) where T : new()
        {
            JsonObject jsonObject = AnalysisObject(span);

            return (T)ConvertObject(jsonObject,typeof(T));
        }

        private static object ConvertObject(JsonObject jObject,Type type)
        {
            object result = Activator.CreateInstance(type);
            var properites = result.GetType().GetProperties();
            foreach (var propertyInfo in properites)
            {

                var jsonItem = jObject.Properties
                    .FirstOrDefault(it => it.Key == propertyInfo.Name || it.Key.ToLower() == propertyInfo.Name.ToLower());
                if (jsonItem != null)
                {
                    if (jsonItem is JsonValue val)
                    {
                        var value = ConvertType(val.Value, propertyInfo.PropertyType);
                        propertyInfo.SetValue(result, value);
                    }
                    else if (jsonItem is JsonObject obj)
                    {
                        var objType = propertyInfo.PropertyType;
                        var item = ConvertObject(obj, objType);
                        propertyInfo.SetValue(result, item);
                    }
                    else if (jsonItem is JsonArray arr)
                    {
                        var item = ConverArray(arr);
                        propertyInfo.SetValue(result, item);
                    }
                }
            }
            return result;
        }

        private static object[] ConverArray(JsonArray arr)
        {
            var arrs = new object[arr.Values.Count];
            var i = 0;
            foreach (var arrValue in arr.Values)
            {
                if (arrValue is JsonValue val)
                {
                  arrs[i]=ConvertValue(val);  
                }else if (arrValue is JsonArray array)
                {
                    arrs[i]=ConverArray(array);
                }
                else if (arrValue is JsonObject obj)
                {
                    arrs[i] = new List<object>();
                }
                i++;
            }
      
            return arrs;
        }
        private static object ConvertValue(JsonValue value)
        {
            if (value.ValueType == JsonValueType.Null) return null;
            if (value.ValueType == JsonValueType.String) return value.Value;
            if (value.ValueType == JsonValueType.Bool) return bool.Parse(value.Value);
            if (value.ValueType == JsonValueType.Number) return double.Parse(value.Value);
            return null;
        }

        private static object ConvertType(string str, Type type)
        {
            if (str == null) return null;
            if (type == typeof(string)) return str;
            if (type == typeof(int)) return int.Parse(str);
            if (type == typeof(double)) return double.Parse(str);
            if (type == typeof(bool)) return bool.Parse(str);
            return null;
        }


        public static JsonObject AnalysisObject(ReadOnlySpan<char> span)
        {
            var tokens = Lexer.Analyzer(span);
            var tokenReader = new TokenReader(tokens);
            return GetObject(tokenReader);
        }

        private static JsonObject GetObject(TokenReader reader)
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
                    continue;
                else if (reader.Current.TokenType == TokenType.EndObject)
                    break;
                else
                    throw new JsonParseException(reader.Current.Postion);
            } while (reader.HasMore());

            return result;
        }

        private static JsonItem GetJsonItem(TokenReader reader)
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

        private static JsonArray GetJsonArray(TokenReader reader)
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