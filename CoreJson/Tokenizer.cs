using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreJson
{
    public static class Tokenizer
    {
        public static IList<Token> Tokenize(ReadOnlySpan<char> span)
        {
            var postion = 0;
            IList<Token> tokens = new List<Token>();
            do
            {
                var token = Next(span, ref postion);
                tokens.Add(token);
            } while (tokens.Last().TokenType != TokenType.EOF);

            return tokens;
        }

        public static Token Next(ReadOnlySpan<char> span, ref int postion)
        {
            SkipWhitSpace(span, ref postion);
            if (postion >= span.Length)
                return new Token(TokenType.EOF);

            var chr = span[postion++];
            switch (chr)
            {
                case '{':
                    return new Token(TokenType.BeginObject);
                case '}':
                    return new Token(TokenType.EndObject);
                case '[':
                    return new Token(TokenType.BeginArray);
                case ']':
                    return new Token(TokenType.EndArray);
                case ',':
                    return new Token(TokenType.Colon);
                case ':':
                    return new Token(TokenType.Comma);
                case 'n':
                    return ReadNull(span, ref postion);
                case 't':
                case 'f':
                    return ReadBool(span, ref postion);
                case '"':
                    return ReadString(span, ref postion);
                case '-':
                    return ReadNumber(span, ref postion);
                    return new Token(TokenType.Comma);
            }
            if (char.IsNumber(chr))
            {
                return ReadNumber(span, ref postion);
            }
            throw new JsonParseException(postion);
        }

        public static void SkipWhitSpace(ReadOnlySpan<char> span, ref int postion)
        {
            for (; postion < span.Length;)
            {
                if (Char.IsWhiteSpace(span[postion]))
                    postion++;
                else
                    break;
            }
        }

        public static Token ReadNull(ReadOnlySpan<char> span, ref int postion)
        {
            var ull = span.Slice(postion);
            if (!(ull[0] == 'u' && ull[1] == 'l' && ull[2] == 'l'))
            {
                throw new JsonParseException(postion);
            }
            postion += 3;
            return new Token(TokenType.Null, "null");
        }

        public static Token ReadBool(ReadOnlySpan<char> span, ref int postion)
        {
            var tmp = span.Slice(postion);
            if (tmp[0] == 'r' && tmp[1] == 'u' && tmp[2] == 'e')
            {
                postion += 3;
                return new Token(TokenType.Bool, "true");
            }
            else if (tmp[0] == 'a' && tmp[1] == 'l' && tmp[2] == 's' && tmp[3] == 'e')
            {
                postion += 4;
                return new Token(TokenType.Bool, "false");
            }
            else
            {
                throw new JsonParseException(postion);
            }
        }

        /// <summary>
        /// 匹配数字
        /// </summary>
        /// <param name="span"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Token ReadNumber(ReadOnlySpan<char> span, ref int postion)
        {
            postion--;
            var tmp = span.Slice(postion);
            var end = 0;
            var existDot = false;
            for (; end < tmp.Length; end++)
            {
                if (tmp[end] == '-' && end == 0 && tmp.Length > 1)
                    if (tmp[1] != '0' && Char.IsNumber(tmp[1]))
                    {
                        end++;
                        continue;
                    }
                    else
                        throw new JsonParseException(postion + end);
                else if (char.IsNumber(tmp[end]))
                    continue;
                else if (tmp[end] == '.' && !existDot && end > 0)
                    if (Char.IsNumber(tmp[end - 1]))
                    {
                        existDot = true;
                        continue;
                    }
                    else
                        throw new JsonParseException(postion + end);
                else
                    break;
            }
            if (end > 0)
            {
                var value = tmp.Slice(0, end).ToString();
                postion += end;
                return new Token(TokenType.Number, value);
            }
            else
            {
                throw new JsonParseException(postion);
            }
        }

        public static Token ReadString(ReadOnlySpan<char> span, ref int postion)
        {
            var tmp = span.Slice(postion);
            var end = 0;
            for (; end < tmp.Length; end++)
            {
                if (tmp[end] == '"')
                {
                    if (end > 0 && tmp[end - 1] == '\\')
                        continue;
                    else
                        return new Token(TokenType.String,tmp.Slice(0,end).ToString());
                }
            }
            throw new JsonParseException(postion);
        }
    }
}