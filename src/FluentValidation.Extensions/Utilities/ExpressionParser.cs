using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace FluentValidation.Extensions.Utilities
{
    public static class ExpressionParser
    {
        public static String ConvertToJSCompliantString(Expression expression)
        {
            Type currentType = expression.GetType();
            if (typeof(BinaryExpression).IsAssignableFrom(expression.GetType()))
            {
                var binary = (BinaryExpression)expression;
                var nodeType = String.Empty;
                switch (binary.NodeType)
                {
                    case ExpressionType.And:
                        nodeType = "&";
                        break;
                    case ExpressionType.Or:
                        nodeType = "|";
                        break;
                    case ExpressionType.AndAlso:
                        nodeType = "&&";
                        break;
                    case ExpressionType.OrElse:
                        nodeType = "||";
                        break;
                    case ExpressionType.Equal:
                        nodeType = "==";
                        break;
                    case ExpressionType.NotEqual:
                        nodeType = "!=";
                        break;
                    case ExpressionType.LessThan:
                        nodeType = "<";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        nodeType = "<=";
                        break;
                    case ExpressionType.GreaterThan:
                        nodeType = ">";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        nodeType = ">=";
                        break;
                    default:
                        nodeType = String.Format("--Binary expression '{0}' not supported.--", binary.NodeType.ToString());
                        break;
                }
                return String.Format("({0}{1}{2})", ConvertToJSCompliantString(binary.Left), nodeType, ConvertToJSCompliantString(binary.Right));
            }
            if (typeof(MemberExpression).IsAssignableFrom(expression.GetType()))
            {
                var member = (MemberExpression)expression;
                if (member.NodeType == ExpressionType.MemberAccess)
                {
                    if (member.Member.Name=="Length")
                        return String.Format("{0}.length",ConvertToJSCompliantString(member.Expression));
                    return String.Format("$(form.{0}).val()", member.Member.Name);
                }
            }
            if (typeof(ConstantExpression).IsAssignableFrom(expression.GetType()))
            {
                var constant = (ConstantExpression)expression;
                if (constant.Value == null)
                    return "''";
                else
                {
                    if (typeof(int).IsAssignableFrom(constant.Value.GetType()))
                        return constant.Value.ToString();
                    else return String.Format("'{0}'",constant.Value);
                }
            }
            if (typeof(MethodCallExpression).IsAssignableFrom(expression.GetType()))
            {
                var method = (MethodCallExpression)expression;
                if (method.Method.Name == "IsNullOrEmpty")
                    return String.Format("(!{0})", ConvertToJSCompliantString(method.Arguments[0]));
                return String.Format("--Method '{0}' not supported.--", method.Method.Name);
            }
            if (typeof(UnaryExpression).IsAssignableFrom(expression.GetType()))
            {
                var unary = (UnaryExpression)expression;
                if (unary.NodeType == ExpressionType.Not)
                    return String.Format("(!{0})", ConvertToJSCompliantString(unary.Operand));
                return String.Format("--Unary '{0}' not supported.--", unary.NodeType.ToString());
            }
            return String.Format("--Expression '{0}' not supported.--", expression.ToString());
        }
    }
}
