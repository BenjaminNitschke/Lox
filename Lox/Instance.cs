using Lox.Expressions;

namespace Lox;

public sealed class Instance
{
	public Instance(Klass klass) => this.klass = klass;
	private readonly Klass klass;
	private readonly Dictionary<string, object> fields = new();

	public object Get(Token name)
	{
		if (fields.ContainsKey(name.Lexeme))
			return fields[name.Lexeme];
		var method = klass.FindMethod(name.Lexeme);
		if (method != null)
			return method.Bind(this);
		throw new UndefinedProperty(name.Lexeme);
	}

	public sealed class UndefinedProperty : Exception
	{
		public UndefinedProperty(string lexeme) : base(lexeme) { }
	}

	public override string ToString() => klass.name + " instance";
	public void Set(Token name, object value) => fields.Add(name.Lexeme, value);
}