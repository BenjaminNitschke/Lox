namespace Expressions;

public sealed class LiteralExpression : Expression
{
	public object? Literal { get; }
	public Token TokenType { get; }

	public LiteralExpression(object? value, Token tokenType)
	{
		Literal = value;
		TokenType = tokenType;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) =>
		visitor.VisitLiteralExpression(this);
}