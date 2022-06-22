using System.Globalization;
using Expressions;

namespace Lox;

// ReSharper disable once ClassTooBig
public class ExpressionInterpreter : ExpressionVisitor<object>
{
	protected Environment CurrentEnvironment { get; set; } = new();
	protected object EvaluateExpression(Expression expression) => expression.Accept(this);
	public object VisitLiteralExpression(LiteralExpression literal) => literal.Literal ?? new object();

	public object VisitGroupingExpression(GroupingExpression groupingExpression) =>
		EvaluateExpression(groupingExpression.expression);

	public object VisitBinaryExpression(BinaryExpression binaryExpression)
	{
		var left = EvaluateExpression(binaryExpression.LeftExpression);
		var right = EvaluateExpression(binaryExpression.RightExpression);
		return EvaluateBinaryExpression(binaryExpression, left, right);
	}

	// ReSharper disable once CyclomaticComplexity
	private static object EvaluateBinaryExpression(BinaryExpression binaryExpression, object left,
		object right) =>
		binaryExpression.OperatorToken.Type switch
		{
			TokenType.Greater => EvaluateGreaterOperatorExpression(binaryExpression, left, right),
			TokenType.GreaterEqual => EvaluateGreaterEqualOperatorExpression(binaryExpression, left,
				right),
			TokenType.Less => EvaluateLessOperatorExpression(binaryExpression, left, right),
			TokenType.LessEqual => EvaluateLessEqualOperatorExpression(binaryExpression, left, right),
			TokenType.EqualEqual => IsEqual(left, right),
			TokenType.BangEqual => !IsEqual(left, right),
			TokenType.Minus => EvaluateMinusOperatorExpression(binaryExpression, left, right),
			TokenType.Plus => EvaluatePlusOperatorExpression(binaryExpression, left, right),
			TokenType.Slash => EvaluateSlashOperatorExpression(binaryExpression, left, right),
			TokenType.Star => EvaluateStarOperatorExpression(binaryExpression, left, right),
			_ => new object() // ncrunch: no coverage
		};

	private static object EvaluateStarOperatorExpression(BinaryExpression binaryExpression,
		object left, object right)
	{
		CheckNumberOperand(binaryExpression.OperatorToken, left, right);
		return (double)left * (double)right;
	}

	private static object EvaluateSlashOperatorExpression(BinaryExpression binaryExpression,
		object left, object right)
	{
		CheckNumberOperand(binaryExpression.OperatorToken, left, right);
		return (double)left / (double)right;
	}

	private static object EvaluateMinusOperatorExpression(BinaryExpression binaryExpression,
		object left, object right)
	{
		CheckNumberOperand(binaryExpression.OperatorToken, left, right);
		return (double)left - (double)right;
	}

	private static object EvaluateLessEqualOperatorExpression(BinaryExpression binaryExpression,
		object left, object right)
	{
		CheckNumberOperand(binaryExpression.OperatorToken, left, right);
		return (double)left <= (double)right;
	}

	private static object EvaluateLessOperatorExpression(BinaryExpression binaryExpression,
		object left, object right)
	{
		CheckNumberOperand(binaryExpression.OperatorToken, left, right);
		return (double)left < (double)right;
	}

	private static object EvaluateGreaterEqualOperatorExpression(BinaryExpression binaryExpression,
		object left, object right)
	{
		CheckNumberOperand(binaryExpression.OperatorToken, left, right);
		return (double)left >= (double)right;
	}

	private static object EvaluateGreaterOperatorExpression(BinaryExpression binaryExpression,
		object left, object right)
	{
		CheckNumberOperand(binaryExpression.OperatorToken, left, right);
		return (double)left > (double)right;
	}

	private static object EvaluatePlusOperatorExpression(BinaryExpression binaryExpression, object left,
		object right) =>
		left switch
		{
			double d when right is double rightExpressionNumber => d + rightExpressionNumber,
			string s when right is string rightExpressionNumber => s + rightExpressionNumber,
			double d when right is string s => d.ToString(CultureInfo.InvariantCulture) + s,
			string s when right is double d => s + d.ToString(CultureInfo.InvariantCulture),
			_ => throw new OperandMustBeANumberOrString(binaryExpression.OperatorToken)
		};

	private static bool IsEqual(object a, object b) => a.Equals(b);

	public sealed class OperandMustBeANumberOrString : InterpreterFailed
	{
		public OperandMustBeANumberOrString(Token expressionOperator) : base(expressionOperator) { }
	}

	private static void CheckNumberOperand(Token expressionOperator, object firstOperand,
		object secondOperand)
	{
		if (firstOperand is double && secondOperand is double)
			return;
		throw new OperandMustBeANumber(expressionOperator);
	}

	private static void CheckNumberOperand(Token expressionOperator, object operand)
	{
		if (operand is double)
			return;
		throw new OperandMustBeANumber(expressionOperator);
	}

	public sealed class OperandMustBeANumber : InterpreterFailed
	{
		public OperandMustBeANumber(Token expressionOperator) : base(expressionOperator) { }
	}

	public object VisitUnaryExpression(UnaryExpression unaryExpression)
	{
		var rightExpressionValue = EvaluateExpression(unaryExpression.RightExpression);
		return unaryExpression.OperatorToken.Type switch
		{
			TokenType.Bang => !IsTruthy(rightExpressionValue),
			TokenType.Minus => EvaluateMinusOperatorExpression(unaryExpression, rightExpressionValue),
			_ => new object() //ncrunch: no coverage
		};
	}

	private static object EvaluateMinusOperatorExpression(UnaryExpression unaryExpression,
		object rightExpressionValue)
	{
		CheckNumberOperand(unaryExpression.OperatorToken, rightExpressionValue);
		return -(double)rightExpressionValue;
	}

	protected static bool IsTruthy(object value) =>
		value switch
		{
			bool a => a,
			_ => true
		};

	public object VisitAssignmentExpression(AssignmentExpression assignmentExpression)
	{
		var value = EvaluateExpression(assignmentExpression.value);
		CurrentEnvironment.Assign(assignmentExpression.name, value);
		return value;
	}

	public object VisitVariableExpression(VariableExpression variableExpression) =>
		CurrentEnvironment.Get(variableExpression.name);

	public object VisitLogicalExpression(LogicalExpression logicalExpression)
	{
		var leftExpressionValue = EvaluateExpression(logicalExpression.left);
		if (logicalExpression.operatorToken.Type == TokenType.Or)
		{
			if (IsTruthy(leftExpressionValue))
				return leftExpressionValue;
		}
		else
		{
			if (!IsTruthy(leftExpressionValue))
				return leftExpressionValue;
		}
		return EvaluateExpression(logicalExpression.right);
	}

	public virtual object VisitCallExpression(CallExpression callExpression) => new();

	public sealed class FunctionCallIsNotSupportedHere : InterpreterFailed
	{
		public FunctionCallIsNotSupportedHere(Token callExpressionParenthesis) : base(
			callExpressionParenthesis, " Can only call functions and classes.") { }
	}

	public sealed class UnmatchedFunctionArguments : InterpreterFailed
	{
		public UnmatchedFunctionArguments(Token token, string message = "") : base(token, message) { }
	}

	public object VisitGetExpression(GetExpression getExpression)
	{
		var getExpressionValue = EvaluateExpression(getExpression.expression);
		if (getExpressionValue is Instance loxInstance)
			return loxInstance.Get(getExpression.name);
		throw new OnlyInstancesCanHaveProperty(getExpression.name);
	}

	public sealed class OnlyInstancesCanHaveProperty : InterpreterFailed
	{
		public OnlyInstancesCanHaveProperty(Token token, string message = "") : base(token, message) { }
	}

	public object VisitSetExpression(SetExpression setExpression)
	{
		var setExpressionValue = EvaluateExpression(setExpression.expression);
		if (setExpressionValue is not Instance loxInstance)
			throw new OnlyInstancesCanHaveFields(setExpression.name);
		var value = EvaluateExpression(setExpression.value);
		loxInstance.Set(setExpression.name, value);
		return value;
	}

	public sealed class OnlyInstancesCanHaveFields : InterpreterFailed
	{
		public OnlyInstancesCanHaveFields(Token token, string message = "") : base(token, message) { }
	}

	public class InterpreterFailed : OperationFailed
	{
		protected InterpreterFailed(Token token, string message = "") : base(
			message + " " + token.Lexeme, token.Line) { }
	}

	public object VisitThisExpression(ThisExpression thisExpression) => CurrentEnvironment.Get(thisExpression.keyword);

	public object VisitSuperExpression(SuperExpression superExpression)
	{
		var superClass = (Klass)CurrentEnvironment.Get(new Token(TokenType.Super, "super", "super", 0));
		var instanceObject = (Instance)CurrentEnvironment.Get(new Token(TokenType.This, "this", "this", 0));
		var method = superClass.FindMethod(superExpression.method.Lexeme);
		return method?.Bind(instanceObject) ?? new object();
	}
}