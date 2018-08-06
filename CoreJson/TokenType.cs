namespace CoreJson
{
    public enum TokenType
    {
        BeginObject,
        EndObject,
        BeginArray,
        EndArray,
        Null,
        Number,
        String,
        Bool,
        /// <summary>
        /// 冒号
        /// </summary>
        Colon,
        /// <summary>
        /// 逗号
        /// </summary>
        Comma,
        /// <summary>
        /// 尾部
        /// </summary>
        EOF

    }
}