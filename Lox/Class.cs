namespace Lox;

public class Class : Callable
{
	public Class(string name, Dictionary<string, Function> methods)
	{
		this.name = name;
		this.methods = methods;
	}

	public readonly string name;
	private readonly Dictionary<string, Function> methods;

	public int Arity()
	{
		var initializer = FindMethod("init");
		return initializer?.Arity() ?? 0;
	}

	public object Call(Interpreter interpreter, List<object> arguments)
	{
		var instance = new Instance(this);
		var initializer = FindMethod("init");
		initializer?.Bind(instance).Call(interpreter, arguments);
		return instance;
	}

	public Function? FindMethod(string methodName) =>
		methods.ContainsKey(methodName)
			? methods[methodName]
			: null;

	public override string ToString() => name;
}