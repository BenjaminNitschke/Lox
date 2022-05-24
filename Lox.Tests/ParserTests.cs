using System.Linq;
using NUnit.Framework;

namespace Lox.Tests;

public sealed class ParserTests
{
	[TestCase("/ 2 30")]
	public void ParseInvalidFactorBinaryExpression(string code) => Assert.That(() => GetParser(code), Throws.InstanceOf<Parser.UnknownExpression>());

	[Test]
	public void ParseUnknownExpression() =>
		Assert.That(() => GetParser("/"), Throws.InstanceOf<Parser.UnknownExpression>());

	[Test]
	public void ParseMissingClosingParenthesisGroupingExpression() =>
		Assert.That(() => GetParser("(a + b"), Throws.InstanceOf<Parser.MissingClosingParenthesis>());

	[TestCase("false", "False", TokenType.False)]
	[TestCase("true", "True", TokenType.True)]
	[TestCase("nil", null, TokenType.Nil)]
	[TestCase("25", "25", TokenType.Number)]
	public void ParseSinglePrimaryExpression(string code, string expectedValue,
		TokenType expectedTokenType)
	{
		var parser = new Parser(new Scanner(code).Tokens);
		Assert.That(parser.Expressions.FirstOrDefault(), Is.InstanceOf<Expression.LiteralExpression>());
		var literalExpression = parser.Expressions.FirstOrDefault() as Expression.LiteralExpression;
		Assert.That(literalExpression?.Literal?.ToString(), Is.EqualTo(expectedValue));
		Assert.That(literalExpression?.TokenType.Type, Is.EqualTo(expectedTokenType));
	}

	[TestCase("!false", "!", "False")]
	[TestCase("!true", "!", "True")]
	[TestCase("-25", "-", "25")]
	public void ParseSingleUnaryExpressions(string code, string expectedOperator,
		string expectedSecondExpressionValue)
	{
		var parser = GetParser(code);
		Assert.That(parser.Expressions.FirstOrDefault(), Is.InstanceOf<Expression.UnaryExpression>());
		var unaryExpression = parser.Expressions.FirstOrDefault() as Expression.UnaryExpression;
		Assert.That(unaryExpression?.OperatorToken.Lexeme, Is.EqualTo(expectedOperator));
		var literalExpression = unaryExpression?.RightExpression as Expression.LiteralExpression;
		Assert.That(literalExpression?.Literal?.ToString(),
			Is.EqualTo(expectedSecondExpressionValue));
	}

	[TestCase("25 + 2", "25", "+", "2")]
	[TestCase("25 * 2", "25", "*", "2")]
	[TestCase("25 > 2", "25", ">", "2")]
	[TestCase("25 < 2", "25", "<", "2")]
	[TestCase("25 == 2", "25", "==", "2")]
	[TestCase("25 != 2", "25", "!=", "2")]
	[TestCase("30 / 2", "30", "/", "2")]
	[TestCase("30 / false", "30", "/", "False")]
	[TestCase("true * false", "True", "*", "False")]
	public void ParseSingleBinaryExpression(string code, string expectedLeftExpression,
		string expectedOperator, string expectedRightExpression)
	{
		var parser = GetParser(code);
		Assert.That(parser.Expressions.FirstOrDefault(), Is.InstanceOf<Expression.BinaryExpression>());
		var binaryExpression = parser.Expressions.FirstOrDefault() as Expression.BinaryExpression;
		Assert.That(binaryExpression?.LeftExpression, Is.InstanceOf<Expression.LiteralExpression>());
		var leftExpression = binaryExpression?.LeftExpression as Expression.LiteralExpression;
		Assert.That(leftExpression?.Literal?.ToString(), Is.EqualTo(expectedLeftExpression));
		Assert.That(binaryExpression?.OperatorToken.Lexeme, Is.EqualTo(expectedOperator));
		var rightExpression = binaryExpression?.RightExpression as Expression.LiteralExpression;
		Assert.That(rightExpression?.Literal?.ToString(), Is.EqualTo(expectedRightExpression));
	}

	[TestCase("25 * 2 + 30 / 2", 4)]
	[TestCase("-25 * 2 + 30 / 2", 5)]
	[TestCase("-25 * 2 + 30 / 2 > 5", 6)]
	public void ParseMultipleBinaryExpressions(string code, int expectedNumberOfExpressions)
	{
		var parser = GetParser(code);
		Assert.That(actual: GetExpressionsCount(parser.Expressions.FirstOrDefault()!),
			Is.EqualTo(expectedNumberOfExpressions));
	}

	[Test]
	public void ParseInvalidTargetAssignmentExpression() =>
		Assert.That(() => GetParser("1 = b"), Throws.InstanceOf<Parser.InvalidAssignmentTarget>());

	[Test]
	public void ParseAssignmentExpression()
	{
		var parser = GetParser("a = b");
		Assert.That(parser.Expressions.FirstOrDefault(), Is.InstanceOf<Expression.AssignmentExpression>());
	}

	[Test]
	public void ParseGroupingExpression()
	{
		var parser = GetParser("(a + b)");
		Assert.That(parser.Expressions.FirstOrDefault(), Is.InstanceOf<Expression.GroupingExpression>());
	}

	private static Parser GetParser(string code) => new(new Scanner(code).Tokens);

	private static int GetExpressionsCount(Expression expression) =>
		expression switch
		{
			Expression.BinaryExpression binaryExpression =>
				GetExpressionsCount(binaryExpression.LeftExpression) +
				GetExpressionsCount(binaryExpression.RightExpression),
			Expression.UnaryExpression unaryExpression => 1 + GetExpressionsCount(unaryExpression.RightExpression),
			Expression.LiteralExpression => 1,
			_ => 0 //ncrunch: no coverage
		};
}