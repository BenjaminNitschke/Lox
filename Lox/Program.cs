namespace Lox;

public class Program
{
	public static void Main(string[] args)
	{
		if (args.Length > 1)
		{
			Console.WriteLine("Usage: Lox [script]");
			Environment.Exit(64);
		}
		else if (args.Length == 1)
			new Program().RunFile(args[0]);
		else
			new Program().RunPrompt();
	}

	private Program() => error = new ConsoleErrorReporter();
	private readonly ConsoleErrorReporter error;

	private void RunFile(string filename)
	{
		Run(File.ReadAllText(filename));
		if (error.HadError)
			Environment.Exit(65);
	}

	private void Run(string code)
	{
		foreach (var token in new Scanner(code, error).Tokens)
			Console.WriteLine(token);
	}

	private void RunPrompt()
	{
		do
		{
			Console.Write("> ");
			var line = Console.ReadLine();
			if (string.IsNullOrEmpty(line))
				return;
			Run(line);
			error.Reset();
		} while (true);
	}
}