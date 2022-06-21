namespace Expressions;

public sealed class AssignmentExpression : Expression
{
	public readonly Token name;
	public readonly Expression value;

	public AssignmentExpression(Token name, Expression value)
	{
		this.name = name;
		this.value = value;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) =>
		visitor.VisitAssignmentExpression(this);
}