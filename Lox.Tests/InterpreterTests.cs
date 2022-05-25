using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Lox.Tests;

public sealed class InterpreterTests
{
	[TestCase("5 + a")]
	public void EvaluatePlusExpressionWithInvalidOperand(string code) =>
		Assert.That(
			() => new Interpreter().VisitBinaryExpression(
				(Expression.BinaryExpression)GetParsedExpressions(code).FirstOrDefault()!),
			Throws.InstanceOf<Interpreter.OperandMustBeANumberOrString>());

	[TestCase("5 > a")]
	[TestCase("5 * a")]
	public void EvaluateBinaryNumberExpressionWithInvalidOperand(string code) =>
		Assert.That(
			() => new Interpreter().VisitBinaryExpression(
				(Expression.BinaryExpression)GetParsedExpressions(code).FirstOrDefault()!),
			Throws.InstanceOf<Interpreter.OperandMustBeANumber>());

	[TestCase("-m")]
	public void EvaluateUnaryNumberExpressionWithInvalidOperand(string code) =>
		Assert.That(
			() => new Interpreter().VisitUnaryExpression(
				(Expression.UnaryExpression)GetParsedExpressions(code).FirstOrDefault()!),
			Throws.InstanceOf<Interpreter.OperandMustBeANumber>());

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

	[TestCase("a = 5", 5)]
	public void EvaluateAssignmentExpression(string code, object expectedValue)
	{
		var result = new Interpreter().VisitAssignmentExpression((Expression.AssignmentExpression)GetParsedExpressions(code).FirstOrDefault()!);
		Assert.That(result, Is.EqualTo(expectedValue));
	}

	private static IReadOnlyList<Expression> GetParsedExpressions(string code) => new Parser(new Scanner(code).Tokens).Expressions;
}