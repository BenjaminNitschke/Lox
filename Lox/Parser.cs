namespace Lox;

// ReSharper disable once ClassTooBig
public sealed class Parser
{
	public Parser(IReadOnlyList<Token> tokens) => this.tokens = tokens;
	private readonly IReadOnlyList<Token> tokens;
	public IReadOnlyList<Expression> Expressions => new List<Expression> { ParseExpression() };
	private bool IsAtEnd() => Peek().Type == TokenType.Eof;
	private Token Peek() => tokens.ElementAt(currentTokenCount);
	private Token Previous() => tokens.ElementAt(currentTokenCount - 1);
	private int currentTokenCount;

	public List<Statement> Parse()
	{
		var statements = new List<Statement>();
		while (!IsAtEnd())
		{
			var statement = Declaration();
			statements.Add(statement);
		}
		return statements;
	}

	private Statement Declaration() =>
		Match(TokenType.Class)
			? ParseClassDeclaration()
			: Match(TokenType.Fun)
				? ParseFunctionStatement("function")
				: Match(TokenType.Var)
					? ParseVariableDeclarationStatement()
					: ParseStatement();

	private Statement ParseClassDeclaration()
	{
		var name = Consume(TokenType.Identifier, "Expect class name");
		Consume(TokenType.LeftBrace, "Expect '{' before class body");
		var methods = new List<Statement.FunctionStatement>();
		while (!Check(TokenType.RightBrace) && !IsAtEnd())
			methods.Add(ParseFunctionStatement("function"));
		Consume(TokenType.RightBrace, "Expect '}' after class body");
		return new Statement.ClassStatement(name, methods);
	}

	private Statement.FunctionStatement ParseFunctionStatement(string kind)
	{
		var name = Consume(TokenType.Identifier, "Expect " + kind + " name.");
		Consume(TokenType.LeftParenthesis, "Expect '(' after " + kind + " name.");
		var parameters = new List<Token>();
		if (!Check(TokenType.RightParenthesis))
			do
			{
				if (parameters.Count >= 255)
					throw new ArgumentOutOfRangeException("Cannot have more than 255 arguments for type " + kind);
				parameters.Add(Consume(TokenType.Identifier, "Expect parameter name."));
			} while (Match(TokenType.Comma));
		Consume(TokenType.RightParenthesis, "Expect ')' after parameters.");
		Consume(TokenType.LeftBrace, "Expect '{' before " + kind + " body.");
		var body = ParseBlockStatement();
		return new Statement.FunctionStatement(name, parameters, body);
	}

	private Statement ParseVariableDeclarationStatement()
	{
		var name = Consume(TokenType.Identifier);
		Expression? initializer = null;
		if (Match(TokenType.Equal))
			initializer = ParseExpression();
		Consume(TokenType.Semicolon);
		return new Statement.VariableStatement(name, initializer);
	}

	private Statement ParseStatement() =>
		Match(TokenType.For)
			? ParseForStatement()
			: Match(TokenType.If)
				? ParseIfStatement()
				: Match(TokenType.Print)
					? ParsePrintStatement()
					: Match(TokenType.Return)
						? ParseReturnStatement()
						: Match(TokenType.While)
							? ParseWhileStatement()
							: Match(TokenType.LeftBrace)
								? new Statement.BlockStatement(ParseBlockStatement())
								: ParseExpressionStatement();

	private Statement ParseReturnStatement()
	{
		Previous();
		Expression? value = null;
		if (!Check(TokenType.Semicolon))
			value = ParseExpression();
		Consume(TokenType.Semicolon, "Expect ';' after return value");
		return new Statement.ReturnStatement(value);
	}

	private Statement ParseForStatement()
	{
		Consume(TokenType.LeftParenthesis, "Expect '(' after for");
		var initializer = GetLoopInitializer();
		var condition = GetLoopCondition();
		var incrementer = GetLoopIncrementer();
		var body = ParseStatement();
		body = AddIncrementerBodyStatements(incrementer, body);
		condition ??=
			new Expression.LiteralExpression(true, new Token(TokenType.True, "True", "True", 0));
		body = new Statement.WhileStatement(condition, body);
		body = AddInitializerBodyStatements(initializer, body);
		return body;
	}

	private static Statement AddInitializerBodyStatements(Statement? initializer, Statement body)
	{
		if (initializer != null)
			body = new Statement.BlockStatement(new List<Statement> { initializer, body });
		return body;
	}

	private static Statement AddIncrementerBodyStatements(Expression? increment, Statement body)
	{
		if (increment != null)
			body = new Statement.BlockStatement(new List<Statement>
			{
				body, new Statement.ExpressionStatement(increment)
			});
		return body;
	}

	private Expression? GetLoopIncrementer()
	{
		Expression? increment = null;
		if (!Check(TokenType.RightParenthesis))
			increment = ParseExpression();
		Consume(TokenType.RightParenthesis, "Expect ')' after for clauses");
		return increment;
	}

	private Expression? GetLoopCondition()
	{
		Expression? condition = null;
		if (!Check(TokenType.Semicolon))
			condition = ParseExpression();
		Consume(TokenType.Semicolon, "Expect ';' after loop condition");
		return condition;
	}

	private Statement? GetLoopInitializer()
	{
		Statement? initializer;
		if (Match(TokenType.Semicolon))
			initializer = null;
		else if (Match(TokenType.Var))
			initializer = ParseVariableDeclarationStatement();
		else
			initializer = ParseExpressionStatement();
		return initializer;
	}

	private Statement ParseIfStatement()
	{
		Consume(TokenType.LeftParenthesis);
		var conditionExpression = ParseExpression();
		Consume(TokenType.RightParenthesis);
		var thenStatement = ParseStatement();
		Statement? elseStatement = null;
		if (Match(TokenType.Else))
			elseStatement = ParseStatement();
		return new Statement.IfStatement(conditionExpression, thenStatement, elseStatement);
	}

	private Statement ParsePrintStatement()
	{
		var value = ParseExpression();
		Consume(TokenType.Semicolon);
		return new Statement.PrintStatement(value);
	}

	private Statement ParseWhileStatement()
	{
		Consume(TokenType.LeftParenthesis, "Expect '(' after while");
		var condition = ParseExpression();
		Consume(TokenType.RightParenthesis, "Expect ')' after while condition");
		var body = ParseStatement();
		return new Statement.WhileStatement(condition, body);
	}

	private List<Statement> ParseBlockStatement()
	{
		List<Statement> statements = new();
		while (!Check(TokenType.RightBrace) && !IsAtEnd())
		{
			var stmt = Declaration();
			statements.Add(stmt);
		}
		Consume(TokenType.RightBrace);
		return statements;
	}

	private Statement ParseExpressionStatement()
	{
		var expression = ParseExpression();
		Consume(TokenType.Semicolon);
		return new Statement.ExpressionStatement(expression);
	}

	private Expression ParseExpression() => ParseAssignmentExpression();

	private Expression ParseAssignmentExpression()
	{
		var expression = ParseOrExpression();
		if (Match(TokenType.Equal))
		{
			var equals = Previous();
			var value = ParseAssignmentExpression();
			if (expression is Expression.VariableExpression variableExpression)
			{
				var name = variableExpression.name;
				return new Expression.AssignmentExpression(name, value);
			}
			if (expression is Expression.GetExpression getExpression)
				return new Expression.SetExpression(getExpression.expression, getExpression.name, value);
			throw new InvalidAssignmentTarget(equals);
		}
		return expression;
	}

	private Expression ParseOrExpression()
	{
		var expression = ParseAndExpression();
		while (Match(TokenType.Or))
		{
			var operatorToken = Previous();
			var right = ParseAndExpression();
			expression = new Expression.LogicalExpression(expression, right, operatorToken);
		}
		return expression;
	}

	private Expression ParseAndExpression()
	{
		var expression = ParseEqualityExpression();
		while (Match(TokenType.And))
		{
			var operatorToken = Previous();
			var right = ParseEqualityExpression();
			expression = new Expression.LogicalExpression(expression, right, operatorToken);
		}
		return expression;
	}

	public class InvalidAssignmentTarget : ParsingFailed
	{
		public InvalidAssignmentTarget(Token token) : base(token) { }
	}

	private Expression ParseEqualityExpression()
	{
		var expression = ParseComparisonExpressions();
		while (Match(TokenType.EqualEqual, TokenType.BangEqual))
			expression =
				new Expression.BinaryExpression(expression, Previous(), ParseComparisonExpressions());
		return expression;
	}

	private Expression ParseComparisonExpressions()
	{
		var expression = ParseArithmeticExpressions();
		while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
			expression =
				new Expression.BinaryExpression(expression, Previous(), ParseArithmeticExpressions());
		return expression;
	}

	private Expression ParseArithmeticExpressions()
	{
		var expression = ParseFactorExpressions();
		while (Match(TokenType.Plus, TokenType.Minus))
			expression = new Expression.BinaryExpression(expression,
				Previous(), ParseFactorExpressions());
		return expression;
	}

	private Expression ParseFactorExpressions()
	{
		var expression = ParseUnaryExpressions();
		while (Match(TokenType.Slash, TokenType.Star))
			expression = new Expression.BinaryExpression(expression,
				Previous(), ParseUnaryExpressions());
		return expression;
	}

	private Expression ParseUnaryExpressions() =>
		Match(TokenType.Bang, TokenType.Minus)
			? new Expression.UnaryExpression(Previous(), ParseUnaryExpressions())
			: ParseFunctionCall();

	private Expression ParseFunctionCall()
	{
		var expression = ParsePrimaryExpressions();
		while (true)
			if (Match(TokenType.LeftParenthesis))
				expression = ParseFunctionBody(expression);
			else if (Match(TokenType.Dot))
			{
				var name = Consume(TokenType.Identifier, "Expect property name after '.'.");
				expression = new Expression.GetExpression(expression, name);
			}
			else
				break;
		return expression;
	}

	private Expression ParseFunctionBody(Expression callee)
	{
		var arguments = new List<Expression>();
		if (!Check(TokenType.RightParenthesis))
			do
			{
				if (arguments.Count >= 255)
					throw new ArgumentOutOfRangeException(
						(callee as Expression.LiteralExpression)?.Literal?.ToString(),
						"Cannot have more than 255 arguments");
				arguments.Add(ParseExpression());
			} while (Match(TokenType.Comma));
		var parenthesis = Consume(TokenType.RightParenthesis, "Expect ')' after arguments");
		return new Expression.CallExpression(callee, parenthesis, arguments);
	}

	// ReSharper disable once MethodTooLong
	private Expression ParsePrimaryExpressions()
	{
		if (Match(TokenType.False))
			return new Expression.LiteralExpression(false, Previous());
		if (Match(TokenType.True))
			return new Expression.LiteralExpression(true, Previous());
		if (Match(TokenType.Nil))
			return new Expression.LiteralExpression(null, Previous());
		if (Match(TokenType.Number, TokenType.String))
			return new Expression.LiteralExpression(Previous().Literal, Previous());
		if (Match(TokenType.This))
			return new Expression.ThisExpression(Previous());
		if (Match(TokenType.Identifier))
			return new Expression.VariableExpression(Previous());
		if (ParseGroupingExpression(out var groupingExpression))
			return groupingExpression ?? throw new UnknownExpression(Peek());
		throw new UnknownExpression(Peek());
	}

	private bool ParseGroupingExpression(out Expression? groupingExpression)
	{
		groupingExpression = null;
		if (Match(TokenType.LeftParenthesis))
		{
			var expression = ParseExpression();
			Consume(TokenType.RightParenthesis);
			{
				groupingExpression = new Expression.GroupingExpression(expression);
				return true;
			}
		}
		return false;
	}

	private Token Consume(TokenType type, string message = "")
	{
		if (Check(type))
			return Advance();
		throw type switch
		{
			TokenType.RightParenthesis => new MissingClosingParenthesis(new Token(type, "", null, Peek().Line)),
			TokenType.Identifier => new MissingVariableName(new Token(type, "", null, Peek().Line), message),
			TokenType.Semicolon => new MissingSemicolon(new Token(type, "", null, Peek().Line)),
			TokenType.RightBrace => new MissingRightBrace(new Token(type, "", null, Peek().Line), message),
			TokenType.LeftParenthesis => new MissingLeftParenthesis(new Token(type, "", null, Peek().Line), message),
			TokenType.LeftBrace => new MissingLeftBrace(new Token(type, "", null, Peek().Line), message),
			_ => new InvalidOperationException() //ncrunch: no coverage
		};
	}

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

	private bool Match(params TokenType[] tokenTypes)
	{
		foreach (var type in tokenTypes)
			if (Check(type))
			{
				Advance();
				return true;
			}
		return false;
	}

	private bool Check(TokenType tokenType)
	{
		if (IsAtEnd())
			return false;
		return Peek().Type == tokenType;
	}

	private Token Advance()
	{
		if (!IsAtEnd())
			currentTokenCount++;
		return Previous();
	}
}

public class ParsingFailed : OperationFailed
{
	protected ParsingFailed(Token token, string message = "") : base(message + " " + token.Type, token.Line) { }
}



