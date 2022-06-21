namespace Expressions;

public sealed class GetExpression : Expression
{
	public GetExpression(Expression expression, Token name)
	{
		this.expression = expression;
		this.name = name;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitGetExpression(this);
	public readonly Expression expression;
	public readonly Token name;
}