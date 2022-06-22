using Expressions;

namespace Lox;

public sealed class Scanner
{
	public Scanner(string code, string filePath = "")
	{
		this.code = code;
		this.filePath = filePath;
		var exceptions = new List<Exception>();
		while (!IsAtEnd())
		{
			startTokenPosition = current;
			try
			{
				ScanToken();
			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
			}
		}
		if (exceptions.Count > 0)
			// ReSharper disable once UnthrowableException
			throw new AggregateException(exceptions);
		tokens.Add(new Token(TokenType.Eof, "", null, line));
	}

	private readonly string code;
	private readonly string filePath;
	private bool IsAtEnd() => current >= code.Length;
	private readonly int startTokenPosition;
	private int current;
	private int line = 1;
	public IReadOnlyList<Token> Tokens => tokens;
	private readonly List<Token> tokens = new();

	private void ScanToken()
	{
		var c = Advance();
		var token = GetNextTokenSingleCharacter(c) ??
			GetNextTokenComparisons(c) ??
			GetCommentOrSlash(c) ??
			IgnoreWhiteSpace(c) ??
			HandleNewLine(c) ??
			HandleString(c) ??
			HandleDigit(c);
		if (token == null)
			throw new UnexpectedCharacter(c, line, filePath);
		if (token.Type != TokenType.Skip)
			tokens.Add(token);
	}

	public sealed class UnexpectedCharacter : OperationFailed
	{
		public UnexpectedCharacter(char character, int fileLineNumber, string filePath) :
			base(character.ToString(), fileLineNumber, filePath) { }
	}

	// ReSharper disable once CyclomaticComplexity
	private Token? GetNextTokenSingleCharacter(char c) =>
		c switch
		{
			'(' => CreateToken(TokenType.LeftParenthesis),
			')' => CreateToken(TokenType.RightParenthesis),
			'{' => CreateToken(TokenType.LeftBrace),
			'}' => CreateToken(TokenType.RightBrace),
			',' => CreateToken(TokenType.Comma),
			'.' => CreateToken(TokenType.Dot),
			'-' => CreateToken(TokenType.Minus),
			'+' => CreateToken(TokenType.Plus),
			';' => CreateToken(TokenType.Semicolon),
			'*' => CreateToken(TokenType.Star),
			_ => null
		};

	private Token? GetNextTokenComparisons(char c) =>
		c switch
		{
			'!' => CreateToken(Match('=')
				? TokenType.BangEqual
				: TokenType.Bang),
			'=' => CreateToken(Match('=')
				? TokenType.EqualEqual
				: TokenType.Equal),
			'<' => CreateToken(Match('=')
				? TokenType.LessEqual
				: TokenType.Less),
			'>' => CreateToken(Match('=')
				? TokenType.GreaterEqual
				: TokenType.Greater),
			_ => null
		};

	private Token? GetCommentOrSlash(char c)
	{
		if (c != '/')
			return null;
		if (!Match('/'))
			return CreateToken(TokenType.Slash);
		// A comment goes until the end of the line.
		while (Peek() != '\n' && !IsAtEnd())
			Advance();
		return SkipToken();
	}

	private static Token? IgnoreWhiteSpace(char c) =>
		c is ' ' or '\r' or '\t'
			? SkipToken()
			: null;

	private Token? HandleNewLine(char c)
	{
		if (c is not '\n')
			return null;
		line++;
		return SkipToken();
	}

	private Token? HandleString(char c) =>
		c is '"'
			? AddString()
			: null;

	private Token? HandleDigit(char c) =>
		IsDigit(c)
			? AddNumber()
			: IsAlpha(c)
				? AddIdentifier()
				: null;

	private char Advance() => code[current++];
	private static Token SkipToken() => new(TokenType.Skip, "", null, 0);

	private Token CreateToken(TokenType type, object? literal = null) =>
		new(type, code.Substring(startTokenPosition, current - startTokenPosition), literal, line);

	private bool Match(char expected)
	{
		if (IsAtEnd())
			return false;
		if (code[current] != expected)
			return false;
		current++;
		return true;
	}

	private char Peek() =>
		IsAtEnd()
			? '\0'
			: code[current];

	private Token AddString()
	{
		while (Peek() != '"' && !IsAtEnd())
		{
			if (Peek() == '\n')
				line++;
			Advance();
		}
		if (IsAtEnd())
			throw new UnterminatedString(line);
		SkipClosingQuote();
		return CreateToken(TokenType.String,
			code.Substring(startTokenPosition + 1, current - startTokenPosition - 2));
	}

	private void SkipClosingQuote() => Advance();

	public sealed class UnterminatedString : Exception
	{
		public UnterminatedString(int fileLineNumber, string message = "") :
			base(message + " : line " + (fileLineNumber + 1)) { }
	}

	private static bool IsDigit(char c) => c is >= '0' and <= '9';

	private Token AddNumber()
	{
		while (IsDigit(Peek()))
			Advance();
		if (Peek() == '.' && IsDigit(PeekNext()))
			ConsumeDotAndDigitsAfter();
		return CreateToken(TokenType.Number, double.Parse(code.Substring(startTokenPosition, current - startTokenPosition)));
	}

	private void ConsumeDotAndDigitsAfter()
	{
		Advance();
		while (IsDigit(Peek()))
			Advance();
	}

	private char PeekNext() =>
		current + 1 >= code.Length
			? '\0'
			: code[current + 1];

	private Token AddIdentifier()
	{
		while (IsAlphaNumeric(Peek()))
			Advance();
		var text = code.Substring(startTokenPosition, current - startTokenPosition);
		return CreateToken(Keywords.TryGetValue(text, out var type)
			? type
			: TokenType.Identifier);
	}

	private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);
	private static bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
	private static readonly Dictionary<string, TokenType> Keywords = new()
	{
		{ "and", TokenType.And },
		{ "class", TokenType.Class },
		{ "else", TokenType.Else },
		{ "false", TokenType.False },
		{ "for", TokenType.For },
		{ "fun", TokenType.Fun },
		{ "if", TokenType.If },
		{ "nil", TokenType.Nil },
		{ "or", TokenType.Or },
		{ "print", TokenType.Print },
		{ "return", TokenType.Return },
		{ "super", TokenType.Super },
		{ "this", TokenType.This },
		{ "true", TokenType.True },
		{ "var", TokenType.Var },
		{ "while", TokenType.While }
	};
}