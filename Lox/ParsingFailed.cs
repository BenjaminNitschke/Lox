namespace Lox;

public class ParsingFailed : OperationFailed
{
	protected ParsingFailed(Token token, string message = "") : base(message + " " + token.Type, token.Line) { }

	public sealed class MissingSemicolon : ParsingFailed
	{
		public MissingSemicolon(Token token) : base(token) { }
	}

	public sealed class MissingClosingParenthesis : ParsingFailed
	{
		public MissingClosingParenthesis(Token token) : base(token) { }
	}

	public sealed class UnknownExpression : ParsingFailed
	{
		public UnknownExpression(Token token) : base(token) { }
	}

	public sealed class MissingVariableName : ParsingFailed
	{
		public MissingVariableName(Token token, string message = "") : base(token, message) { }
	}

	public sealed class MissingRightBrace : ParsingFailed
	{
		public MissingRightBrace(Token token, string message = "") : base(token, message) { }
	}

	public sealed class MissingLeftParenthesis : ParsingFailed
	{
		public MissingLeftParenthesis(Token token, string message = "") : base(token, message) { }
	}

	public sealed class MissingLeftBrace : ParsingFailed
	{
		public MissingLeftBrace(Token token, string message = "") : base(token, message) { }
	}
}