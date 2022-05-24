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
}