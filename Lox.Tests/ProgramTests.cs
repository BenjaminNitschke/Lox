using System;
using System.IO;
using NUnit.Framework;

namespace Lox.Tests;

public sealed class ProgramTests
{
	[Test]
	public void RunLoxFileWithInvalidCharacterFile()
	{
		try
		{
			var currentDir = Directory.GetCurrentDirectory();
			Program.Main(new[] { currentDir + @"\..\..\..\Examples\InvalidCharacters.lox" });
		} //ncrunch: no coverage
		catch (AggregateException ex)
		{
			Assert.That(ex.InnerExceptions[0], Is.InstanceOf<Scanner.UnexpectedCharacter>());
			Assert.That(ex.InnerExceptions[0].Message, Contains.Substring(@"$\n   :at InvalidCharacters.lox in"));
			Assert.That(ex.InnerExceptions[1].Message, Contains.Substring(@"#\n   :at InvalidCharacters.lox in"));
			Assert.That(ex.InnerExceptions[2].Message, Contains.Substring(@"&\n   :at InvalidCharacters.lox in"));
			Assert.That(ex.InnerExceptions[3].Message, Contains.Substring(@"|\n   :at InvalidCharacters.lox in"));
		}
	}

	[Test]
	public void PrintFibonacciNumbersIn10000()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\Fibonacci.lox" });
		Assert.That(stringWriter.ToString(),
			Is.EqualTo(
				"0\r\n1\r\n1\r\n2\r\n3\r\n5\r\n8\r\n13\r\n21\r\n34\r\n55\r\n89\r\n144\r\n233\r\n377\r\n610\r\n987\r\n1597\r\n2584\r\n4181\r\n6765\r\n"));
	}
}