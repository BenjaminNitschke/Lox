namespace Lox;

public class ConsoleErrorReporter : ErrorReporter
{
	public void Report(int line, string where, string message)
	{
		Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
		HadError = true;
	}

	public bool HadError { get; private set; }
	public void Reset() => HadError = false;
}