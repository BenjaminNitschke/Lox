namespace Lox;

// ReSharper disable once IdentifierTypo
public sealed class Klass : Callable
{
	public Klass(string name, Dictionary<string, Function> methods, Klass? superClass = null)
	{
		this.name = name;
		this.methods = methods;
		this.superClass = superClass;
	}

	public readonly string name;
	private readonly Dictionary<string, Function> methods;
	private readonly Klass? superClass;

	public int Arity()
	{
		var initializer = FindMethod("init");
		return initializer?.Arity() ?? 0;
	}

	public object Call(StatementInterpreter statementInterpreter, List<object> arguments)
	{
		var instance = new Instance(this);
		var initializer = FindMethod("init");
		initializer?.Bind(instance).Call(statementInterpreter, arguments);
		return instance;
	}

	public Function? FindMethod(string methodName) =>
		methods.ContainsKey(methodName)
			? methods[methodName]
			: superClass?.FindMethod(name);

	public override string ToString() => name;
}