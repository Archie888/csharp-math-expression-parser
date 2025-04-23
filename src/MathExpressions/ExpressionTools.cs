using System;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace MathExpressions
{
    public static class ExpressionTools
    {

        public static string GetSymbol(ExpressionType type)
        {
            return type switch
            {
                // Arithmetic
                ExpressionType.Arithmetic_Addition => "+",
                ExpressionType.Arithmetic_Subtraction => "-",
                ExpressionType.Arithmetic_Multiplication => "×",
                ExpressionType.Arithmetic_Division => "÷",
                ExpressionType.Arithmetic_Exponent => "^",

                // Comparison
                ExpressionType.Comparison_Equal => "=",
                ExpressionType.Comparison_NotEqual => "≠",
                ExpressionType.Comparison_LessThan => "<",
                ExpressionType.Comparison_LessThanOrEqual => "≤",
                ExpressionType.Comparison_GreaterThan => ">",
                ExpressionType.Comparison_GreaterThanOrEqual => "≥",

                // Constants
                ExpressionType.Constant_Pi => "π",
                ExpressionType.Constant_E => "e",

                // Functions (optional if you want label text instead of math symbol)
                ExpressionType.Function_Sqrt => "√",
                ExpressionType.Function_Logarithm => "log",
                ExpressionType.Function_Log => "log",
                ExpressionType.Function_Sin => "sin",
                ExpressionType.Function_Cos => "cos",
                ExpressionType.Function_Tan => "tan",
                ExpressionType.Function_Min => "min",
                ExpressionType.Function_Max => "max",

                _ => "?"
            };
        }

        public static ExpressionType GetFunctionType(string name)
        {
            return name switch
            {
                "sin" => ExpressionType.Function_Sin,
                "cos" => ExpressionType.Function_Cos,
                "tan" => ExpressionType.Function_Tan,
                "sqrt" => ExpressionType.Function_Sqrt,
                "log" => ExpressionType.Function_Log,
                "max" => ExpressionType.Function_Max,
                "min" => ExpressionType.Function_Min,
                _ => throw new Exception($"Unknown function: {name}")
            };
        }

        public static ExpressionType GetConstantType(string name)
        {
            return name switch
            {
                "pi" => ExpressionType.Constant_Pi,
                "e" => ExpressionType.Constant_E,
                _ => throw new Exception($"Unknown constant: {name}")
            };
        }

        public static ExpressionType GetComparisonType(string op)
        {
            return op switch
            {
                "=" or "==" => ExpressionType.Comparison_Equal,
                "!=" => ExpressionType.Comparison_NotEqual,
                "<" => ExpressionType.Comparison_LessThan,
                "<=" => ExpressionType.Comparison_LessThanOrEqual,
                ">" => ExpressionType.Comparison_GreaterThan,
                ">=" => ExpressionType.Comparison_GreaterThanOrEqual,
                _ => throw new Exception($"Unknown comparison operator: {op}")
            };
        }

        public static void LogExpression(Expression expr, string indent = "", bool isLast = true)
        {
            if (expr == null)
            {
                #if UNITY_5_3_OR_NEWER
                    Debug.Log(indent + "NULL");
                #else
                    Console.WriteLine(indent + "NULL");
                #endif
                return;
            }

            string marker = isLast ? "└── " : "├── ";
            string line = indent + marker + expr.Type;

            if (expr.Type == ExpressionType.Number)
                line += ": " + expr.NumberValue;
            else if (expr.Type == ExpressionType.Variable)
                line += ": " + expr.VariableName;

            #if UNITY_5_3_OR_NEWER
                Debug.Log(line);
            #else
                Console.WriteLine(line);
            #endif

            indent += isLast ? "    " : "│   ";

            for (int i = 0; i < expr.SubExpressions.Count; i++)
            {
                bool last = i == expr.SubExpressions.Count - 1;
                LogExpression(expr.SubExpressions[i], indent, last);
            }
        }
    }
}
