namespace Lox.Exception;

public class UnknownExpression : ParserException
{
	public UnknownExpression(Token token, string message = "") : base(token, message) { }
}