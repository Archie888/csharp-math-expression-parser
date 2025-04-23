namespace MathExpressions
{
    public class Token
    {
        public TokenType Type;
        public string Value;

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() => $"{Type}: {Value}";
    }
}
