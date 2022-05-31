namespace Lox
{
	public class Function : Callable
	{
		private readonly Statement.FunctionStatement declaration;
		public Function(Statement.FunctionStatement declaration) => this.declaration = declaration;
		public int Arity() => declaration.functionParams.Count;

		public object Call(Interpreter interpreter, List<object> arguments)
		{
			var environment = new Environment(interpreter.environment);
			for (var i = 0; i < declaration.functionParams.Count; i++)
				environment.Define(declaration.functionParams[i].Lexeme, arguments[i]);
			try
			{
				interpreter.ExecuteBlock(declaration.body, environment);
			}
			catch (Interpreter.Return returnValue)
			{
				return returnValue.value ?? new object();
			}
			return new object();
		}

		public override string ToString() => "<fn " + declaration.name.Lexeme + ">";
	}
}
