using System;
using System.Text.RegularExpressions;

namespace CoreJson
{
    public class CoreJson
    {
        public T Deserialization<T>(string json)
        {
            var span = json.AsSpan();
            return default(T);
        }

        private T Deserialization<T>(ReadOnlySpan<char> span)
        {
      
            return default(T);
        }

        
    }
}