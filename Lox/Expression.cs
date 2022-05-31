namespace Lox;

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
}

public interface ExpressionVisitor<out T>
{
	T VisitLiteralExpression(Expression.LiteralExpression literal);
	T VisitGroupingExpression(Expression.GroupingExpression groupingExpression);
	T VisitBinaryExpression(Expression.BinaryExpression binaryExpression);
	T VisitUnaryExpression(Expression.UnaryExpression unaryExpression);
	T VisitVariableExpression(Expression.VariableExpression variableExpression);
	T VisitAssignmentExpression(Expression.AssignmentExpression assignmentExpression);
	T VisitLogicalExpression(Expression.LogicalExpression logicalExpression);
}