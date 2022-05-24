using System;
using NUnit.Framework;

namespace Lox.Tests;

public sealed class LoxTests
{
	[Test]
	public void RunLoxFileWithInvalidCharacterFile()
	{
		try
		{
			Lox.Main(new[] { @"C:\Code\Murali\Lox\Lox.Tests\Examples\InvalidCharacters.lox" });
		} //ncrunch: no coverage
		catch (AggregateException ex)
		{
			Assert.That(ex.InnerExceptions[0], Is.InstanceOf<Scanner.UnexpectedCharacter>());
			Assert.That(ex.InnerExceptions[0].Message, Is.EqualTo(@"$\n   :at InvalidCharacters.lox in C:\Code\Murali\Lox\Lox.Tests\Examples\InvalidCharacters.lox :line 1"));
			Assert.That(ex.InnerExceptions[1].Message, Is.EqualTo(@"#\n   :at InvalidCharacters.lox in C:\Code\Murali\Lox\Lox.Tests\Examples\InvalidCharacters.lox :line 2"));
			Assert.That(ex.InnerExceptions[2].Message, Is.EqualTo(@"&\n   :at InvalidCharacters.lox in C:\Code\Murali\Lox\Lox.Tests\Examples\InvalidCharacters.lox :line 3"));
			Assert.That(ex.InnerExceptions[3].Message, Is.EqualTo(@"|\n   :at InvalidCharacters.lox in C:\Code\Murali\Lox\Lox.Tests\Examples\InvalidCharacters.lox :line 4"));
		}
	}
}