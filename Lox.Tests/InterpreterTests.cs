using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

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

	private static List<Statement> GetStatements(string code) => new Parser(new Scanner(code).Tokens).Parse();
	private static IReadOnlyList<Expression> GetParsedExpressions(string code) => new Parser(new Scanner(code).Tokens).Expressions;
}