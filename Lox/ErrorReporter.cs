namespace Lox;

public interface ErrorReporter
{
	void Report(int line, string error) => Report(line, "", error);
	void Report(int line, string where, string message);
}