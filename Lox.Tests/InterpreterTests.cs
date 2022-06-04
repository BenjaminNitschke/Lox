namespace Lox.Tests;

public sealed class InterpreterTests
{
	[TestCase("5 + true")]
	public void EvaluatePlusExpressionWithInvalidOperand(string code) =>
		Assert.That(
			() => new Interpreter().VisitBinaryExpression(
				(Expression.BinaryExpression)GetParsedExpressions(code).FirstOrDefault()!),
			Throws.InstanceOf<Interpreter.OperandMustBeANumberOrString>());

	[TestCase("5 > true")]
	[TestCase("5 * true")]
	public void EvaluateBinaryNumberExpressionWithInvalidOperand(string code) =>
		Assert.That(
			() => new Interpreter().VisitBinaryExpression(
				(Expression.BinaryExpression)GetParsedExpressions(code).FirstOrDefault()!),
			Throws.InstanceOf<Interpreter.OperandMustBeANumber>());

	[TestCase("-\"m\"")]
	public void EvaluateUnaryNumberExpressionWithInvalidOperand(string code) =>
		Assert.That(() => new Interpreter().VisitUnaryExpression((Expression.UnaryExpression)GetParsedExpressions(code).FirstOrDefault()!),
			Throws.InstanceOf<Interpreter.OperandMustBeANumber>());

	[TestCase("{var a =5; var a = 6;}")]
	public void DefineDuplicateVariableName(string code) =>
		Assert.That(() => new Interpreter().Interpret(GetStatements(code)),
			Throws.InstanceOf<Environment.DuplicateVariableName>());

	[TestCase("a =5;")]
	[TestCase("print a;")]
	public void AccessUndefinedVariable(string code) =>
		Assert.That(() => new Interpreter().Interpret(GetStatements(code)),
			Throws.InstanceOf<Environment.UndefinedVariable>()!);

	[TestCase("while")]
	public void WhileWithoutOpeningBracket(string code) =>
		Assert.That(() => new Interpreter().Interpret(GetStatements(code)),
			Throws.InstanceOf<Parser.MissingLeftParenthesis>()!);

	[Test]
	public void FunctionWithUnMatchingArguments() =>
		Assert.That(
			() => new Interpreter().Interpret(GetStatements(
				"fun sayHi(first, last) { print \"Hi, \" + first + \" \" + last + \"!\";}" +
				"sayHi(\"Dear\");")), Throws.InstanceOf<Interpreter.UnmatchedFunctionArguments>());

	[Test]
	public void FunctionCallNotSupportedException() =>
		Assert.That(
			() => new Interpreter().Interpret(GetStatements(
				"fun sayHi(first, last) { print \"Hi, \" + first + \" \" + last + \"!\";} sayHi = 10;" +
				"sayHi(\"Dear\");")), Throws.InstanceOf<Interpreter.FunctionCallIsNotSupportedHere>()!);

	[Test]
	public void OnlyInstancesCanHaveProperty() =>
		Assert.That(
			() => new Interpreter().Interpret(GetStatements(
				"fun sayHi(first, last) { print \"Hi, \" + first + \" \" + last + \"!\";} sayHi." +
				"sayHi(\"Dear\");")), Throws.InstanceOf<Interpreter.OnlyInstancesCanHaveProperty>()!);

	[Test]
	public void OnlyInstancesCanHaveFields() =>
		Assert.That(
			() => new Interpreter().Interpret(GetStatements(
				"class Cake { taste() { var adjective = \"delicious\"; print \"The \" + this.flavor + \" cake is \" + adjective + \"!\"; } } var cake = Cake(); cake = 10; cake.flavor = \"German chocolate\";")),
			Throws.InstanceOf<Interpreter.OnlyInstancesCanHaveFields>()!);

	[Test]
	public void AccessUndefinedProperty() =>
		Assert.That(
			() => new Interpreter().Interpret(GetStatements(
				"class Cake { taste() { var adjective = \"delicious\"; print \"The \" + this.flavor + \" cake is \" + adjective + \"!\"; } } var cake = Cake(); var test = cake.random;")),
			Throws.InstanceOf<Instance.UndefinedProperty>()!);

	[Test]
	public void AccessSuperClassUndefinedProperty() =>
		Assert.That(
			() => new Interpreter().Interpret(GetStatements(
				"class Cake { } class SuperClass < Cake { bake(){ }} SuperClass().random();")),
			Throws.InstanceOf<Instance.UndefinedProperty>()!);

	[Test]
	public void EvaluateLiteralExpression()
	{
		var result = new Interpreter().VisitLiteralExpression((Expression.LiteralExpression)GetParsedExpressions("1").FirstOrDefault()!);
		Assert.That(result, Is.EqualTo(1));
	}

	[TestCase("8 * ( 5 + 2)", 56)]
	public void EvaluateGroupingExpression(string code, object expectedValue)
	{
		var result = new Interpreter().VisitBinaryExpression((Expression.BinaryExpression)GetParsedExpressions(code).FirstOrDefault()!);
		Assert.That(result, Is.EqualTo(56));
	}

	[TestCase("1 + 1", 2)]
	[TestCase("1 > 2", false)]
	[TestCase("1 >= 1", true)]
	[TestCase("1 < 2", true)]
	[TestCase("5 <= 4", false)]
	[TestCase("5 == 5", true)]
	[TestCase("5 != 4", true)]
	[TestCase("5 - 4", 1)]
	[TestCase("\"a\" + \"b\"", "ab")]
	[TestCase("\"a\" + 5", "a5")]
	[TestCase("5 + \"b\"", "5b")]
	[TestCase("8 / 4", 2)]
	[TestCase("8 * 4", 32)]
	public void EvaluateBinaryExpression(string code, object expectedValue)
	{
		var result = new Interpreter().VisitBinaryExpression((Expression.BinaryExpression)GetParsedExpressions(code).FirstOrDefault()!);
		Assert.That(result, Is.EqualTo(expectedValue));
	}

	[TestCase("!true", false)]
	[TestCase("-5", -5)]
	[TestCase("!5", false)]
	public void EvaluateUnaryExpression(string code, object expectedValue)
	{
		var result = new Interpreter().VisitUnaryExpression((Expression.UnaryExpression)GetParsedExpressions(code).FirstOrDefault()!);
		Assert.That(result, Is.EqualTo(expectedValue));
	}

	[TestCase("{var a = \"expectedResult\"; print a;}", "expectedResult\r\n")]
	[TestCase("{var a = 6.0; print a;}", "6\r\n")]
	[TestCase("{var a=5; a = 6; print a;}", "6\r\n")]
	[TestCase("{var a=5; a = 6.052; print a;}", "6.052\r\n")]
	[TestCase("{var a=5; var b = 5; print a + b;}", "10\r\n")]
	[TestCase("{var a= true; a = !a; print a;}", "False\r\n")]
	[TestCase("var b = true; { var b = false; b = true; print b;}", "True\r\n")]
	public void EvaluateStatements(string code, string expectedValue)
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		new Interpreter().Interpret(GetStatements(code));
		Assert.That(stringWriter.ToString(), Is.EqualTo(expectedValue));
	}

	[TestCase("if(5 > 4) print 5;", "5\r\n")]
	[TestCase("if(5 > 6) print 5; else print 6;", "6\r\n")]
	public void EvaluateIfElseStatements(string code, string expectedValue)
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		new Interpreter().Interpret(GetStatements(code));
		Assert.That(stringWriter.ToString(), Is.EqualTo(expectedValue));
	}

	[TestCase("if(5 < 4 or 4 == 4) print 5;", "5\r\n")]
	[TestCase("if(true and false) print 5; else print 6;", "6\r\n")]
	[TestCase("if(true or false) print 5; else print 6;", "5\r\n")]
	[TestCase("if(false and false) print 5; else print 6;", "6\r\n")]
	public void EvaluateLogicalExpressionStatements(string code, string expectedValue)
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		new Interpreter().Interpret(GetStatements(code));
		Assert.That(stringWriter.ToString(), Is.EqualTo(expectedValue));
	}

	[TestCase("var i = 0; while(i < 5) { print i; i = 5;} ", "0\r\n")]
	[TestCase("if(5 > 6) print 5; else print 6;", "6\r\n")]
	public void EvaluateWhileStatements(string code, string expectedValue)
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		new Interpreter().Interpret(GetStatements(code));
		Assert.That(stringWriter.ToString(), Is.EqualTo(expectedValue));
	}

	[TestCase("var i = 0; for(; i < 1; i = i + 1) { print 0; } ", "0\r\n")]
	[TestCase("var i = 0; var j = 1; for(j = i; i < 1; i = i + 1) { print 0; } ", "0\r\n")]
	public void EvaluateForLoop(string code, string expectedValue)
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		new Interpreter().Interpret(GetStatements(code));
		Assert.That(stringWriter.ToString(), Is.EqualTo(expectedValue));
	}

	[Test]
	public void EvaluateFunction()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		new Interpreter().Interpret(GetStatements("fun sayHi(first, last) { print \"Hi, \" + first + \" \" + last + \"!\";}" +
			"sayHi(\"Dear\", \"Reader\");"));
		Assert.That(stringWriter.ToString(), Is.EqualTo("Hi, Dear Reader!\r\n"));
	}

	[Test]
	public void PrintFunctionName()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		new Interpreter().Interpret(GetStatements("fun sayHi(first, last) { print \"Hi, \" + first + \" \" + last + \"!\";}" +
			"print sayHi;"));
		Assert.That(stringWriter.ToString(), Is.EqualTo("<fn sayHi>\r\n"));
	}

	private static List<Statement> GetStatements(string code) => new Parser(new Scanner(code).Tokens).Parse();
	private static IReadOnlyList<Expression> GetParsedExpressions(string code) => new Parser(new Scanner(code).Tokens).Expressions;
}