using System.Linq;
using Lox.Exception;
using NUnit.Framework;

namespace Lox.Tests;

public sealed class ParserTests
{
	[SetUp]
	public void CreateErrorReporter() => error = new TestErrorReporter();

	private TestErrorReporter error = null!;

	[TestCase("false", "False")]
	[TestCase("true", "True")]
	[TestCase("nil", null)]
	[TestCase("25", "25")]
	public void ParseSingleLiteralExpression(string code, string expectedValue)
	{
		var parser = new Parser(new Scanner(code, error).Tokens);
		Assert.That(parser.Expressions.FirstOrDefault(), Is.InstanceOf<Expression.Literal>());
		var literalExpression = parser.Expressions.FirstOrDefault() as Expression.Literal;
		Assert.That(literalExpression?.literal?.ToString(), Is.EqualTo(expectedValue));
	}

	[Test]
	public void ParseUnknownExpression() =>
		Assert.That(() => new Parser(new Scanner("random", error).Tokens),
			Throws.InstanceOf<UnknownExpression>());
}