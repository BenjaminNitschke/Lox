namespace Lox;

public interface StatementVisitor<out T>
{
	T VisitPrintStatement(Statement.PrintStatement printStatement);
	T VisitExpressionStatement(Statement.ExpressionStatement expressionStatement);
	T VisitVariableStatement(Statement.VariableStatement variableStatement);
	T VisitBlockStatement(Statement.BlockStatement blockStatement);
	T VisitIfStatement(Statement.IfStatement ifStatement);
	T VisitWhileStatement(Statement.WhileStatement whileStatement);
	T VisitFunctionStatement(Statement.FunctionStatement functionStatement);
	T VisitReturnStatement(Statement.ReturnStatement returnStatement);
	T VisitClassStatement(Statement.ClassStatement classStatement);
}