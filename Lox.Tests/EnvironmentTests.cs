using NUnit.Framework;

namespace Lox.Tests;

public sealed class EnvironmentTests
{
	[Test]
	public void AssignVariableToEnclosedEnvironment()
	{
		var innerEnvironment = new Environment();
		innerEnvironment.Define("a", 10);
		var environment = new Environment(innerEnvironment);
		environment.Assign(new Token(TokenType.Var, "a", "", 1), 5);
	}

	[Test]
	public void AccessVariableFromEnclosedEnvironment()
	{
		var innerEnvironment = new Environment();
		innerEnvironment.Define("a", 10);
		var environment = new Environment(innerEnvironment);
		Assert.That(environment.Get(new Token(TokenType.Var, "a", "", 1)), Is.EqualTo(10));
	}
}