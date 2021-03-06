namespace Expressions;

public interface ExpressionVisitor<out T> : ClassExpressionVisitor<T>
{
	T VisitLiteralExpression(LiteralExpression literal);
	T VisitGroupingExpression(GroupingExpression groupingExpression);
	T VisitBinaryExpression(BinaryExpression binaryExpression);
	T VisitUnaryExpression(UnaryExpression unaryExpression);
	T VisitVariableExpression(VariableExpression variableExpression);
	T VisitAssignmentExpression(AssignmentExpression assignmentExpression);
	T VisitLogicalExpression(LogicalExpression logicalExpression);
}

public interface ClassExpressionVisitor<out T>
{
	T VisitCallExpression(CallExpression callExpression);
	T VisitGetExpression(GetExpression getExpression);
	T VisitSetExpression(SetExpression setExpression);
	T VisitThisExpression(ThisExpression thisExpression);
	T VisitSuperExpression(SuperExpression superExpression);
}