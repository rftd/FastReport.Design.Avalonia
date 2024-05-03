using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FastReport.Designer.Extensions;

internal static class ExpressionsExtensions
{
    /// <summary>
    /// Gets a MemberInfo from a member expression.
    /// </summary>
    public static MemberInfo? GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression) {
        var memberExp = RemoveUnary(expression.Body) as MemberExpression;

        if (memberExp == null) {
            return null;
        }

        var currentExpr = memberExp.Expression;

        // Unwind the expression to get the root object that the expression acts upon.
        while (true)
        {
            if (currentExpr == null) continue;
            currentExpr = RemoveUnary(currentExpr);

            if (currentExpr.NodeType == ExpressionType.MemberAccess)
            {
                currentExpr = ((MemberExpression)currentExpr).Expression;
            }
            else
            {
                break;
            }
        }

        return currentExpr.NodeType != ExpressionType.Parameter ? null : 
            memberExp.Member;
    }

    private static Expression RemoveUnary(Expression toUnwrap) {
        if (toUnwrap is UnaryExpression expression) {
            return expression.Operand;
        }

        return toUnwrap;
    }
}