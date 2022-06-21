namespace Lox;

public interface StatementVisitor<out T>
{
	T VisitPrintStatement(PrintStatement printStatement);
	T VisitExpressionStatement(ExpressionStatement expressionStatement);
	T VisitVariableStatement(VariableStatement variableStatement);
	T VisitBlockStatement(BlockStatement blockStatement);
	T VisitIfStatement(IfStatement ifStatement);
	T VisitWhileStatement(WhileStatement whileStatement);
	T VisitFunctionStatement(FunctionStatement functionStatement);
	T VisitReturnStatement(ReturnStatement returnStatement);
	T VisitClassStatement(ClassStatement classStatement);
}