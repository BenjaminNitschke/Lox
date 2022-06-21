using Expressions;

namespace Lox;

public sealed class StatementInterpreter : ExpressionInterpreter, StatementVisitor<object>
{
	public void Interpret(List<Statement> statements)
	{
		foreach (var statement in statements)
			Execute(statement);
	}

	private void Execute(Statement statement) => statement.Accept(this);

	public object VisitPrintStatement(PrintStatement printStatement)
	{
		var value = EvaluateExpression(printStatement.expression);
		Console.Out.WriteLine(Stringify(value));
		return value;
	}

	private static string Stringify(object resultObject)
	{
		switch (resultObject)
		{
		case double:
		{
			var text = resultObject.ToString()!;
			return text;
		}
		default:
			return resultObject.ToString()!;
		}
	}

	public object VisitExpressionStatement(ExpressionStatement expressionStatement) =>
		EvaluateExpression(expressionStatement.expression);

	public object VisitVariableStatement(VariableStatement variableStatement)
	{
		var value = new object();
		if (variableStatement.initializer != null)
			value = EvaluateExpression(variableStatement.initializer);
		environment.Define(variableStatement.name.Lexeme, value);
		return new object();
	}

	public object VisitBlockStatement(BlockStatement blockStatement)
	{
		ExecuteBlock(blockStatement.statements, new Environment(environment));
		return new object();
	}

	public void ExecuteBlock(List<Statement> statements, Environment innerEnvironment)
	{
		var previous = environment;
		try
		{
			environment = innerEnvironment;
			foreach (var statement in statements)
				Execute(statement);
		}
		finally
		{
			environment = previous;
		}
	}

	public object VisitIfStatement(IfStatement ifStatement)
	{
		if (IsTruthy(EvaluateExpression(ifStatement.condition)))
			Execute(ifStatement.thenStatement);
		else if (ifStatement.elseStatement != null)
			Execute(ifStatement.elseStatement);
		return new object();
	}

	public object VisitWhileStatement(WhileStatement whileStatement)
	{
		while (IsTruthy(EvaluateExpression(whileStatement.condition)))
			Execute(whileStatement.bodyStatement);
		return new object();
	}

	public object VisitFunctionStatement(FunctionStatement functionStatement)
	{
		var loxFunction = new Function(functionStatement, environment, false);
		environment.Define(functionStatement.name.Lexeme, loxFunction);
		return new object();
	}

	public object VisitReturnStatement(ReturnStatement returnStatement)
	{
		object? value = null;
		if (returnStatement.value != null)
			value = EvaluateExpression(returnStatement.value);
		throw new Return(value);
	}

	public sealed class Return : Exception
	{
		public readonly object? value;
		public Return(object? value) => this.value = value;
	}

	public object VisitClassStatement(ClassStatement classStatement)
	{
		var superClass = CheckSuperClassAndEvaluate(classStatement);
		if (classStatement.superClass != null && superClass != null)
		{
			environment = new Environment(environment);
			environment.Define("super", superClass);
		}
		environment.Define(classStatement.name.Lexeme, new object());
		var methods = classStatement.methods.ToDictionary(method => method.name.Lexeme,
			method => new Function(method, environment, false));
		var loxClass = new Klass(classStatement.name.Lexeme, methods);
		if (superClass != null)
			loxClass = new Klass(classStatement.name.Lexeme, methods, loxClass);
		environment.Assign(classStatement.name, loxClass);
		return new object();
	}

	private object? CheckSuperClassAndEvaluate(ClassStatement classStatement)
	{
		object? superClass = null;
		if (classStatement.superClass != null)
		{
			superClass = EvaluateExpression(classStatement.superClass);
			if (superClass is not Klass)
				throw new SuperClassMustBeAClass(classStatement.superClass.name);
		}
		return superClass;
	}

	public sealed class SuperClassMustBeAClass : OperationFailed
	{
		public SuperClassMustBeAClass(Token token) : base(token.Lexeme, token.Line) { }
	}
}