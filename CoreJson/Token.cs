namespace CoreJson
{
    public class Token
    {
        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
        }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }


        public TokenType TokenType { get; set; }
        public string Value { get; set; }
    }
}