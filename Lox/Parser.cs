﻿namespace Lox;

public sealed class Parser
{
	public Parser(IReadOnlyList<Token> tokens)
	{
		this.tokens = tokens;
		expressions = Parse();
	}

	private readonly IReadOnlyList<Token> tokens;
	public IReadOnlyList<Expression> Expressions => expressions;
	private readonly List<Expression> expressions;
	private List<Expression> Parse() => new() { ParseExpression() };
	private bool IsAtEnd() => Peek().Type == TokenType.Eof;
	private Token Peek() => tokens.ElementAt(currentTokenCount);
	private Token Previous() => tokens.ElementAt(currentTokenCount - 1);
	private int currentTokenCount;
	private Expression ParseExpression() => ParseAssignmentExpression();

	private Expression ParseAssignmentExpression()
	{
		var expression = ParseEqualityExpression();
		if (Match(TokenType.Equal))
		{
			var equals = Previous();
			var value = ParseAssignmentExpression();
			if (expression is Expression.VariableExpression variableExpression)
			{
				var name = variableExpression.name;
				return new Expression.AssignmentExpression(name, value);
			}
			throw new InvalidAssignmentTarget(equals);
		}
		return expression;
	}

	public class InvalidAssignmentTarget : Exception
	{
		public InvalidAssignmentTarget(Token token) : base(token.Type.ToString()) { }
	}

	private Expression ParseEqualityExpression()
	{
		var expression = ParseComparisonExpressions();
		while (Match(TokenType.EqualEqual, TokenType.BangEqual))
			expression =
				new Expression.BinaryExpression(expression, Previous(), ParseComparisonExpressions());
		return expression;
	}

	private Expression ParseComparisonExpressions()
	{
		var expression = ParseArithmeticExpressions();
		while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
			expression =
				new Expression.BinaryExpression(expression, Previous(), ParseArithmeticExpressions());
		return expression;
	}

	private Expression ParseArithmeticExpressions()
	{
		var expression = ParseFactorExpressions();
		while (Match(TokenType.Plus, TokenType.Minus))
			expression = new Expression.BinaryExpression(expression,
				Previous(), ParseFactorExpressions());
		return expression;
	}

	private Expression ParseFactorExpressions()
	{
		var expression = ParseUnaryExpressions();
		while (Match(TokenType.Slash, TokenType.Star))
			expression = new Expression.BinaryExpression(expression,
				Previous(), ParseUnaryExpressions());
		return expression;
	}

	private Expression ParseUnaryExpressions() =>
		Match(TokenType.Bang, TokenType.Minus)
			? new Expression.UnaryExpression(Previous(), ParseUnaryExpressions())
			: ParsePrimaryExpressions();

	// ReSharper disable once MethodTooLong
	private Expression ParsePrimaryExpressions()
	{
		if (Match(TokenType.False))
			return new Expression.LiteralExpression(false, Previous());
		if (Match(TokenType.True))
			return new Expression.LiteralExpression(true, Previous());
		if (Match(TokenType.Nil))
			return new Expression.LiteralExpression(null, Previous());
		if (Match(TokenType.Number, TokenType.String))
			return new Expression.LiteralExpression(Previous().Literal, Previous());
		if (Match(TokenType.Identifier))
			return new Expression.VariableExpression(Previous());
		if (ParseGroupingExpression(out var groupingExpression))
			return groupingExpression;
		throw new UnknownExpression(Peek());
	}

	private bool ParseGroupingExpression(out Expression groupingExpression)
	{
		groupingExpression = null!;
		if (Match(TokenType.LeftParenthesis))
		{
			var expression = ParseExpression();
			Consume(TokenType.RightParenthesis);
			{
				groupingExpression = new Expression.GroupingExpression(expression);
				return true;
			}
		}
		return false;
	}

	private Token Consume(TokenType type)
	{
		if (Check(type))
			return Advance();
		throw type switch
		{
			TokenType.RightParenthesis => new MissingClosingParenthesis(Peek()),
			_ => new NotImplementedException() //ncrunch: no coverage
		};
	}

	public class MissingClosingParenthesis : Exception
	{
		public MissingClosingParenthesis(Token token) : base(token.Type.ToString()) { }
	}

	public class UnknownExpression : Exception
	{
		public UnknownExpression(Token token) : base(token.Type.ToString()) { }
	}

	private bool Match(params TokenType[] tokenTypes)
	{
		foreach (var type in tokenTypes)
			if (Check(type))
			{
				Advance();
				return true;
			}
		return false;
	}

	private bool Check(TokenType tokenType)
	{
		if (IsAtEnd())
			return false;
		return Peek().Type == tokenType;
	}

	private Token Advance()
	{
		if (!IsAtEnd())
			currentTokenCount++;
		return Previous();
	}
}