namespace Lox;

public sealed class Program
{
	private static readonly Interpreter Interpreter = new();

	public static void Main(string[] args)
	{
		if (args.Length > 1)
		{
			Console.WriteLine("Usage: Lox [script]");
			System.Environment.Exit(64);
		}
		else if (args.Length == 1)
			RunFile(args[0]);
		else
			RunPrompt();
	}

	private static void RunFile(string filename) => Run(File.ReadAllText(filename), filename);

	private static void Run(string code, string filePath = "")
	{
		var statements = new Parser(new Scanner(code, filePath).Tokens).Parse();
		Interpreter.Interpret(statements);
	}

	private static void RunPrompt()
	{
		Console.WriteLine("Lox repl, press Enter to quit");
		do
		{
			Console.Write("> ");
			var line = Console.ReadLine();
			if (string.IsNullOrEmpty(line))
				return;
			try
			{
				Run(line);
			}
			catch (AggregateException ex)
			{
				foreach (var inner in ex.InnerExceptions)
					Console.WriteLine(inner.GetType().Name + " " + inner.Message);
			}
		} while (true);
	}
}