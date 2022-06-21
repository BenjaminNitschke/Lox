namespace Expressions;

public interface ExpressionVisitor<out T>
{
	T VisitLiteralExpression(Expression.LiteralExpression literal);
	T VisitGroupingExpression(Expression.GroupingExpression groupingExpression);
	T VisitBinaryExpression(Expression.BinaryExpression binaryExpression);
	T VisitUnaryExpression(Expression.UnaryExpression unaryExpression);
	T VisitVariableExpression(Expression.VariableExpression variableExpression);
	T VisitAssignmentExpression(Expression.AssignmentExpression assignmentExpression);
	T VisitLogicalExpression(Expression.LogicalExpression logicalExpression);
	T VisitCallExpression(Expression.CallExpression callExpression);
	T VisitGetExpression(Expression.GetExpression getExpression);
	T VisitSetExpression(Expression.SetExpression setExpression);
	T VisitThisExpression(Expression.ThisExpression thisExpression);
	T VisitSuperExpression(Expression.SuperExpression superExpression);
}