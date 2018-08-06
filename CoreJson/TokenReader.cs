using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJson
{
    public class TokenReader
    {
        private IList<Token> _tokens;
        private int _index;

        public TokenReader(IList<Token> tokens)
        {
            _tokens = tokens;
            this._index = 0;
        }

        /// <summary>
        /// 返回顶部的第一个元素
        /// </summary>
        /// <returns></returns>
        public Token Current => _tokens[_index];

        /// <summary>
        /// 返回顶部的第一个元素，并向后移动
        /// </summary>
        /// <returns></returns>
        public Token Pop()
        {
            return _tokens[_index++];
        }

        public bool HasMore()
        {
            return _index < _tokens.Count;
        }

        /// <summary>
        /// 回退一步
        /// </summary>
        public void Previous()
        {
            --_index;
        }

        /// <summary>
        /// 向后移动
        /// </summary>
        public void NextStep()
        {
            ++_index;
        }
    }
}