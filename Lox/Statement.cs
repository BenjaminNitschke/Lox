using Expressions;

namespace Lox;

public abstract class Statement
{
	public abstract T Accept<T>(StatementVisitor<T> statementVisitor);
}

public sealed class ExpressionStatement : Statement
{
	public readonly Expression expression;
	public ExpressionStatement(Expression expression) => this.expression = expression;

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitExpressionStatement(this);
}

public sealed class PrintStatement : Statement
{
	public readonly Expression expression;
	public PrintStatement(Expression expression) => this.expression = expression;

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitPrintStatement(this);
}

public sealed class VariableStatement : Statement
{
	public readonly Token name;
	public readonly Expression? initializer;

	public VariableStatement(Token name, Expression? initializer)
	{
		this.name = name;
		if (initializer != null)
			this.initializer = initializer;
	}

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitVariableStatement(this);
}

public sealed class BlockStatement : Statement
{
	public readonly List<Statement> statements;
	public BlockStatement(List<Statement> statements) => this.statements = statements;

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitBlockStatement(this);
}

public sealed class IfStatement : Statement
{
	public IfStatement(Expression condition, Statement thenStatement, Statement? elseStatement)
	{
		this.condition = condition;
		this.thenStatement = thenStatement;
		this.elseStatement = elseStatement;
	}

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitIfStatement(this);

	public readonly Expression condition;
	public readonly Statement thenStatement;
	public readonly Statement? elseStatement;
}

public sealed class WhileStatement : Statement
{
	public WhileStatement(Expression condition, Statement bodyStatement)
	{
		this.condition = condition;
		this.bodyStatement = bodyStatement;
	}

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitWhileStatement(this);

	public readonly Expression condition;
	public readonly Statement bodyStatement;
}

public sealed class FunctionStatement : Statement
{
	public FunctionStatement(Token name, List<Token> functionParams, List<Statement> body)
	{
		this.name = name;
		this.functionParams = functionParams;
		this.body = body;
	}

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitFunctionStatement(this);

	public readonly Token name;
	public readonly List<Token> functionParams;
	public readonly List<Statement> body;
}

public sealed class ReturnStatement : Statement
{
	public ReturnStatement(Expression? value) => this.value = value;

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitReturnStatement(this);

	public readonly Expression? value;
}

public sealed class ClassStatement : Statement
{
	public ClassStatement(Token name, List<FunctionStatement> methods,
		VariableExpression? superClass)
	{
		this.name = name;
		this.methods = methods;
		this.superClass = superClass;
	}

	public override T Accept<T>(StatementVisitor<T> statementVisitor) =>
		statementVisitor.VisitClassStatement(this);

	public readonly Token name;
	public readonly List<FunctionStatement> methods;
	public readonly VariableExpression? superClass;
}