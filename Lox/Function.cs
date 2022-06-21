using Expressions;

namespace Lox;

public sealed class Function : Callable
{
	public Function(Statement.FunctionStatement declaration, Environment closure, bool isInitializer)
	{
		this.declaration = declaration;
		this.closure = closure;
		this.isInitializer = isInitializer;
	}

	private readonly Statement.FunctionStatement declaration;
	private readonly Environment closure;
	private readonly bool isInitializer;

	public Function Bind(Instance instance)
	{
		var environment = new Environment(closure);
		environment.Define("this", instance);
		return new Function(declaration, environment, isInitializer);
	}

	public int Arity() => declaration.functionParams.Count;

	public object Call(Interpreter interpreter, List<object> arguments)
	{
		var environment = new Environment(closure);
		for (var i = 0; i < declaration.functionParams.Count; i++)
			environment.Define(declaration.functionParams[i].Lexeme, arguments[i]);
		try
		{
			interpreter.ExecuteBlock(declaration.body, environment);
		}
		catch (Interpreter.Return returnValue)
		{
			return isInitializer
				? closure.Get(new Token(TokenType.This, "this", null, 0))
				: returnValue.value ?? new object();
		}
		return isInitializer
			? closure.Get(new Token(TokenType.This, "this", null, 0))
			: new object();
	}

	public override string ToString() => "<fn " + declaration.name.Lexeme + ">";
}