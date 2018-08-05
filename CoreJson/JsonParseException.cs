using System;

namespace CoreJson
{
    public class JsonParseException : Exception
    {
        /// <summary>
        /// ³ö´íµÄÎ»ÖÃ
        /// </summary>
        public int Postion { get; set; }

        public JsonParseException(int postion) : base($"Invalid json string;\r\nPostion:{postion}")
        {
            this.Postion = postion;
        }
    }
}