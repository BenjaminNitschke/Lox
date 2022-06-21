using Expressions;

namespace Lox;

public sealed class StatementParser : ExpressionParser
{
	public StatementParser(IReadOnlyList<Token> tokens) : base(tokens) { }

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
		VariableExpression? superClass = null;
		if (Match(TokenType.Less))
		{
			Consume(TokenType.Identifier, "Expect Super Class Name");
			superClass = new VariableExpression(Previous());
		}
		Consume(TokenType.LeftBrace, "Expect '{' before class body");
		var methods = new List<FunctionStatement>();
		while (!Check(TokenType.RightBrace) && !IsAtEnd())
			methods.Add(ParseFunctionStatement("function"));
		Consume(TokenType.RightBrace, "Expect '}' after class body");
		return new ClassStatement(name, methods, superClass);
	}

	private FunctionStatement ParseFunctionStatement(string kind)
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
		return new FunctionStatement(name, parameters, body);
	}

	private Statement ParseVariableDeclarationStatement()
	{
		var name = Consume(TokenType.Identifier);
		Expression? initializer = null;
		if (Match(TokenType.Equal))
			initializer = ParseExpression();
		Consume(TokenType.Semicolon);
		return new VariableStatement(name, initializer);
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
								? new BlockStatement(ParseBlockStatement())
								: ParseExpressionStatement();

	private Statement ParseReturnStatement()
	{
		Previous();
		Expression? value = null;
		if (!Check(TokenType.Semicolon))
			value = ParseExpression();
		Consume(TokenType.Semicolon, "Expect ';' after return value");
		return new ReturnStatement(value);
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
			new LiteralExpression(true, new Token(TokenType.True, "True", "True", 0));
		body = new WhileStatement(condition, body);
		body = AddInitializerBodyStatements(initializer, body);
		return body;
	}

	private static Statement AddInitializerBodyStatements(Statement? initializer, Statement body)
	{
		if (initializer != null)
			body = new BlockStatement(new List<Statement> { initializer, body });
		return body;
	}

	private static Statement AddIncrementerBodyStatements(Expression? increment, Statement body)
	{
		if (increment != null)
			body = new BlockStatement(new List<Statement>
			{
				body, new ExpressionStatement(increment)
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
		return new IfStatement(conditionExpression, thenStatement, elseStatement);
	}

	private Statement ParsePrintStatement()
	{
		var value = ParseExpression();
		Consume(TokenType.Semicolon);
		return new PrintStatement(value);
	}

	private Statement ParseWhileStatement()
	{
		Consume(TokenType.LeftParenthesis, "Expect '(' after while");
		var condition = ParseExpression();
		Consume(TokenType.RightParenthesis, "Expect ')' after while condition");
		var body = ParseStatement();
		return new WhileStatement(condition, body);
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
		return new ExpressionStatement(expression);
	}
}