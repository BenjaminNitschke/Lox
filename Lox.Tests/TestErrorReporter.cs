namespace Lox.Tests;

public class TestErrorReporter : ErrorReporter
{
	public void Report(int line, string location, string message)
	{
		Line = line;
		Location = location;
		Message = message;
	}

	public int Line { get; set; }
	public string Location { get; set; } = "";
	public string Message { get; set; } = "";
}