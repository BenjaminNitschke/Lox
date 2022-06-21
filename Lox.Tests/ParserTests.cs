using Expressions;

namespace Lox.Tests;

public sealed class ParserTests
{
	[TestCase("/ 2 30")]
	public void ParseInvalidFactorBinaryExpression(string code) =>
		Assert.That(() => GetParser(code).Parse(), Throws.InstanceOf<Parser.UnknownExpression>());

	[Test]
	public void ParseUnknownExpression() =>
		Assert.That(() => GetParser("/").Parse(), Throws.InstanceOf<Parser.UnknownExpression>());

	[Test]
	public void ParseMissingClosingParenthesisGroupingExpression() =>
		Assert.That(() => GetParser("(a + b").Parse(),
			Throws.InstanceOf<Parser.MissingClosingParenthesis>());

	[Test]
	public void ParseMissingVariableNameExpression() =>
		Assert.That(() => GetParser("var;").Parse(), Throws.InstanceOf<Parser.MissingVariableName>());

	[Test]
	public void ParseMissingSemicolonExpression() =>
		Assert.That(() => GetParser("a + b").Parse(), Throws.InstanceOf<Parser.MissingSemicolon>());

	[Test]
	public void ParseMissingBrace() =>
		Assert.That(() => GetParser("{ a + b; ").Parse(),
			Throws.InstanceOf<Parser.MissingRightBrace>());

	[Test]
	public void TestArgumentsMoreThanAllowed()
	{
		var funcArguments = GenerateFunctionArguments(256);
		Assert.That(() => GetParser("fun test(" + funcArguments + ");").Parse(),
			Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void ParseFunctionExpressionWithMaximumArguments()
	{
		var funcArguments = GenerateFunctionArguments(256);
		Assert.That(() => GetParser("test(" + funcArguments + ");").Expressions,
			Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void FunctionMissingLeftBrace() =>
		Assert.That(() => GetParser("fun test();").Parse(),
			Throws.InstanceOf<Parser.MissingLeftBrace>());

	private static string GenerateFunctionArguments(int count)
	{
		var argumentsBuilder = new StringBuilder();
		for (var i = 0; i < count; i++)
			argumentsBuilder.Append(i == count - 1
				? "a"
				: "a,");
		return argumentsBuilder.ToString();
	}

	[TestCase("false", "False", TokenType.False)]
	[TestCase("true", "True", TokenType.True)]
	[TestCase("nil", null, TokenType.Nil)]
	[TestCase("25", "25", TokenType.Number)]
	public void ParseSinglePrimaryExpression(string code, string expectedValue,
		TokenType expectedTokenType)
	{
		var resultExpression =
			new Parser(new Scanner(code).Tokens).Expressions.FirstOrDefault() as
				LiteralExpression;
		Assert.That(resultExpression, Is.InstanceOf<LiteralExpression>());
		Assert.That(resultExpression?.Literal?.ToString(), Is.EqualTo(expectedValue));
		Assert.That(resultExpression?.TokenType.Type, Is.EqualTo(expectedTokenType));
	}

	[TestCase("!false", "!", "False")]
	[TestCase("!true", "!", "True")]
	[TestCase("-25", "-", "25")]
	public void ParseSingleUnaryExpressions(string code, string expectedOperator,
		string expectedSecondExpressionValue)
	{
		var resultExpression =
			GetParser(code).Expressions.FirstOrDefault() as UnaryExpression;
		Assert.That(resultExpression, Is.InstanceOf<UnaryExpression>());
		Assert.That(resultExpression?.OperatorToken.Lexeme, Is.EqualTo(expectedOperator));
		var literalExpression = resultExpression?.RightExpression as LiteralExpression;
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
		var resultExpressions = GetParser(code).Expressions;
		Assert.That(resultExpressions.FirstOrDefault(), Is.InstanceOf<BinaryExpression>());
		var binaryExpression = resultExpressions.FirstOrDefault() as BinaryExpression;
		Assert.That(binaryExpression?.LeftExpression, Is.InstanceOf<LiteralExpression>());
		var leftExpression = binaryExpression?.LeftExpression as LiteralExpression;
		Assert.That(leftExpression?.Literal?.ToString(), Is.EqualTo(expectedLeftExpression));
		Assert.That(binaryExpression?.OperatorToken.Lexeme, Is.EqualTo(expectedOperator));
		var rightExpression = binaryExpression?.RightExpression as LiteralExpression;
		Assert.That(rightExpression?.Literal?.ToString(), Is.EqualTo(expectedRightExpression));
	}

	[TestCase("25 * 2 + 30 / 2", 4)]
	[TestCase("-25 * 2 + 30 / 2", 5)]
	[TestCase("-25 * 2 + 30 / 2 > 5", 6)]
	public void ParseMultipleBinaryExpressions(string code, int expectedNumberOfExpressions)
	{
		var parser = GetParser(code);
		Assert.That(GetExpressionsCount(parser.Expressions.FirstOrDefault()!),
			Is.EqualTo(expectedNumberOfExpressions));
	}

	[Test]
	public void ParseInvalidTargetAssignmentExpression() =>
		Assert.That(() => GetParser("1 = b").Parse(),
			Throws.InstanceOf<Parser.InvalidAssignmentTarget>());

	[Test]
	public void ParseAssignmentExpression()
	{
		var parser = GetParser("a = b");
		Assert.That(parser.Expressions.FirstOrDefault(),
			Is.InstanceOf<AssignmentExpression>());
	}

	[Test]
	public void ParseGroupingExpression()
	{
		var parser = GetParser("(a + b)");
		Assert.That(parser.Expressions.FirstOrDefault(),
			Is.InstanceOf<GroupingExpression>());
	}

	[Test]
	public void ParsePrintStatement()
	{
		var statements = GetParser("print 5;").Parse();
		Assert.That(statements.Count, Is.EqualTo(1));
		Assert.That(statements[0], Is.InstanceOf<Statement.PrintStatement>());
	}

	[Test]
	public void ParseVariableDeclarationStatement()
	{
		var statements = GetParser("var a =5;").Parse();
		Assert.That(statements.Count, Is.EqualTo(1));
		Assert.That(statements[0], Is.InstanceOf<Statement.VariableStatement>());
	}

	[Test]
	public void ParseBlockStatement()
	{
		var blockStatement =
			GetParser("{ var a =5; \n print a; }").Parse()[0] as Statement.BlockStatement;
		Assert.That(blockStatement!.statements.Count, Is.EqualTo(2));
	}

	[Test]
	public void ParseIfStatement() =>
		Assert.That(GetParser("if(true) print 5;").Parse()[0],
			Is.InstanceOf<Statement.IfStatement>());

	[Test]
	public void ParseIfElseStatement() =>
		Assert.That(GetParser("if(true) print 5; else print 6;").Parse()[0],
			Is.InstanceOf<Statement.IfStatement>());

	private static Parser GetParser(string code) => new(new Scanner(code).Tokens);

	private static int GetExpressionsCount(Expression expression) =>
		expression switch
		{
			BinaryExpression binaryExpression =>
				GetExpressionsCount(binaryExpression.LeftExpression) +
				GetExpressionsCount(binaryExpression.RightExpression),
			UnaryExpression unaryExpression => 1 +
				GetExpressionsCount(unaryExpression.RightExpression),
			LiteralExpression => 1,
			_ => 0 //ncrunch: no coverage
		};
}