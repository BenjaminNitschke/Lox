namespace Expressions;

public sealed record Token(TokenType Type, string Lexeme, object? Literal, int Line);