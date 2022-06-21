namespace Expressions;

public abstract class Expression
{
	public abstract T Accept<T>(ExpressionVisitor<T> visitor);
}