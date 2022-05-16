using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox.Exception;

public class UnexpectedCharacter : System.Exception
{
	public UnexpectedCharacter(int fileLineNumber, string message = "") :
		base(message + " : line " + (fileLineNumber + 1)) { }
}