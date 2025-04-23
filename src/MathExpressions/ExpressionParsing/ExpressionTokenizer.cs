using System;
using System.Collections.Generic;

namespace MathExpressions
{

    public class ExpressionTokenizer
    {
        private static ExpressionTokenizer _instance;
        public static ExpressionTokenizer Instance => _instance ??= new ExpressionTokenizer();

        private ExpressionTokenizer() { }

        public List<Token> Tokenize(string input)
        {
            List<Token> tokens = new();
            int i = 0;

            while (i < input.Length)
            {
                char c = input[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // Number token (digits + optional dot)
                if (char.IsDigit(c))
                {
                    string num = "";
                    while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
                    {
                        num += input[i];
                        i++;
                    }
                    tokens.Add(new Token(TokenType.Number, num));
                    continue;
                }

                // Variable name (identifier) – allow simple lowercase letters
                if (char.IsLetter(c))
                {
                    string ident = "";
                    while (i < input.Length && char.IsLetter(input[i]))
                    {
                        ident += input[i];
                        i++;
                    }
                    tokens.Add(new Token(TokenType.Identifier, ident));
                    continue;
                }

                // Check 2-character operators first
                if (i + 1 < input.Length)
                {
                    string twoCharOp = input.Substring(i, 2);
                    if (twoCharOp == "==" || twoCharOp == "!=" || twoCharOp == "<=" || twoCharOp == ">=")
                    {
                        tokens.Add(new Token(TokenType.Operator, twoCharOp));
                        i += 2;
                        continue;
                    }
                }

                // Then single-character operators
                if ("=<>+-*/^".Contains(c))
                {
                    tokens.Add(new Token(TokenType.Operator, c.ToString()));
                    i++;
                    continue;
                }

                // Parentheses
                if (c == '(')
                {
                    tokens.Add(new Token(TokenType.LeftParen, "("));
                    i++;
                    continue;
                }

                if (c == ')')
                {
                    tokens.Add(new Token(TokenType.RightParen, ")"));
                    i++;
                    continue;
                }

                // Commas
                if (c == ',')
                {
                    tokens.Add(new Token(TokenType.Comma, ","));
                    i++;
                    continue;
                }

                // Unknown character
                Console.Error.WriteLine($"Invalid character in expression: U+{(int)c:X4}");

                return null;
            }

            return tokens;
        }
    }
}
