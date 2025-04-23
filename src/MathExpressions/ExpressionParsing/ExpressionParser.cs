using System;
using System.Collections.Generic;

namespace MathExpressions
{
    public class ExpressionParser
    {
        private static ExpressionParser _instance;
        public static ExpressionParser Instance => _instance ??= new ExpressionParser();

        private ExpressionParser() { }

        private List<Token> tokens;
        private int current;

        private static readonly HashSet<string> KnownFunctions = new()
        {
            "sin", "cos", "tan", "sqrt", "log", "max", "min"
        };

            private static readonly HashSet<string> KnownConstants = new()
        {
            "pi", "e"
        };

        public Expression ParseExpression(string input)
        {
            Console.WriteLine("Starting to parse expression: '"+input+"'");

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Empty expression string received. Skipping parse.");
                return null;
            }

            tokens = ExpressionTokenizer.Instance.Tokenize(input);
            current = 0;
            return ParseComparison();
        }

        public Expression ParseSubExpression() {
            return ParseComparison();
        }

        internal Expression ParseComparison()
        {
            Expression left = ParseAddSub();

            if (Match(TokenType.Operator, "=", "==", "!=", "<", ">", "<=", ">="))
            {
                string op = Previous().Value;
                Expression right = ParseAddSub();
                //ExpressionType type = GetComparisonType(op);
                ExpressionType type = ExpressionTools.GetComparisonType(op);
                return new Expression(type, left, right);
            }

            return left;
        }

        internal Expression ParseAddSub()
        {
            Expression expr = ParseMulDiv();

            while (Match(TokenType.Operator, "+", "-"))
            {
                string op = Previous().Value;
                Expression right = ParseMulDiv();
                ExpressionType type = op == "+" ? ExpressionType.Arithmetic_Addition : ExpressionType.Arithmetic_Subtraction;
                expr = new Expression(type, expr, right);
            }

            return expr;
        }

        internal Expression ParseMulDiv()
        {
            Expression expr = ParseExponent();

            while (Match(TokenType.Operator, "*", "/"))
            {
                string op = Previous().Value;
                Expression right = ParseExponent();
                ExpressionType type = op == "*" ? ExpressionType.Arithmetic_Multiplication : ExpressionType.Arithmetic_Division;
                expr = new Expression(type, expr, right);
            }

            return expr;
        }

        internal Expression ParseExponent()
        {
            Expression left = ParseUnary();

            if (Match(TokenType.Operator, "^"))
            {
                Expression right = ParseExponent(); // recurse here!
                return new Expression(ExpressionType.Arithmetic_Exponent, left, right);
            }

            return left;
        }

        internal Expression ParseUnary()
        {
            if (Match(TokenType.Operator, "-"))
            {
                Expression operand = ParseUnary();
                return new Expression(ExpressionType.Unary_Negation, operand);
            }

            if (Match(TokenType.Operator, "+"))
            {
                return ParseUnary(); // Unary plus is a no-op
            }

            return ParsePrimary();
        }

        internal Expression ParsePrimary()
        {
            Expression expr = ParseAtomicPrimary();

            // Detect implicit multiplication
            while (Check(TokenType.Number) || Check(TokenType.Identifier) || Check(TokenType.LeftParen))
            {
                Expression right = ParseAtomicPrimary();
                expr = new Expression(ExpressionType.Arithmetic_Multiplication, expr, right);
            }

            return expr;
        }

        internal Expression ParseAtomicPrimary()
        {
            if (Match(TokenType.Number))
            {
                float value = float.Parse(Previous().Value);
                return new Expression(ExpressionType.Number, value);
            }

            if (Match(TokenType.Identifier))
            {
                string ident = Previous().Value;

                if (KnownConstants.Contains(ident))
                {
                    //return new Expression(GetConstantExpressionType(ident));
                    return new Expression(ExpressionTools.GetConstantType(ident));
                }

                if (KnownFunctions.Contains(ident))
                {
                    Consume(TokenType.LeftParen, "Expected '(' after function name.");
                    var args = new List<Expression>();
                    if (!Check(TokenType.RightParen))
                    {
                        do
                        {
                            args.Add(ParsePrimarySub());
                        } while (Match(TokenType.Comma));
                    }
                    Consume(TokenType.RightParen, "Expected ')' after function arguments.");
                    //return new Expression(GetFunctionExpressionType(ident), args.ToArray());
                    return new Expression(ExpressionTools.GetFunctionType(ident), args.ToArray());
                }

                if (ident.Length == 1 && char.IsLetter(ident[0]))
                {
                    return new Expression(ExpressionType.Variable, ident);
                }

                Console.Error.WriteLine($"Unknown identifier: {ident}");
                return null;
            }

            if (Match(TokenType.LeftParen))
            {
                Expression expr = ParsePrimarySub();
                Consume(TokenType.RightParen, "Expected ')'");
                return expr;
            }

            if (!IsAtEnd())
            {
                var token = Peek();
                Console.Error.WriteLine($"Unexpected token while parsing: {token.Type} '{token.Value}' at index {current}");
            }
            else
            {
                Console.Error.WriteLine("Unexpected end of expression while parsing.");
            }
            return null;
        }

        private Expression ParsePrimarySub() {
            return ParseAddSub();
        }

        // Token helpers
        private bool Match(TokenType type, params string[] values)
        {
            if (Check(type) && (values.Length == 0 || System.Array.Exists(values, v => v == Peek().Value)))
            {
                Advance();
                return true;
            }
            return false;
        }

        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
            return false;
        }

        private void Consume(TokenType type, string errorMessage)
        {
            if (!Match(type))
                Console.Error.WriteLine(errorMessage);
        }

        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;
        private Token Advance() => tokens[current++];
        private bool IsAtEnd() => current >= tokens.Count;
        private Token Peek() => tokens[current];
        private Token Previous() => tokens[current - 1];
    }
}
