namespace Expressions;

public sealed class UnaryExpression : Expression
{
	public Token OperatorToken { get; }
	public Expression RightExpression { get; }

	public UnaryExpression(Token operatorToken, Expression rightExpression)
	{
		OperatorToken = operatorToken;
		RightExpression = rightExpression;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitUnaryExpression(this);
}