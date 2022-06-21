namespace Expressions;

public sealed class SetExpression : Expression
{
	public SetExpression(Expression expression, Token name, Expression value)
	{
		this.expression = expression;
		this.name = name;
		this.value = value;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitSetExpression(this);
	public readonly Expression expression;
	public readonly Token name;
	public readonly Expression value;
}