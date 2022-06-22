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
		CurrentEnvironment.Define(variableStatement.name.Lexeme, value);
		return new object();
	}

	public object VisitBlockStatement(BlockStatement blockStatement)
	{
		ExecuteBlock(blockStatement.statements, new Environment(CurrentEnvironment));
		return new object();
	}

	public void ExecuteBlock(List<Statement> statements, Environment innerEnvironment)
	{
		var previous = CurrentEnvironment;
		try
		{
			CurrentEnvironment = innerEnvironment;
			foreach (var statement in statements)
				Execute(statement);
		}
		finally
		{
			CurrentEnvironment = previous;
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
		var loxFunction = new Function(functionStatement, CurrentEnvironment, false);
		CurrentEnvironment.Define(functionStatement.name.Lexeme, loxFunction);
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
			CurrentEnvironment = new Environment(CurrentEnvironment);
			CurrentEnvironment.Define("super", superClass);
		}
		CurrentEnvironment.Define(classStatement.name.Lexeme, new object());
		var methods = classStatement.methods.ToDictionary(method => method.name.Lexeme,
			method => new Function(method, CurrentEnvironment, false));
		var loxClass = new Klass(classStatement.name.Lexeme, methods);
		if (superClass != null)
			loxClass = new Klass(classStatement.name.Lexeme, methods, loxClass);
		CurrentEnvironment.Assign(classStatement.name, loxClass);
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

	public override object VisitCallExpression(CallExpression callExpression)
	{
		var callee = EvaluateExpression(callExpression.callee);
		var arguments = callExpression.arguments.Select(EvaluateExpression).ToList();
		if (callee is not Callable callableFunction)
			throw new FunctionCallIsNotSupportedHere(new Token(TokenType.Call, "Function Call", null,
				callExpression.parenthesis.Line));
		if (arguments.Count != callableFunction.Arity())
			throw new UnmatchedFunctionArguments(
				new Token(TokenType.Call, "Function Call", null, callExpression.parenthesis.Line),
				"Expected " + callableFunction.Arity() + " arguments but got " + arguments.Count + ".");
		return callableFunction.Call(this, arguments);
	}
}