namespace Lox;

public sealed class Class : Callable
{
	public Class(string name, Dictionary<string, Function> methods, Class? superClass = null)
	{
		this.name = name;
		this.methods = methods;
		this.superClass = superClass;
	}

	public readonly string name;
	private readonly Dictionary<string, Function> methods;
	private readonly Class? superClass;

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
			: superClass?.FindMethod(name);

	public override string ToString() => name;
}