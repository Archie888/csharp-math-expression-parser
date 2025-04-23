using NUnit.Framework;
using MathExpressions;
using System;
using TestUtils;

[TestFixture]
public class ExpressionConstructorTests
{
    [Test]
    public void Constructor_NumberExpression_SetsNumberValue()
    {
        var expr = new Expression(ExpressionType.Number, 42.5f);

        Assert.AreEqual(ExpressionType.Number, expr.Type);
        Assert.AreEqual(42.5f, expr.NumberValue);
        Assert.IsNull(expr.VariableName);
        Assert.IsNotNull(expr.SubExpressions);
        Assert.IsEmpty(expr.SubExpressions);
    }

    [Test]
    public void Constructor_NumberExpression_InvalidType_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var expr = new Expression(ExpressionType.Variable, 10f);
        });
    }

    [Test]
    public void Constructor_VariableExpression_SetsVariableName()
    {
        var expr = new Expression(ExpressionType.Variable, "x");

        Assert.AreEqual(ExpressionType.Variable, expr.Type);
        Assert.AreEqual("x", expr.VariableName);
        Assert.IsNull(expr.NumberValue);
        Assert.IsNotNull(expr.SubExpressions);
        Assert.IsEmpty(expr.SubExpressions);
    }

    [Test]
    public void Constructor_VariableExpression_InvalidType_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var expr = new Expression(ExpressionType.Number, "invalid");
        });
    }

    [Test]
    public void Constructor_GeneralPurpose_SetsSubExpressions()
    {
        var left = new Expression(ExpressionType.Number, 3f);
        var right = new Expression(ExpressionType.Number, 4f);
        var expr = new Expression(ExpressionType.Arithmetic_Addition, left, right);

        Assert.AreEqual(ExpressionType.Arithmetic_Addition, expr.Type);
        Assert.AreEqual(2, expr.SubExpressions.Count);
        Assert.AreSame(left, expr.SubExpressions[0]);
        Assert.AreSame(right, expr.SubExpressions[1]);
        Assert.IsNull(expr.NumberValue);
        Assert.IsNull(expr.VariableName);
    }

    [Test]
    public void Constructor_GeneralPurpose_WithNoSubExpressions_AllowsEmptyList()
    {
        var expr = new Expression(ExpressionType.Constant_Pi);

        Assert.AreEqual(ExpressionType.Constant_Pi, expr.Type);
        Assert.IsNotNull(expr.SubExpressions);
        Assert.IsEmpty(expr.SubExpressions);
        Assert.IsNull(expr.NumberValue);
        Assert.IsNull(expr.VariableName);
    }

    [Test]
    public void UnaryNegation_ConstructsCorrectly()
    {
        // Arrange
        var operand = new Expression(ExpressionType.Number, 5f);

        // Act
        var negationExpr = new Expression(ExpressionType.Unary_Negation, operand);

        // Assert
        Assert.AreEqual(ExpressionType.Unary_Negation, negationExpr.Type);
        Assert.AreEqual(1, negationExpr.SubExpressions.Count);
        Assert.AreEqual(5f, negationExpr.SubExpressions[0].NumberValue);
    }
}
