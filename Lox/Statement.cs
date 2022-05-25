namespace Lox;

public abstract class Statement
{
	public abstract T Accept<T>(StatementVisitor<T> statementVisitor);

	public class ExpressionStatement : Statement
	{
		public readonly Expression expression;
		public ExpressionStatement(Expression expression) => this.expression = expression;
		public override T Accept<T>(StatementVisitor<T> statementVisitor) => statementVisitor.VisitExpressionStatement(this);
	}

	public class PrintStatement : Statement
	{
		public readonly Expression expression;
		public PrintStatement(Expression expression) => this.expression = expression;
		public override T Accept<T>(StatementVisitor<T> statementVisitor) => statementVisitor.VisitPrintStatement(this);
	}

	public class VariableStatement : Statement
	{
		public readonly Token name;
		public readonly Expression? initializer;

		public VariableStatement(Token name, Expression? initializer)
		{
			this.name = name;
			if (initializer != null)
				this.initializer = initializer;
		}

		public override T Accept<T>(StatementVisitor<T> statementVisitor) => statementVisitor.VisitVariableStatement(this);
	}

	public class BlockStatement : Statement
	{
		public readonly List<Statement> statements;
		public BlockStatement(List<Statement> statements) => this.statements = statements;
		public override T Accept<T>(StatementVisitor<T> statementVisitor) => statementVisitor.VisitBlockStatement(this);
	}
}

public interface StatementVisitor<out T>
{
	T VisitPrintStatement(Statement.PrintStatement printStatement);
	T VisitExpressionStatement(Statement.ExpressionStatement expressionStatement);
	T VisitVariableStatement(Statement.VariableStatement variableStatement);
	T VisitBlockStatement(Statement.BlockStatement blockStatement);
}