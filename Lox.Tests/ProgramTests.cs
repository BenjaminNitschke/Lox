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
	public void PrintHelloWorld()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\HelloWorld.lox" });
		Assert.That(stringWriter.ToString(),
			Is.EqualTo(
				"Hello, world!\r\n"));
	}

	[Test]
	public void PrintClassName()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\PrintClassName.lox" });
		Assert.That(stringWriter.ToString(),
			Is.EqualTo(
				"Bagel\r\nBagel instance\r\n"));
	}

	[Test]
	public void AccessClassPropertyUsingInstance()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\AccessClassProperty.lox" });
		Assert.That(stringWriter.ToString(),
			Is.EqualTo(
				"Crunch crunch crunch!\r\n"));
	}

	[Test]
	public void PrintUsingThisInstance()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\PrintUsingThisInstance.lox" });
		Assert.That(stringWriter.ToString(),
			Is.EqualTo(
				"The German chocolate cake is delicious!\r\n"));
	}

	[Test]
	public void SuperClassMustBeAClass()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Assert.That(
			() => Program.Main(new[] { currentDir + @"\..\..\..\Examples\SuperClassMustBeAClass.lox" }),
			Throws.InstanceOf<StatementInterpreter.SuperClassMustBeAClass>()!);
	}

	[Test]
	public void AccessBaseClassMethodUsingSuperClassInstance()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\AccessSuperClassInstance.lox" });
		Assert.That(stringWriter.ToString(), Is.EqualTo("Fry until golden brown.\r\nPipe full of custard and coat with chocolate.\r\n"));
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

	//ncrunch: no coverage start
	[Test]
	[Category("Slow")]
	public void PrintFibonacciNumbersWithRecursion()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\RecursiveFibonacci.lox" });
		Assert.That(stringWriter.ToString(),
			Is.EqualTo(
				"0\r\n1\r\n1\r\n2\r\n3\r\n5\r\n8\r\n13\r\n21\r\n34\r\n55\r\n89\r\n144\r\n233\r\n377\r\n610\r\n987\r\n1597\r\n2584\r\n4181\r\n"));
	} //ncrunch: no coverage end

	[Test]
	public void Run99BottlesBeerProgram()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\99Bottles.lox" });
		Assert.That(stringWriter.ToString(),
			Is.EqualTo(
				"99 bottles of beer on the wall, 99 bottles of beer.\r\nTake one down and pass it around, 98 bottles of beer on the wall.\r\n98 bottles of beer on the wall, 98 bottles of beer.\r\nTake one down and pass it around, 97 bottles of beer on the wall.\r\n97 bottles of beer on the wall, 97 bottles of beer.\r\nTake one down and pass it around, 96 bottles of beer on the wall.\r\n96 bottles of beer on the wall, 96 bottles of beer.\r\nTake one down and pass it around, 95 bottles of beer on the wall.\r\n95 bottles of beer on the wall, 95 bottles of beer.\r\nTake one down and pass it around, 94 bottles of beer on the wall.\r\n94 bottles of beer on the wall, 94 bottles of beer.\r\nTake one down and pass it around, 93 bottles of beer on the wall.\r\n93 bottles of beer on the wall, 93 bottles of beer.\r\nTake one down and pass it around, 92 bottles of beer on the wall.\r\n92 bottles of beer on the wall, 92 bottles of beer.\r\nTake one down and pass it around, 91 bottles of beer on the wall.\r\n91 bottles of beer on the wall, 91 bottles of beer.\r\nTake one down and pass it around, 90 bottles of beer on the wall.\r\n90 bottles of beer on the wall, 90 bottles of beer.\r\nTake one down and pass it around, 89 bottles of beer on the wall.\r\n89 bottles of beer on the wall, 89 bottles of beer.\r\nTake one down and pass it around, 88 bottles of beer on the wall.\r\n88 bottles of beer on the wall, 88 bottles of beer.\r\nTake one down and pass it around, 87 bottles of beer on the wall.\r\n87 bottles of beer on the wall, 87 bottles of beer.\r\nTake one down and pass it around, 86 bottles of beer on the wall.\r\n86 bottles of beer on the wall, 86 bottles of beer.\r\nTake one down and pass it around, 85 bottles of beer on the wall.\r\n85 bottles of beer on the wall, 85 bottles of beer.\r\nTake one down and pass it around, 84 bottles of beer on the wall.\r\n84 bottles of beer on the wall, 84 bottles of beer.\r\nTake one down and pass it around, 83 bottles of beer on the wall.\r\n83 bottles of beer on the wall, 83 bottles of beer.\r\nTake one down and pass it around, 82 bottles of beer on the wall.\r\n82 bottles of beer on the wall, 82 bottles of beer.\r\nTake one down and pass it around, 81 bottles of beer on the wall.\r\n81 bottles of beer on the wall, 81 bottles of beer.\r\nTake one down and pass it around, 80 bottles of beer on the wall.\r\n80 bottles of beer on the wall, 80 bottles of beer.\r\nTake one down and pass it around, 79 bottles of beer on the wall.\r\n79 bottles of beer on the wall, 79 bottles of beer.\r\nTake one down and pass it around, 78 bottles of beer on the wall.\r\n78 bottles of beer on the wall, 78 bottles of beer.\r\nTake one down and pass it around, 77 bottles of beer on the wall.\r\n77 bottles of beer on the wall, 77 bottles of beer.\r\nTake one down and pass it around, 76 bottles of beer on the wall.\r\n76 bottles of beer on the wall, 76 bottles of beer.\r\nTake one down and pass it around, 75 bottles of beer on the wall.\r\n75 bottles of beer on the wall, 75 bottles of beer.\r\nTake one down and pass it around, 74 bottles of beer on the wall.\r\n74 bottles of beer on the wall, 74 bottles of beer.\r\nTake one down and pass it around, 73 bottles of beer on the wall.\r\n73 bottles of beer on the wall, 73 bottles of beer.\r\nTake one down and pass it around, 72 bottles of beer on the wall.\r\n72 bottles of beer on the wall, 72 bottles of beer.\r\nTake one down and pass it around, 71 bottles of beer on the wall.\r\n71 bottles of beer on the wall, 71 bottles of beer.\r\nTake one down and pass it around, 70 bottles of beer on the wall.\r\n70 bottles of beer on the wall, 70 bottles of beer.\r\nTake one down and pass it around, 69 bottles of beer on the wall.\r\n69 bottles of beer on the wall, 69 bottles of beer.\r\nTake one down and pass it around, 68 bottles of beer on the wall.\r\n68 bottles of beer on the wall, 68 bottles of beer.\r\nTake one down and pass it around, 67 bottles of beer on the wall.\r\n67 bottles of beer on the wall, 67 bottles of beer.\r\nTake one down and pass it around, 66 bottles of beer on the wall.\r\n66 bottles of beer on the wall, 66 bottles of beer.\r\nTake one down and pass it around, 65 bottles of beer on the wall.\r\n65 bottles of beer on the wall, 65 bottles of beer.\r\nTake one down and pass it around, 64 bottles of beer on the wall.\r\n64 bottles of beer on the wall, 64 bottles of beer.\r\nTake one down and pass it around, 63 bottles of beer on the wall.\r\n63 bottles of beer on the wall, 63 bottles of beer.\r\nTake one down and pass it around, 62 bottles of beer on the wall.\r\n62 bottles of beer on the wall, 62 bottles of beer.\r\nTake one down and pass it around, 61 bottles of beer on the wall.\r\n61 bottles of beer on the wall, 61 bottles of beer.\r\nTake one down and pass it around, 60 bottles of beer on the wall.\r\n60 bottles of beer on the wall, 60 bottles of beer.\r\nTake one down and pass it around, 59 bottles of beer on the wall.\r\n59 bottles of beer on the wall, 59 bottles of beer.\r\nTake one down and pass it around, 58 bottles of beer on the wall.\r\n58 bottles of beer on the wall, 58 bottles of beer.\r\nTake one down and pass it around, 57 bottles of beer on the wall.\r\n57 bottles of beer on the wall, 57 bottles of beer.\r\nTake one down and pass it around, 56 bottles of beer on the wall.\r\n56 bottles of beer on the wall, 56 bottles of beer.\r\nTake one down and pass it around, 55 bottles of beer on the wall.\r\n55 bottles of beer on the wall, 55 bottles of beer.\r\nTake one down and pass it around, 54 bottles of beer on the wall.\r\n54 bottles of beer on the wall, 54 bottles of beer.\r\nTake one down and pass it around, 53 bottles of beer on the wall.\r\n53 bottles of beer on the wall, 53 bottles of beer.\r\nTake one down and pass it around, 52 bottles of beer on the wall.\r\n52 bottles of beer on the wall, 52 bottles of beer.\r\nTake one down and pass it around, 51 bottles of beer on the wall.\r\n51 bottles of beer on the wall, 51 bottles of beer.\r\nTake one down and pass it around, 50 bottles of beer on the wall.\r\n50 bottles of beer on the wall, 50 bottles of beer.\r\nTake one down and pass it around, 49 bottles of beer on the wall.\r\n49 bottles of beer on the wall, 49 bottles of beer.\r\nTake one down and pass it around, 48 bottles of beer on the wall.\r\n48 bottles of beer on the wall, 48 bottles of beer.\r\nTake one down and pass it around, 47 bottles of beer on the wall.\r\n47 bottles of beer on the wall, 47 bottles of beer.\r\nTake one down and pass it around, 46 bottles of beer on the wall.\r\n46 bottles of beer on the wall, 46 bottles of beer.\r\nTake one down and pass it around, 45 bottles of beer on the wall.\r\n45 bottles of beer on the wall, 45 bottles of beer.\r\nTake one down and pass it around, 44 bottles of beer on the wall.\r\n44 bottles of beer on the wall, 44 bottles of beer.\r\nTake one down and pass it around, 43 bottles of beer on the wall.\r\n43 bottles of beer on the wall, 43 bottles of beer.\r\nTake one down and pass it around, 42 bottles of beer on the wall.\r\n42 bottles of beer on the wall, 42 bottles of beer.\r\nTake one down and pass it around, 41 bottles of beer on the wall.\r\n41 bottles of beer on the wall, 41 bottles of beer.\r\nTake one down and pass it around, 40 bottles of beer on the wall.\r\n40 bottles of beer on the wall, 40 bottles of beer.\r\nTake one down and pass it around, 39 bottles of beer on the wall.\r\n39 bottles of beer on the wall, 39 bottles of beer.\r\nTake one down and pass it around, 38 bottles of beer on the wall.\r\n38 bottles of beer on the wall, 38 bottles of beer.\r\nTake one down and pass it around, 37 bottles of beer on the wall.\r\n37 bottles of beer on the wall, 37 bottles of beer.\r\nTake one down and pass it around, 36 bottles of beer on the wall.\r\n36 bottles of beer on the wall, 36 bottles of beer.\r\nTake one down and pass it around, 35 bottles of beer on the wall.\r\n35 bottles of beer on the wall, 35 bottles of beer.\r\nTake one down and pass it around, 34 bottles of beer on the wall.\r\n34 bottles of beer on the wall, 34 bottles of beer.\r\nTake one down and pass it around, 33 bottles of beer on the wall.\r\n33 bottles of beer on the wall, 33 bottles of beer.\r\nTake one down and pass it around, 32 bottles of beer on the wall.\r\n32 bottles of beer on the wall, 32 bottles of beer.\r\nTake one down and pass it around, 31 bottles of beer on the wall.\r\n31 bottles of beer on the wall, 31 bottles of beer.\r\nTake one down and pass it around, 30 bottles of beer on the wall.\r\n30 bottles of beer on the wall, 30 bottles of beer.\r\nTake one down and pass it around, 29 bottles of beer on the wall.\r\n29 bottles of beer on the wall, 29 bottles of beer.\r\nTake one down and pass it around, 28 bottles of beer on the wall.\r\n28 bottles of beer on the wall, 28 bottles of beer.\r\nTake one down and pass it around, 27 bottles of beer on the wall.\r\n27 bottles of beer on the wall, 27 bottles of beer.\r\nTake one down and pass it around, 26 bottles of beer on the wall.\r\n26 bottles of beer on the wall, 26 bottles of beer.\r\nTake one down and pass it around, 25 bottles of beer on the wall.\r\n25 bottles of beer on the wall, 25 bottles of beer.\r\nTake one down and pass it around, 24 bottles of beer on the wall.\r\n24 bottles of beer on the wall, 24 bottles of beer.\r\nTake one down and pass it around, 23 bottles of beer on the wall.\r\n23 bottles of beer on the wall, 23 bottles of beer.\r\nTake one down and pass it around, 22 bottles of beer on the wall.\r\n22 bottles of beer on the wall, 22 bottles of beer.\r\nTake one down and pass it around, 21 bottles of beer on the wall.\r\n21 bottles of beer on the wall, 21 bottles of beer.\r\nTake one down and pass it around, 20 bottles of beer on the wall.\r\n20 bottles of beer on the wall, 20 bottles of beer.\r\nTake one down and pass it around, 19 bottles of beer on the wall.\r\n19 bottles of beer on the wall, 19 bottles of beer.\r\nTake one down and pass it around, 18 bottles of beer on the wall.\r\n18 bottles of beer on the wall, 18 bottles of beer.\r\nTake one down and pass it around, 17 bottles of beer on the wall.\r\n17 bottles of beer on the wall, 17 bottles of beer.\r\nTake one down and pass it around, 16 bottles of beer on the wall.\r\n16 bottles of beer on the wall, 16 bottles of beer.\r\nTake one down and pass it around, 15 bottles of beer on the wall.\r\n15 bottles of beer on the wall, 15 bottles of beer.\r\nTake one down and pass it around, 14 bottles of beer on the wall.\r\n14 bottles of beer on the wall, 14 bottles of beer.\r\nTake one down and pass it around, 13 bottles of beer on the wall.\r\n13 bottles of beer on the wall, 13 bottles of beer.\r\nTake one down and pass it around, 12 bottles of beer on the wall.\r\n12 bottles of beer on the wall, 12 bottles of beer.\r\nTake one down and pass it around, 11 bottles of beer on the wall.\r\n11 bottles of beer on the wall, 11 bottles of beer.\r\nTake one down and pass it around, 10 bottles of beer on the wall.\r\n10 bottles of beer on the wall, 10 bottles of beer.\r\nTake one down and pass it around, 9 bottles of beer on the wall.\r\n9 bottles of beer on the wall, 9 bottles of beer.\r\nTake one down and pass it around, 8 bottles of beer on the wall.\r\n8 bottles of beer on the wall, 8 bottles of beer.\r\nTake one down and pass it around, 7 bottles of beer on the wall.\r\n7 bottles of beer on the wall, 7 bottles of beer.\r\nTake one down and pass it around, 6 bottles of beer on the wall.\r\n6 bottles of beer on the wall, 6 bottles of beer.\r\nTake one down and pass it around, 5 bottles of beer on the wall.\r\n5 bottles of beer on the wall, 5 bottles of beer.\r\nTake one down and pass it around, 4 bottles of beer on the wall.\r\n4 bottles of beer on the wall, 4 bottles of beer.\r\nTake one down and pass it around, 3 bottles of beer on the wall.\r\n3 bottles of beer on the wall, 3 bottles of beer.\r\nTake one down and pass it around, 2 bottles of beer on the wall.\r\n2 bottles of beer on the wall, 2 bottles of beer.\r\nTake one down and pass it around, 1 bottle of beer on the wall.\r\n1 bottle of beer on the wall, 1 bottle of beer.\r\nTake one down and pass it around, 0 bottles of beer on the wall.\r\nNo more bottles of beer on the wall. No more bottles of beer.\r\nGo to the store and buy some more.\r\n"));
	}

	[Test]
	public void PrintReferenceFunctionVariableUsingClosure()
	{
		var stringWriter = new StringWriter();
		Console.SetOut(stringWriter);
		var currentDir = Directory.GetCurrentDirectory();
		Program.Main(new[] { currentDir + @"\..\..\..\Examples\ClosureExample.lox" });
		Assert.That(stringWriter.ToString(), Is.EqualTo("1\r\n2\r\n"));
	}
}