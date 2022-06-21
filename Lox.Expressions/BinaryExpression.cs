namespace Expressions;

public sealed class BinaryExpression : Expression
{
	public Expression LeftExpression { get; }
	public Token OperatorToken { get; }
	public Expression RightExpression { get; }

	public BinaryExpression(Expression leftExpression, Token operatorToken,
		Expression rightExpression)
	{
		OperatorToken = operatorToken;
		RightExpression = rightExpression;
		LeftExpression = leftExpression;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) =>
		visitor.VisitBinaryExpression(this);
}