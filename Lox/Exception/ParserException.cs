namespace Lox.Exception;

public class ParserException : System.Exception
{
	protected ParserException(Token token, string message = "") : base(message + " Token Type " +
		token.Type) { }
}