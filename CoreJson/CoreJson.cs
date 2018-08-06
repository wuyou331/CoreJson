using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoreJson
{
    public class CoreJson
    {
        public T Deserialization<T>(string json) where T:new()
        {
            var span = json.AsSpan();
            return Deserializer.ToObject<T>(span);
        }

  



    }
}