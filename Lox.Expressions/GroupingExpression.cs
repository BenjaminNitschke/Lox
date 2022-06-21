namespace Expressions;

public sealed class GroupingExpression : Expression
{
	public readonly Expression expression;
	public GroupingExpression(Expression expression) => this.expression = expression;

	public override T Accept<T>(ExpressionVisitor<T> visitor) =>
		visitor.VisitGroupingExpression(this);
}