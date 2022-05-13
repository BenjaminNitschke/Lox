using System.Linq;
using NUnit.Framework;

namespace Lox.Tests
{
	public sealed class ScannerTests
	{
		[SetUp]
		public void CreateErrorReporter() => error = new TestErrorReporter();

		private TestErrorReporter error = null!;

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

		[Test]
		public void ParseVariableDeclaration()
		{
			var scanner = new Scanner("var yo = \"lox\";", error);
			Assert.That(scanner.Tokens.Select(t => t.Type),
				Is.EqualTo(new[]
				{
					TokenType.Var,
					TokenType.Identifier,
					TokenType.Equal,
					TokenType.String,
					TokenType.Semicolon,
					TokenType.Eof
				}));
		}

		[Test]
		public void ParseHelloWorld()
		{
			var scanner = new Scanner("print \"Hello, world!\";", error);
			Assert.That(scanner.Tokens.Select(t => t.Type),
				Is.EqualTo(new[]
				{
					TokenType.Print,
					TokenType.String,
					TokenType.Semicolon,
					TokenType.Eof
				}));
		}

		[Test]
		public void ParseMultiLine()
		{
			var scanner = new Scanner(@"// this is a comment
(( )){} // grouping stuff
!*+-/=<> <= == // operators", error);
			Assert.That(scanner.Tokens, Has.Count.GreaterThan(10));
		}
	}
}