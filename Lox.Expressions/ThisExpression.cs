namespace Expressions;

public sealed class ThisExpression : Expression
{
	public ThisExpression(Token keyword) => this.keyword = keyword;
	public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitThisExpression(this);
	public readonly Token keyword;
}