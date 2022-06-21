namespace Expressions;

public sealed class LogicalExpression : Expression
{
	public LogicalExpression(Expression left, Expression right, Token operatorToken)
	{
		this.left = left;
		this.right = right;
		this.operatorToken = operatorToken;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) =>
		visitor.VisitLogicalExpression(this);

	public readonly Expression left;
	public readonly Token operatorToken;
	public readonly Expression right;
}