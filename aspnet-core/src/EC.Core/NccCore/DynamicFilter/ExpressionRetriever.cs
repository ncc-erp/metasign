using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NccCore.DynamicFilter
{
    public static class ExpressionRetriever
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", 0, new Type[] { typeof(string) });
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        public static Expression GetExpression<T>(ParameterExpression param, ExpressionFilter filter, Type propertyType)
        {
            MemberExpression member = Expression.Property(param, filter.ActualPropertyName);
            ConstantExpression constant = Expression.Constant(filter.ActualValue, propertyType);
            string s = string.Empty;
            switch (filter.Comparision)
            {
                case ComparisionOperator.Equal:
                    return Expression.Equal(member, constant);
                case ComparisionOperator.GreaterThan:
                    return Expression.GreaterThan(member, constant);
                case ComparisionOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);
                case ComparisionOperator.LessThan:
                    return Expression.LessThan(member, constant);
                case ComparisionOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);
                case ComparisionOperator.NotEqual:
                    return Expression.NotEqual(member, constant);
                case ComparisionOperator.Contains:
                    return Expression.Call(ToLowerMember(member), containsMethod, constant);
                case ComparisionOperator.StartsWith:
                    return Expression.Call(member, startsWithMethod, constant);
                case ComparisionOperator.EndsWith:
                    return Expression.Call(member, endsWithMethod, constant);
                case ComparisionOperator.In:
                    return constant.ListContains(member);
                default:
                    return null;
            }
        }

        private static Expression ToLowerMember(MemberExpression member)
        {
            var toLowerMethodInfo = typeof(string).GetMethod("ToLower", new Type[] { });
            if (toLowerMethodInfo == null)
            {
                return null;
            }

            return Expression.Call(member, toLowerMethodInfo);
        }
    }
}
