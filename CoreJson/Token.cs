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

        public Token(TokenType tokenType,  int postion) : this(tokenType)
        {
            Postion = postion;
        }

        public Token(TokenType tokenType, string value, int postion) : this(tokenType, value)
        {
            Postion = postion;
        }

        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public int Postion { get; set; }
    }
}