using System.Linq;
using Lox.Exception;
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

		[Test]
		public void ParseIfStatement()
		{
			var scanner = new Scanner(@"if(5 == 5) print 23;", error);
			Assert.That(scanner.Tokens.Select(t => t.Type).ToArray(), Is.EqualTo(new[]
			{
				TokenType.If, TokenType.LeftParenthesis, TokenType.Number, TokenType.EqualEqual,
				TokenType.Number, TokenType.RightParenthesis, TokenType.Print, TokenType.Number,
				TokenType.Semicolon, TokenType.Eof
			}));
		}

		[Test]
		public void ParseStringLineIncrement()
		{
			var scanner = new Scanner("\"testText\n\"", error);
			Assert.That(scanner.Tokens[0].Line, Is.EqualTo(2));
		}

		[Test]
		public void ParseInvalidCode() =>
			Assert.That(() => new Scanner("#", error), Throws.InstanceOf<UnexpectedCharacter>());

		[Test]
		public void ParseUnterminatedString() =>
			Assert.That(() => new Scanner("\"testText", error),
				Throws.InstanceOf<UnterminatedString>());

		[TestCase(".", TokenType.Dot)]
		[TestCase(",", TokenType.Comma)]
		[TestCase("25.4", TokenType.Number)]
		[TestCase(">", TokenType.Greater)]
		public void ParseTokenTypes(string code, TokenType expectedTokenType)
		{
			var scanner = new Scanner(code, error);
			Assert.That(scanner.Tokens.Select(t => t.Type).FirstOrDefault(), Is.EqualTo(expectedTokenType));
		}
	}
}