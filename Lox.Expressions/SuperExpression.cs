namespace Expressions;

public sealed class SuperExpression : Expression
{
	public SuperExpression(Token keyword, Token method)
	{
		this.keyword = keyword;
		this.method = method;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitSuperExpression(this);
	public readonly Token keyword;
	public readonly Token method;
}