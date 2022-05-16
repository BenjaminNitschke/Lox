namespace Lox;

public abstract class Expression
{
	public class Literal : Expression
	{
		public readonly object? literal;
		public readonly Token tokenType;

		public Literal(object? value, Token tokenType)
		{
			literal = value;
			this.tokenType = tokenType;
		}
	}
}