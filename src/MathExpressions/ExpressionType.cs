namespace MathExpressions
{
    public enum ExpressionType
    {
        // Binary arithmetic operators
        Arithmetic_MIN,
        Arithmetic_Addition,
        Arithmetic_Subtraction,
        Arithmetic_Multiplication,
        Arithmetic_Division,
        Arithmetic_Exponent,
        Arithmetic_MAX,

        // Binary comparison operators
        Comparison_MIN,
        Comparison_Equal,
        Comparison_NotEqual,
        Comparison_LessThan,
        Comparison_LessThanOrEqual,
        Comparison_GreaterThan,
        Comparison_GreaterThanOrEqual,
        Comparison_MAX,

        // Unary
        Unary_Negation,

        // Functions
        Function_MIN,
        Function_Sqrt,
        Function_Logarithm,
        Function_Log,
        Function_Sin,
        Function_Cos,
        Function_Tan,
        Function_Min,
        Function_Max,
        Function_MAX,

        // Constants
        Constant_MIN,
        Constant_Pi,
        Constant_E,
        Constant_MAX,

        // Atoms
        Number,
        Variable
    }
}
