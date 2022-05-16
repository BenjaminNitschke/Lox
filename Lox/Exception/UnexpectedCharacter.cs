namespace Lox.Exception;

public class UnexpectedCharacter : System.Exception
{
	public UnexpectedCharacter(int fileLineNumber, string message = "") :
		base(message + " : line " + (fileLineNumber + 1)) { }
}