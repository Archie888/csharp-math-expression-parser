using System;
using System.Collections.Generic;

namespace MathExpressions
{
    public class Expression
    {
        public ExpressionType Type { get; private set; }
        public List<Expression> SubExpressions { get; private set; } = new();
        public float? NumberValue { get; private set; }
        public string VariableName { get; private set; }

        // Constructor for Numbers
        public Expression(ExpressionType type, float value)
        {
            if (type != ExpressionType.Number)
                throw new ArgumentException($"ExpressionType '{type}' is invalid for a number constructor.");

            Type = type;
            NumberValue = value;
        }

        // Constructor for Variables
        public Expression(ExpressionType type, string variableName)
        {
            if (type != ExpressionType.Variable)
                throw new ArgumentException($"ExpressionType '{type}' is invalid for a variable constructor.");

            Type = type;
            VariableName = variableName;
        }

        // General-purpose constructor
        public Expression(ExpressionType type, params Expression[] subExprs)
        {
            // You can optionally validate the expected arity here too
            Type = type;

            if (subExprs != null)
                SubExpressions.AddRange(subExprs);

            // Optional: Validate arity
            if (type == ExpressionType.Unary_Negation && SubExpressions.Count != 1)
                throw new ArgumentException("Unary_Negation must have exactly 1 sub-expression.");
        }
    }
}
