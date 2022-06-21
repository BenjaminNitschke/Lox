namespace Lox.Expressions;

public abstract class Expression
{
	public abstract T Accept<T>(ExpressionVisitor<T> visitor);

	public sealed class LiteralExpression : Expression
	{
		public object? Literal { get; }
		public Token TokenType { get; }

		public LiteralExpression(object? value, Token tokenType)
		{
			Literal = value;
			TokenType = tokenType;
		}

		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitLiteralExpression(this);
	}

	public sealed class UnaryExpression : Expression
	{
		public Token OperatorToken { get; }
		public Expression RightExpression { get; }

		public UnaryExpression(Token operatorToken, Expression rightExpression)
		{
			OperatorToken = operatorToken;
			RightExpression = rightExpression;
		}

		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitUnaryExpression(this);
	}

	public sealed class BinaryExpression : Expression
	{
		public Expression LeftExpression { get; }
		public Token OperatorToken { get; }
		public Expression RightExpression { get; }

		public BinaryExpression(Expression leftExpression, Token operatorToken, Expression rightExpression)
		{
			OperatorToken = operatorToken;
			RightExpression = rightExpression;
			LeftExpression = leftExpression;
		}

		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitBinaryExpression(this);
	}

	public sealed class VariableExpression : Expression
	{
		public readonly Token name;
		public VariableExpression(Token name) => this.name = name;
		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitVariableExpression(this);
	}

	public sealed class AssignmentExpression : Expression
	{
		public readonly Token name;
		public readonly Expression value;

		public AssignmentExpression(Token name, Expression value)
		{
			this.name = name;
			this.value = value;
		}

		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitAssignmentExpression(this);
	}

	public sealed class GroupingExpression : Expression
	{
		public readonly Expression expression;
		public GroupingExpression(Expression expression) => this.expression = expression;
		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitGroupingExpression(this);
	}

	public sealed class LogicalExpression : Expression
	{
		public LogicalExpression(Expression left, Expression right, Token operatorToken)
		{
			this.left = left;
			this.right = right;
			this.operatorToken = operatorToken;
		}

		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitLogicalExpression(this);
		public readonly Expression left;
		public readonly Token operatorToken;
		public readonly Expression right;
	}

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

	public sealed class ThisExpression : Expression
	{
		public ThisExpression(Token keyword) => this.keyword = keyword;
		public override T Accept<T>(ExpressionVisitor<T> visitor) => visitor.VisitThisExpression(this);
		public readonly Token keyword;
	}

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
}