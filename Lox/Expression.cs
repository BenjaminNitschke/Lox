namespace Lox;

public abstract class Expression
{
	public class LiteralExpression : Expression
	{
		public object? Literal { get; }
		public Token TokenType { get; }

		public LiteralExpression(object? value, Token tokenType)
		{
			Literal = value;
			TokenType = tokenType;
		}
	}

	public class UnaryExpression : Expression
	{
		public Token OperatorToken { get; }
		public Expression RightExpression { get; }

		public UnaryExpression(Token operatorToken, Expression rightExpression)
		{
			OperatorToken = operatorToken;
			RightExpression = rightExpression;
		}
	}

	public class BinaryExpression : Expression
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
	}

	public class VariableExpression : Expression
	{
		public readonly Token name;
		public VariableExpression(Token name) => this.name = name;
	}

	public class AssignmentExpression : Expression
	{
		public readonly Token name;
		public readonly Expression value;

		public AssignmentExpression(Token name, Expression value)
		{
			this.name = name;
			this.value = value;
		}
	}

	public class GroupingExpression : Expression
	{
		public readonly Expression expression;
		public GroupingExpression(Expression expression) => this.expression = expression;
	}
}