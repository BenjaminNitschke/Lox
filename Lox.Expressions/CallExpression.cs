namespace Expressions;

public sealed class CallExpression : Expression
{
	public CallExpression(Expression callee, Token parenthesis, List<Expression> arguments)
	{
		this.callee = callee;
		this.parenthesis = parenthesis;
		this.arguments = arguments;
	}

	public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitCallExpression(this);
	public readonly Expression callee;
	public readonly Token parenthesis;
	public readonly List<Expression> arguments;
}