using Lox.Exception;

namespace Lox;

public sealed class Scanner
{
	public Scanner(string code, ErrorReporter error)
	{
		this.code = code;
		this.error = error;
		while (!IsAtEnd())
		{
			start = current;
			ScanToken();
		}
		tokens.Add(new Token(TokenType.Eof, "", null, line));
	}

	private readonly string code;
	private readonly ErrorReporter error;
	private bool IsAtEnd() => current >= code.Length;
	private readonly int start;
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
			throw new UnexpectedCharacter(line);
	}

	// ReSharper disable once CyclomaticComplexity
	private Token? GetNextTokenSingleCharacter(char c) =>
		c switch
		{
			'(' => AddToken(TokenType.LeftParenthesis),
			')' => AddToken(TokenType.RightParenthesis),
			'{' => AddToken(TokenType.LeftBrace),
			'}' => AddToken(TokenType.RightBrace),
			',' => AddToken(TokenType.Comma),
			'.' => AddToken(TokenType.Dot),
			'-' => AddToken(TokenType.Minus),
			'+' => AddToken(TokenType.Plus),
			';' => AddToken(TokenType.Semicolon),
			'*' => AddToken(TokenType.Star),
			_ => null
		};

	private Token? GetNextTokenComparisons(char c) =>
		c switch
		{
			'!' => AddToken(Match('=')
				? TokenType.BangEqual
				: TokenType.Bang),
			'=' => AddToken(Match('=')
				? TokenType.EqualEqual
				: TokenType.Equal),
			'<' => AddToken(Match('=')
				? TokenType.LessEqual
				: TokenType.Less),
			'>' => AddToken(Match('=')
				? TokenType.GreaterEqual
				: TokenType.Greater),
			_ => null
		};

	private Token? GetCommentOrSlash(char c)
	{
		if (c != '/')
			return null;
		if (!Match('/'))
			return AddToken(TokenType.Slash);
		// A comment goes until the end of the line.
		while (Peek() != '\n' && !IsAtEnd())
			Advance();
		return NoToken();
	}

	private static Token? IgnoreWhiteSpace(char c) =>
		c is ' ' or '\r' or '\t'
			? NoToken()
			: null;

	private Token? HandleNewLine(char c)
	{
		if (c is not '\n')
			return null;
		line++;
		return NoToken();
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
	private static Token NoToken() => new(TokenType.False, "", null, 0);

	private Token AddToken(TokenType type, object? literal = null)
	{
		var token = new Token(type, code.Substring(start, current - start), literal, line);
		// should be done at caller
		tokens.Add(token);
		return token;
	}

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
		// The closing "
		Advance();
		// Trim the surrounding quotes.
		return AddToken(TokenType.String, code.Substring(start + 1, current - start - 2));
	}

	private static bool IsDigit(char c) => c is >= '0' and <= '9';

	private Token AddNumber()
	{
		while (IsDigit(Peek()))
			Advance();
		if (Peek() == '.' && IsDigit(PeekNext()))
		{
			// Consume the "."
			Advance();
			while (IsDigit(Peek()))
				Advance();
		}
		return AddToken(TokenType.Number, double.Parse(code.Substring(start, current - start)));
	}

	private char PeekNext() =>
		current + 1 >= code.Length
			? '\0'
			: code[current + 1];

	private Token AddIdentifier()
	{
		while (IsAlphaNumeric(Peek()))
			Advance();
		var text = code.Substring(start, current - start);
		return AddToken(Keywords.TryGetValue(text, out var type)
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