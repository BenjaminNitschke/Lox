namespace Lox.Exception;

public class UnterminatedString : System.Exception
{
	public UnterminatedString(int fileLineNumber, string message = "") :
		base(message + " : line " + (fileLineNumber + 1)) { }
}