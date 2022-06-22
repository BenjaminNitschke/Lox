namespace Expressions;

public sealed class LiteralExpression : Expression
{
	public object? Literal { get; }
	public Token Token { get; }

	public LiteralExpression(object? value, Token token)
	{
		Literal = value;
		Token = token;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) =>
		visitor.VisitLiteralExpression(this);
}