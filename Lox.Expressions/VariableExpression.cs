namespace Expressions;

public sealed class VariableExpression : Expression
{
	public readonly Token name;
	public VariableExpression(Token name) => this.name = name;

	public override T Accept<T>(ExpressionVisitor<T> visitor) =>
		visitor.VisitVariableExpression(this);
}