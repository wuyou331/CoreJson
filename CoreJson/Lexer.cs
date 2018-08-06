using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreJson
{
    public static class Lexer
    {
        /// <summary>
        /// 将Json文本，去除空格并格式化为一个个单词
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static IList<Token> Analyzer(ReadOnlySpan<char> span)
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
                    return new Token(TokenType.BeginObject, postion);
                case '}':
                    return new Token(TokenType.EndObject, postion);
                case '[':
                    return new Token(TokenType.BeginArray, postion);
                case ']':
                    return new Token(TokenType.EndArray, postion);
                case ',':
                    return new Token(TokenType.Comma, postion);
                case ':':
                    return new Token(TokenType.Colon, postion);
                case 'n':
                    return ReadNull(span, ref postion);
                case 't':
                case 'f':
                    return ReadBool(span, ref postion);
                case '"':
                    return ReadString(span, ref postion);
                case '-':
                    return ReadNumber(span, ref postion);
            }
            if (char.IsNumber(chr))
            {
                return ReadNumber(span, ref postion);
            }
            throw new JsonParseException(postion);
        }

        /// <summary>
        /// 跳过连续的空字符串
        /// </summary>
        /// <param name="span"></param>
        /// <param name="postion"></param>
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
            return new Token(TokenType.Null, "null", postion);
        }

        public static Token ReadBool(ReadOnlySpan<char> span, ref int postion)
        {
            var tmp = span.Slice(postion);
            if (tmp[0] == 'r' && tmp[1] == 'u' && tmp[2] == 'e')
            {
                postion += 3;
                return new Token(TokenType.Bool, "true", postion);
            }
            else if (tmp[0] == 'a' && tmp[1] == 'l' && tmp[2] == 's' && tmp[3] == 'e')
            {
                postion += 4;
                return new Token(TokenType.Bool, "false", postion);
            }
            else
            {
                throw new JsonParseException(postion);
            }
        }

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
                return new Token(TokenType.Number, value, postion);
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
                    {
                        postion += end + 1;
                        return new Token(TokenType.String, tmp.Slice(0, end).ToString(), postion);
                    }
                }
            }
            //没有结尾引号的字符串。
            throw new JsonParseException(postion);
        }
    }
}