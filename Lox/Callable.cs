namespace Lox;

public interface Callable
{
	int Arity();
	object Call(Interpreter interpreter, List<object> arguments);
}