using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MathExpressions;
using TestUtils;

[TestFixture]
public class ExpressionTokenizerTests
{
    private ExpressionTokenizer tokenizer;

    [SetUp]
    public void SetUp()
    {
        tokenizer = ExpressionTokenizer.Instance;
    }

    [Test]
    public void Tokenize_SimpleAddition_ReturnsCorrectTokens()
    {
        var tokens = tokenizer.Tokenize("3 + 4");

        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual("3", tokens[0].Value);
        Assert.AreEqual("+", tokens[1].Value);
        Assert.AreEqual("4", tokens[2].Value);
    }

    [Test]
    public void Tokenize_WhitespaceOnly_IgnoresWhitespace()
    {
        var tokens = tokenizer.Tokenize("    ");
        Assert.AreEqual(0, tokens.Count);
    }

    [Test]
    public void Tokenize_IntegerAndDecimalNumbers_ParsesCorrectly()
    {
        var tokens = tokenizer.Tokenize("42 3.14");

        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(TokenType.Number, tokens[0].Type);
        Assert.AreEqual("42", tokens[0].Value);
        Assert.AreEqual("3.14", tokens[1].Value);
    }

    [Test]
    public void Tokenize_Identifiers_ParsesLettersAsIdentifiers()
    {
        var tokens = tokenizer.Tokenize("x y foo");

        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual("x", tokens[0].Value);
        Assert.AreEqual("y", tokens[1].Value);
        Assert.AreEqual("foo", tokens[2].Value);
    }

    [Test]
    public void Tokenize_SingleCharacterOperators_ParsesCorrectly()
    {
        var tokens = tokenizer.Tokenize("+ - * / = < > ^");

        List<string> expected = new() { "+", "-", "*", "/", "=", "<", ">", "^" };
        Assert.AreEqual(expected.Count, tokens.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(TokenType.Operator, tokens[i].Type);
            Assert.AreEqual(expected[i], tokens[i].Value);
        }
    }

    [Test]
    public void Tokenize_TwoCharacterOperators_ParsesCorrectly()
    {
        var tokens = tokenizer.Tokenize("== != <= >=");

        List<string> expected = new() { "==", "!=", "<=", ">=" };
        Assert.AreEqual(expected.Count, tokens.Count);

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(TokenType.Operator, tokens[i].Type);
            Assert.AreEqual(expected[i], tokens[i].Value);
        }
    }

    [Test]
    public void Tokenize_Parentheses_ParsesCorrectly()
    {
        var tokens = tokenizer.Tokenize("(3 + 4)");

        Assert.AreEqual(TokenType.LeftParen, tokens[0].Type);
        Assert.AreEqual("(", tokens[0].Value);
        Assert.AreEqual("3", tokens[1].Value);
        Assert.AreEqual("+", tokens[2].Value);
        Assert.AreEqual("4", tokens[3].Value);
        Assert.AreEqual(TokenType.RightParen, tokens[4].Type);
        Assert.AreEqual(")", tokens[4].Value);
    }

    [Test]
    public void Tokenize_CommaToken_ParsesCorrectly()
    {
        var tokens = tokenizer.Tokenize("max(x, y)");

        Assert.AreEqual("max", tokens[0].Value);
        Assert.AreEqual("(", tokens[1].Value);
        Assert.AreEqual("x", tokens[2].Value);
        Assert.AreEqual(",", tokens[3].Value);
        Assert.AreEqual("y", tokens[4].Value);
        Assert.AreEqual(")", tokens[5].Value);
    }

    [Test]
    public void Tokenize_ComplexExpression_ParsesAllTokens()
    {
        var tokens = tokenizer.Tokenize("sum(x1 + 3.5, y2) != 7");

        List<string> expected = new() { "sum", "(", "x", "1", "+", "3.5", ",", "y", "2", ")", "!=", "7" };

        Assert.IsNotNull(tokens);
        Assert.AreEqual(12, tokens.Count); // Due to tokenizer rules, x1 and y2 will be split into "x" + "1", "y" + "2"

        Assert.AreEqual("sum", tokens[0].Value);
        Assert.AreEqual("(", tokens[1].Value);
        Assert.AreEqual("x", tokens[2].Value);
        Assert.AreEqual("1", tokens[3].Value); // Because "1" is a digit, split from "x"
        Assert.AreEqual("+", tokens[4].Value);
        Assert.AreEqual("3.5", tokens[5].Value);
        Assert.AreEqual(",", tokens[6].Value);
        Assert.AreEqual("y", tokens[7].Value);
        Assert.AreEqual("2", tokens[8].Value);
        Assert.AreEqual(")", tokens[9].Value);
        Assert.AreEqual("!=", tokens[10].Value);
        Assert.AreEqual("7", tokens[11].Value);
    }

    [Test]
    public void Tokenize_InvalidCharacter_ReturnsNullAndLogsError()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = tokenizer.Tokenize("3 + $");
            StringAssert.Contains("Invalid character in expression: U+0024", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }
}

[TestFixture]
public class ExpressionTokenizerInvalidInputTests
{
    private ExpressionTokenizer tokenizer;

    [SetUp]
    public void SetUp()
    {
        tokenizer = ExpressionTokenizer.Instance;
    }

    [Test]
    public void Tokenize_InvalidSymbolDollarSign_ReturnsNull()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = tokenizer.Tokenize("3 + $");
            StringAssert.Contains("Invalid character in expression: U+0024", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }

    [Test]
    public void Tokenize_InvalidSymbolAtSign_ReturnsNull()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = tokenizer.Tokenize("a @ b");
            StringAssert.Contains("Invalid character in expression: U+0040", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }

    [Test]
    public void Tokenize_InvalidSymbolSemicolon_ReturnsNull()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = tokenizer.Tokenize("5 + 2;");
            StringAssert.Contains("Invalid character in expression: U+003B", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }

    [Test]
    public void Tokenize_MixedValidAndInvalidSymbols_ReturnsNull()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = tokenizer.Tokenize("sum(x, y) # max(a, b)");
            StringAssert.Contains("Invalid character in expression: U+0023", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }

    [Test]
    public void Tokenize_EmojiCharacter_ReturnsNull()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = tokenizer.Tokenize("x 😊 y");
            StringAssert.Contains("Invalid character in expression: U+D83D", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }

    [Test]
    public void Tokenize_OnlyInvalidCharacters_ReturnsNull()
    {
        using (var console = new ConsoleOutput())
        {
            var tokens = tokenizer.Tokenize("!@#$%^&*");
            StringAssert.Contains("Invalid character in expression: U+0021", console.GetOutput());
            Assert.IsNull(tokens);
        }
    }

    [Test]
    public void Tokenize_EmptyString_ReturnsEmptyTokenList()
    {
        var tokens = tokenizer.Tokenize("");
        Assert.IsNotNull(tokens);
        Assert.AreEqual(0, tokens.Count);
    }
}
