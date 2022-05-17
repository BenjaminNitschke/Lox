using Lox.Exception;

namespace Lox;

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
	private List<Expression> Parse() => new() { ParseFactorBinaryExpressions() };
	private bool IsAtEnd() => Peek().Type == TokenType.Eof;
	private Token Peek() => tokens.ElementAt(currentTokenCount);
	private Token Previous() => tokens.ElementAt(currentTokenCount - 1);
	private int currentTokenCount;

	private Expression ParseFactorBinaryExpressions()
	{
		var expression = ParseUnaryExpressions();
		if (Match(TokenType.Slash, TokenType.Star))
			expression = new Expression.BinaryExpression(leftExpression: expression,
				operatorToken: Previous(), rightExpression: ParseUnaryExpressions());
		return expression;
	}

	private Expression ParseUnaryExpressions() =>
		Match(TokenType.Bang, TokenType.Minus)
			? new Expression.UnaryExpression(operatorToken: Previous(), rightExpression: ParseUnaryExpressions())
			: ParsePrimaryExpressions();

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
		throw new UnknownExpression(Peek());
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

	private void Advance()
	{
		if (!IsAtEnd())
			currentTokenCount++;
		Previous();
	}
}