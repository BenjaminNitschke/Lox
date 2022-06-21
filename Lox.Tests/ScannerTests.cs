using Expressions;

namespace Lox.Tests;

public sealed class ScannerTests
{
	[Test]
	public void ParseEmpty() => Assert.That(() => new Scanner(""), Throws.Nothing);

	[Test]
	public void ParseInvalidCode() =>
		Assert.That(() => new Scanner("#"),
			Throws.InstanceOf<AggregateException>().With.InnerException.With.Message.
				EqualTo("# in  :line 1"));

	[Test]
	public void ParseValidPrintTokenWithNumber() =>
		Assert.That(() => new Scanner("print 9"), Throws.Nothing);

	[Test]
	public void ParseInvalidCodeAfterValidToken() =>
		Assert.That(() => new Scanner("print $"), Throws.InstanceOf<AggregateException>());

	[Test]
	public void ParseUnterminatedString() =>
		Assert.That(() => new Scanner("\"testText"),
			Throws.InstanceOf<AggregateException>().With.InnerException.
				InstanceOf<Scanner.UnterminatedString>());

	[Test]
	public void ParseVariableDeclaration()
	{
		var scanner = new Scanner("var yo = \"lox\";");
		Assert.That(scanner.Tokens.Select(t => t.Type),
			Is.EqualTo(new[]
			{
				TokenType.Var, TokenType.Identifier, TokenType.Equal, TokenType.String,
				TokenType.Semicolon, TokenType.Eof
			}));
	}

	[Test]
	public void ParseHelloWorld()
	{
		var scanner = new Scanner("print \"Hello, world!\";");
		Assert.That(scanner.Tokens.Select(t => t.Type),
			Is.EqualTo(new[]
			{
				TokenType.Print, TokenType.String, TokenType.Semicolon, TokenType.Eof
			}));
	}

	[Test]
	public void ParseMultiLine()
	{
		var scanner = new Scanner(@"// this is a comment
(( )){} // grouping stuff
!*+-/=<> <= == // operators");
		Assert.That(scanner.Tokens, Has.Count.GreaterThan(10));
	}

	[Test]
	public void ParseIfStatement()
	{
		var scanner = new Scanner(@"if(5 == 5) print 23;");
		Assert.That(scanner.Tokens.Select(t => t.Type).ToArray(),
			Is.EqualTo(new[]
			{
				TokenType.If, TokenType.LeftParenthesis, TokenType.Number, TokenType.EqualEqual,
				TokenType.Number, TokenType.RightParenthesis, TokenType.Print, TokenType.Number,
				TokenType.Semicolon, TokenType.Eof
			}));
	}

	[Test]
	public void ParseStringLineIncrement()
	{
		var scanner = new Scanner("\"testText\n\"");
		Assert.That(scanner.Tokens[0].Line, Is.EqualTo(2));
	}

	[TestCase(".", TokenType.Dot)]
	[TestCase(",", TokenType.Comma)]
	[TestCase("25.4", TokenType.Number)]
	[TestCase(">", TokenType.Greater)]
	[TestCase(">=", TokenType.GreaterEqual)]
	public void ParseTokenTypes(string code, TokenType expectedTokenType)
	{
		var scanner = new Scanner(code);
		Assert.That(scanner.Tokens.Select(t => t.Type).FirstOrDefault(),
			Is.EqualTo(expectedTokenType));
	}

	[Test]
	public void ShowMultipleErrors()
	{
		try
		{
			_ = new Scanner(@"
print $
#
&
||");
		} //ncrunch: no coverage
		catch (AggregateException ex)
		{
			Assert.That(ex.InnerExceptions[0], Is.InstanceOf<Scanner.UnexpectedCharacter>());
			Assert.That(ex.InnerExceptions[0].Message, Is.EqualTo(@"$ in  :line 2"));
			Assert.That(ex.InnerExceptions[1].Message, Is.EqualTo(@"# in  :line 3"));
			Assert.That(ex.InnerExceptions[2].Message, Is.EqualTo(@"& in  :line 4"));
			Assert.That(ex.InnerExceptions[3].Message, Is.EqualTo(@"| in  :line 5"));
		}
	}
}