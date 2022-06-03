namespace Lox;

public sealed class Environment
{
	public readonly Environment? enclosing;
	private readonly Dictionary<string, object> values = new();
	private string globalName = "";
	private object globalValue = null!;
	public Environment() => enclosing = null;
	public Environment(Environment enclosing) => this.enclosing = enclosing;

	public object Get(Token name)
	{
		if (values.ContainsKey(name.Lexeme))
			return values[name.Lexeme];
		if (enclosing != null)
			return enclosing.Get(name);
		if (name.Lexeme == globalName)
			return globalValue;
		throw new UndefinedVariable(name);
	}

	public sealed class UndefinedVariable : OperationFailed
	{
		public UndefinedVariable(Token name) : base(name.Lexeme, name.Line) { }
	}

	public void Assign(Token name, object value)
	{
		if (values.ContainsKey(name.Lexeme))
		{
			values[name.Lexeme] = value;
			return;
		}
		if (enclosing == null)
		{
			globalName = name.Lexeme;
			globalValue = value;
		} //throw new UndefinedVariable(name);
		else
			enclosing.Assign(name, value);
	}

	public void Define(string name, object value)
	{
		if (values.ContainsKey(name))
			throw new DuplicateVariableName(name);
		values.Add(name, value);
	}

	public sealed class DuplicateVariableName : Exception
	{
		public DuplicateVariableName(string name) : base(name) { }
	}
}