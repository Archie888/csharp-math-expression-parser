using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using MathExpressions;
using TestUtils;

[TestFixture]
public class ExpressionParserTests
{
    private ExpressionParser parser;

    [SetUp]
    public void SetUp()
    {
        parser = ExpressionParser.Instance;
    }

    private void SetupTokens(string input)
    {
        Console.WriteLine(input);
        var tokenizer = ExpressionTokenizer.Instance;
        var tokenList = tokenizer.Tokenize(input);

        Assert.IsNotNull(tokenList, $"Tokenization failed for input: {input}");

        typeof(ExpressionParser)
            .GetField("tokens", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(parser, tokenList);

        typeof(ExpressionParser)
            .GetField("current", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(parser, 0);
    }

    // -------------------------
    // T01–T07: ParsePrimary
    // -------------------------

    [Test]
    public void T01_ParsePrimary_NumberLiteral_ReturnsNumberExpression()
    {
        SetupTokens("42");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Number, expr.Type);
        Assert.AreEqual(42f, expr.NumberValue);
    }

    [Test]
    public void T02a_ParsePrimary_Variable_ReturnsVariableExpression()
    {
        SetupTokens("x");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Variable, expr.Type);
        Assert.AreEqual("x", expr.VariableName);
    }

    [Test]
    public void T02b_ParsePrimary_ImplicitMultiplication_NumberVariable()
    {
        SetupTokens("2x");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);

        // Should be: 2 * x
        Assert.AreEqual(ExpressionType.Arithmetic_Multiplication, expr.Type);

        var left = expr.SubExpressions[0];
        var right = expr.SubExpressions[1];

        Assert.AreEqual(ExpressionType.Number, left.Type);
        Assert.AreEqual(2f, left.NumberValue);

        Assert.AreEqual(ExpressionType.Variable, right.Type);
        Assert.AreEqual("x", right.VariableName);
    }

    [Test]
    public void T02c_ParsePrimary_ImplicitMultiplication_VariableVariable()
    {
        SetupTokens("a b");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);

        // Should be: a * b
        Assert.AreEqual(ExpressionType.Arithmetic_Multiplication, expr.Type);

        var left = expr.SubExpressions[0];
        var right = expr.SubExpressions[1];

        Assert.AreEqual(ExpressionType.Variable, left.Type);
        Assert.AreEqual("a", left.VariableName);

        Assert.AreEqual(ExpressionType.Variable, right.Type);
        Assert.AreEqual("b", right.VariableName);
    }

    [Test]
    public void T02d_ParsePrimary_ImplicitMultiplication_NumberParen()
    {
        SetupTokens("2(x + y)");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);

        // Should be: 2 * (x + y)
        Assert.AreEqual(ExpressionType.Arithmetic_Multiplication, expr.Type);

        var left = expr.SubExpressions[0];
        var right = expr.SubExpressions[1];

        // Left should be number 2
        Assert.AreEqual(ExpressionType.Number, left.Type);
        Assert.AreEqual(2f, left.NumberValue);

        // Right should be (x + y)
        Assert.AreEqual(ExpressionType.Arithmetic_Addition, right.Type);
        Assert.AreEqual(ExpressionType.Variable, right.SubExpressions[0].Type);
        Assert.AreEqual("x", right.SubExpressions[0].VariableName);
        Assert.AreEqual(ExpressionType.Variable, right.SubExpressions[1].Type);
        Assert.AreEqual("y", right.SubExpressions[1].VariableName);
    }

    [Test]
    public void T03a_ParsePrimary_KnownConstant_Pi_ReturnsConstantExpression()
    {
        SetupTokens("pi");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Constant_Pi, expr.Type);
    }

    [Test]
    public void T03b_ParsePrimary_KnownConstant_E_ReturnsConstantExpression()
    {
        SetupTokens("e");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Constant_E, expr.Type);
    }

    [Test]
    public void T04_ParsePrimary_KnownFunctionWithArgs_ReturnsFunctionExpression()
    {
        SetupTokens("max(3, 5)");
        var expr = parser.ParsePrimary();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Function_Max, expr.Type);
        Assert.AreEqual(2, expr.SubExpressions.Count);
    }

    [Test]
    public void T05_ParsePrimary_UnknownIdentifier_LogsErrorAndReturnsNull()
    {
        using (var console = new ConsoleOutput())
        {
            SetupTokens("foobar");
            var expr = parser.ParsePrimary();
            ExpressionTools.LogExpression(expr);
            StringAssert.Contains("Unknown identifier: foobar", console.GetOutput());
            Assert.IsNull(expr);
        }
    }

    [Test]
    public void T06_ParsePrimary_MissingClosingParen_LogsError()
    {
        using (var console = new ConsoleOutput())
        {
            SetupTokens("max(3, 5");
            var expr = parser.ParsePrimary();
            ExpressionTools.LogExpression(expr);
            StringAssert.Contains("Expected ')' after function arguments.", console.GetOutput());

            // It logs the error but still returns a function expression with args
            Assert.IsNotNull(expr);
            Assert.AreEqual(ExpressionType.Function_Max, expr.Type);
            Assert.AreEqual(2, expr.SubExpressions.Count);
        }
    }
    
    [Test]
    public void T07_Tokenize_InvalidCharacter_LogsError()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = ExpressionTokenizer.Instance.Tokenize("$");
            StringAssert.Contains("Invalid character in expression: U+0024", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }

    // -------------------------
    // T08–T09: ParseUnary
    // -------------------------

    [Test]
    public void T08_ParseUnary_NegativeNumber_ReturnsNegationExpression()
    {
        SetupTokens("-5");
        var expr = parser.ParseUnary();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Unary_Negation, expr.Type);
        Assert.AreEqual(5f, expr.SubExpressions[0].NumberValue);
    }

    [Test]
    public void T09_ParseUnary_UnaryPlus_IsNoOp()
    {
        SetupTokens("+5");
        var expr = parser.ParseUnary();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Number, expr.Type);
        Assert.AreEqual(5f, expr.NumberValue);
    }

    // -------------------------
    // T10: ParseExponent
    // -------------------------

    [Test]
    public void T10_ParseExponent_RightAssociative()
    {
        SetupTokens("2 ^ 3 ^ 2");
        var expr = parser.ParseExponent();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Arithmetic_Exponent, expr.Type);
        Assert.AreEqual(2f, expr.SubExpressions[0].NumberValue);
        Assert.AreEqual(ExpressionType.Arithmetic_Exponent, expr.SubExpressions[1].Type);
    }

    // -------------------------
    // T11: ParseMulDiv
    // -------------------------

    [Test]
    public void T11_ParseMulDiv_ChainedMultiplicationAndDivision()
    {
        SetupTokens("6 / 3 * 2");
        var expr = parser.ParseMulDiv();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Arithmetic_Multiplication, expr.Type);
        Assert.AreEqual(ExpressionType.Arithmetic_Division, expr.SubExpressions[0].Type);
    }

    // -------------------------
    // T12: ParseAddSub
    // -------------------------

    [Test]
    public void T12_ParseAddSub_ChainedAdditionAndSubtraction()
    {
        SetupTokens("1 + 2 - 3");
        var expr = parser.ParseAddSub();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Arithmetic_Subtraction, expr.Type);
        Assert.AreEqual(ExpressionType.Arithmetic_Addition, expr.SubExpressions[0].Type);
    }

    // -------------------------
    // T13–T14: ParseComparison
    // -------------------------

    [Test]
    public void T13_ParseComparison_EqualOperator()
    {
        SetupTokens("4 = 4");
        var expr = parser.ParseComparison();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Comparison_Equal, expr.Type);
    }

    [Test]
    public void T14_ParseComparison_GreaterThanOrEqualOperator()
    {
        SetupTokens("x >= 0");
        var expr = parser.ParseComparison();
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Comparison_GreaterThanOrEqual, expr.Type);
    }

    // -------------------------
    // T15: ParseExpression (Integration)
    // -------------------------

    [Test]
    public void T15_ParseExpression_ComplexTree()
    {
        var exprStr = "(1 + 2) * (3 + 4)";
        Console.WriteLine(exprStr);
        var expr = parser.ParseExpression(exprStr);
        ExpressionTools.LogExpression(expr);
        Assert.AreEqual(ExpressionType.Arithmetic_Multiplication, expr.Type);
        Assert.AreEqual(ExpressionType.Arithmetic_Addition, expr.SubExpressions[0].Type);
        Assert.AreEqual(ExpressionType.Arithmetic_Addition, expr.SubExpressions[1].Type);
    }

    [Test]
    public void T16_ParseExpression_ChristmasTree_WithImplicitMultiplication()
    {
        // No * between (...) and max(...) → should still be parsed as multiplication
        var exprStr = "(log(e) + sin(pi / 2)) max(1, 2 ^ 3) = sqrt(49)";
        Console.WriteLine(exprStr);
        var expr = parser.ParseExpression(exprStr);
        ExpressionTools.LogExpression(expr);

        // Top level: =
        Assert.AreEqual(ExpressionType.Comparison_Equal, expr.Type);

        var left = expr.SubExpressions[0];
        var right = expr.SubExpressions[1];

        // Right should be sqrt(49)
        Assert.AreEqual(ExpressionType.Function_Sqrt, right.Type);
        Assert.AreEqual(49f, right.SubExpressions[0].NumberValue);

        // Left should be: (log(e) + sin(pi / 2)) * max(1, 2 ^ 3)
        Assert.AreEqual(ExpressionType.Arithmetic_Multiplication, left.Type);

        var addExpr = left.SubExpressions[0];
        var maxExpr = left.SubExpressions[1];

        Assert.AreEqual(ExpressionType.Arithmetic_Addition, addExpr.Type);
        Assert.AreEqual(ExpressionType.Function_Max, maxExpr.Type);

        // log(e)
        var logExpr = addExpr.SubExpressions[0];
        Assert.AreEqual(ExpressionType.Function_Log, logExpr.Type);
        Assert.AreEqual(ExpressionType.Constant_E, logExpr.SubExpressions[0].Type);

        // sin(pi / 2)
        var sinExpr = addExpr.SubExpressions[1];
        Assert.AreEqual(ExpressionType.Function_Sin, sinExpr.Type);
        var div = sinExpr.SubExpressions[0];
        Assert.AreEqual(ExpressionType.Arithmetic_Division, div.Type);
        Assert.AreEqual(ExpressionType.Constant_Pi, div.SubExpressions[0].Type);
        Assert.AreEqual(2f, div.SubExpressions[1].NumberValue);

        // max(1, 2 ^ 3)
        Assert.AreEqual(2, maxExpr.SubExpressions.Count);
        Assert.AreEqual(1f, maxExpr.SubExpressions[0].NumberValue);

        var pow = maxExpr.SubExpressions[1];
        Assert.AreEqual(ExpressionType.Arithmetic_Exponent, pow.Type);
        Assert.AreEqual(2f, pow.SubExpressions[0].NumberValue);
        Assert.AreEqual(3f, pow.SubExpressions[1].NumberValue);
    }
}
