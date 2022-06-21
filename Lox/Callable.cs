namespace Lox;

public interface Callable
{
	int Arity();
	object Call(StatementInterpreter statementInterpreter, List<object> arguments);
}