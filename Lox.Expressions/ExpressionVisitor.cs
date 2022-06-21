namespace Expressions;

public interface ExpressionVisitor<out T>
{
	T VisitLiteralExpression(LiteralExpression literal);
	T VisitGroupingExpression(GroupingExpression groupingExpression);
	T VisitBinaryExpression(BinaryExpression binaryExpression);
	T VisitUnaryExpression(UnaryExpression unaryExpression);
	T VisitVariableExpression(VariableExpression variableExpression);
	T VisitAssignmentExpression(AssignmentExpression assignmentExpression);
	T VisitLogicalExpression(LogicalExpression logicalExpression);
	T VisitCallExpression(CallExpression callExpression);
	T VisitGetExpression(GetExpression getExpression);
	T VisitSetExpression(SetExpression setExpression);
	T VisitThisExpression(ThisExpression thisExpression);
	T VisitSuperExpression(SuperExpression superExpression);
}