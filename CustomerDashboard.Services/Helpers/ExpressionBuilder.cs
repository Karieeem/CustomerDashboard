using CustomerDashboard.Models.Dtos;
using CustomerDashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDashboard.Services.Helpers
{
    public static class ExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        public static Expression<Func<T, bool>> GetFilterExpression<T>(IList<FilterModel> filters)
        {
            if (filters.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]);
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1]);
            //else
            //{
            //    while (filters.Count > 0)
            //    {
            //        var f1 = filters[0];
            //        var f2 = filters[1];

            //        if (exp == null)
            //            exp = GetExpression<T>(param, filters[0], filters[1]);
            //        else
            //            exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

            //        filters.Remove(f1);
            //        filters.Remove(f2);

            //        if (filters.Count == 1)
            //        {
            //            exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
            //            filters.RemoveAt(0);
            //        }
            //    }
            //}

            if (exp == null)
                return null;

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static bool IsCollectionProperty(Expression expression)
        {
            var type = expression.Type;
            return type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }

        private static Expression GetExpression<T>(ParameterExpression param, FilterModel filter)
        {
            Expression member = param;//Expression.Property(param, filter.PropertyName);

            var property = param.Type.GetProperty(filter.PropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null)
                return null;

            member = Expression.Property(param, filter.PropertyName);

            if (IsCollectionProperty(member))
                member = Expression.Call(typeof(Enumerable), "Count", new[] { member.Type.GetGenericArguments()[0] }, member);

            var propertyType = member.Type;
            var converter = TypeDescriptor.GetConverter(propertyType);

            if (!converter.CanConvertFrom(typeof(string)))
                throw new NotSupportedException();

            object propertyValue = null;
            if (filter.PropertyValue != null)
                propertyValue = converter.ConvertFromInvariantString(filter.PropertyValue.ToString());

            ConstantExpression constant = Expression.Constant(propertyValue);
            var valueExpression = Expression.Convert(constant, propertyType);


            switch (filter.Operator)
            {
                case Operator.Equals:
                    return Expression.Equal(member, valueExpression);

                case Operator.GreaterThan:
                    return Expression.GreaterThan(member, valueExpression);

                case Operator.LessThan:
                    return Expression.LessThan(member, valueExpression);


                case Operator.Contains:
                    if (valueExpression.Type.Name.ToLower() == "string")
                        return Expression.Call(member, containsMethod, valueExpression);
                    else
                        return null;
            }
            return null;
        }
        private static BinaryExpression GetExpression<T>(ParameterExpression param, FilterModel filter1, FilterModel filter2)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            if (filter1.NextLogicalOperator == LogicalOperator.And)
            {
                return Expression.AndAlso(bin1, bin2);
            }
            else
            {
                return Expression.OrElse(bin1, bin2);
            }
        }
    }

}
