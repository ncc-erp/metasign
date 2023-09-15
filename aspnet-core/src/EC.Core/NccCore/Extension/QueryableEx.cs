using Abp.ObjectMapping;
using NccCore.Anotations;
using NccCore.DynamicFilter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NccCore.Paging;
using NccCore.Extension;

namespace NccCore.Extension
{
    public static class QueryableEx
    {
        public static IQueryable<T> TakePage<T>(this IQueryable<T> query, GridParam gridParam) where T : class
        {
            var newQuery = query;
            if (!string.IsNullOrEmpty(gridParam.Sort))
            {
                var sortProperty = gridParam.Sort.Pascalize();
                newQuery = OrderingHelper(newQuery, sortProperty, gridParam.SortDirection == SortDirection.DESC, false);
            }
            return newQuery.Skip(gridParam.SkipCount).Take(gridParam.MaxResultCount);
        }

        public static async Task<GridResult<T>> GetGridResult<T, Y>(this IQueryable<T> qFullQuery, IQueryable<Y> qTotalQuery, GridParam param) where T : class
        {
            var isSearchAndFilter = (param.FilterItems != null && param.FilterItems.Count > 0) || !string.IsNullOrEmpty(param.SearchText);
            var query = qFullQuery.ApplySearchAndFilter(param);
            var list = await query.TakePage(param).ToListAsync();
            var total = await query.CountAsync();
            var isSearch = isSearchAndFilter && total == 0;
            return new GridResult<T>(list, total, isSearch);
        }


        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property = Expression.PropertyOrField(param, propertyName);
            LambdaExpression sort = Expression.Lambda(property, param);
            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));
            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, false);
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, false);
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, true);
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, true);
        }

        public static IOrderedQueryable<TItem> OrderBy<TItem, TKey>(this IQueryable<TItem> query, bool asc, Expression<Func<TItem, TKey>> orderBy)
        {
            return asc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        public static IQueryable<T> ApplySearchAndFilter<T>(this IQueryable<T> query, GridParam gridParam)
        {
            var searchTerm = gridParam.SearchText.EmptyIfNull().Trim().ToLower();
            var newQuery = query;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchFilter = typeof(T).GetAllProperties().Where(s => s.GetCustomAttributes(typeof(ApplySearchAttribute), true).Any()).Select(s => new ExpressionFilter
                {
                    PropertyName = s.Name,
                    Value = searchTerm,
                    PropertyType = s.PropertyType,
                    Comparision = ComparisionOperator.Contains
                }).ToList();
                
                var searchExp = ExpressionEx.CombineExpressions<T>(searchFilter, false);
                if (searchExp != null)
                {
                    newQuery = newQuery.Where(searchExp) as IQueryable<T>;
                }
            }
            //var orExpression = ExpressionEx.CombineExpressions<T>(gridParam.SearchItems, false);
            var andExpression = ExpressionEx.CombineExpressions<T>(gridParam.FilterItems, true);

            
            //if (orExpression != null)
            //{
            //    newQuery = newQuery.Where(orExpression) as IQueryable<T>;
            //}
            if (andExpression != null)
            {
                newQuery = newQuery.Where(andExpression) as IQueryable<T>;
            }
            return newQuery;
        }

        public static int Count(this IQueryable query)
        {
            return query.Provider.Execute<int>(Expression.Call(typeof(System.Linq.Queryable), "Count", new[] { query.ElementType }, query.Expression));
        }
        public static IQueryable Transform(this IQueryable query, MethodCallExpression f)
        {
            return f == null ? query : query.Provider.CreateQuery(f);
        }
        public static MethodCallExpression CallQueryable(this IQueryable query, string queryableMethodName, Type[] methodTypeArguments, params Expression[] args)
        {
            return Expression.Call(typeof(Queryable), queryableMethodName, methodTypeArguments, new[] { query.Expression }.Concat(args).ToArray());
        }
        public static MethodCallExpression CallQueryable(this IQueryable query, string queryableMethodName, Expression queryableExprArg = null, Type methodTypeArgument = null)
        {
            return query.CallQueryable(queryableMethodName, new[] { methodTypeArgument ?? query.ElementType }, new[] { queryableExprArg }.Where(a => a != null).ToArray());
        }
        public static IQueryable Where(this IQueryable items, Func<ParameterExpression, Expression> predicate)
        {
            var res = items.Transform(items.CallQueryable("Where", Expression.Quote(ExpressionEx.Predicate(items.ElementType, predicate))));
            return res;
        }
        public static IQueryable TakeUntyped(this IQueryable items, int count)
        {
            var res = items.Transform(items.CallQueryable("Take", count.AsConstantExpression()));
            return res;
        }
        public static IQueryable SkipUntyped(this IQueryable items, int count)
        {
            var res = items.Transform(items.CallQueryable("Skip", count.AsConstantExpression()));
            return res;
        }
        public static IQueryable DistinctUntyped(this IQueryable items)
        {
            var res = items.Transform(items.CallQueryable("Distinct"));
            return res;
        }

        public static bool Any(this IQueryable query)
        {
            var res = query.Provider.Execute<bool>(Expression.Call(typeof(System.Linq.Queryable), "Any", new[] { query.ElementType }, query.Expression));
            return res;
        }

        public static IQueryable GroupJoin(this IQueryable outer, IQueryable inner, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector)
        {
            var touter = outer.ElementType;
            var tinner = inner.ElementType;
            var tkey = outerKeySelector.ReturnType;

            var resultSelectorExpression = ExpressionEx.Func(
                touter.AsParameter(),
                typeof(IEnumerable<>).MakeGenericType(tinner).AsParameter(),
                (element, joinedElements) => GenericGroupJoinItem.CreateNewExpression(element, joinedElements)
            );

            return outer.Transform(outer.CallQueryable("GroupJoin", new Type[] { touter, tinner, tkey, resultSelectorExpression.ReturnType }, inner.Expression, outerKeySelector.Quote(), innerKeySelector.Quote(), resultSelectorExpression.Quote()));
        }
        public static IQueryable Join(this IQueryable outer, IQueryable inner, Func<ParameterExpression, Expression> outerKeySelector, Func<ParameterExpression, Expression> innerKeySelector, Func<ParameterExpression, ParameterExpression, Expression> resultSelector = null)
        {
            var touter = outer.ElementType;
            var tinner = inner.ElementType;
            return outer.Join(inner, ExpressionEx.Func(touter.AsParameter(), outerKeySelector), ExpressionEx.Func(tinner.AsParameter(), innerKeySelector), resultSelector);
        }
        public static IQueryable Join(this IQueryable outer, IQueryable inner, LambdaExpression outerKeySelectorExpr, LambdaExpression innerKeySelectorExpr, Func<ParameterExpression, ParameterExpression, Expression> resultSelector = null)
        {
            var touter = outer.ElementType;
            var tinner = inner.ElementType;

            var tkey = outerKeySelectorExpr.ReturnType;

            if (resultSelector == null)
                resultSelector = (left, right) => GenericJoinItem.CreateNewExpression(left, right);

            var resultSelectorExpr = ExpressionEx.Func(touter.AsParameter(), tinner.AsParameter(), resultSelector);

            return outer.Transform(outer.CallQueryable("Join", new Type[] { touter, tinner, tkey, resultSelectorExpr.ReturnType }, inner.Expression, outerKeySelectorExpr.Quote(), innerKeySelectorExpr.Quote(), resultSelectorExpr.Quote()));
        }
        static Type AsEnumerableType(this Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }
        public static IQueryable GroupJoin(this IQueryable outer, IQueryable inner, Func<ParameterExpression, Expression> outerKeySelector, Func<ParameterExpression, Expression> innerKeySelector, Func<ParameterExpression, ParameterExpression, Expression> resultSelector)
        {
            var touter = outer.ElementType;
            var tinner = inner.ElementType;

            var outerKeySelectorExpr = ExpressionEx.Func(touter.AsParameter(), outerKeySelector);
            var innerKeySelectorExpr = ExpressionEx.Func(tinner.AsParameter(), innerKeySelector);
            var resultSelectorExpr = ExpressionEx.Func(touter.AsParameter(), tinner.AsEnumerableType().AsParameter(), resultSelector);

            var tkey = outerKeySelectorExpr.ReturnType;

            return outer.Transform(outer.CallQueryable("GroupJoin", new Type[] { touter, tinner, tkey, resultSelectorExpr.ReturnType }, inner.Expression, outerKeySelectorExpr.Quote(), innerKeySelectorExpr.Quote(), resultSelectorExpr.Quote()));
        }
        public static IQueryable Select(this IQueryable items, Func<ParameterExpression, Expression> projection)
        {
            var projectionLambda = ExpressionEx.Converter(items.ElementType, projection);
            var queryableCallExpression = items.CallQueryable("Select", new[] { items.ElementType, projectionLambda.ReturnType }, projectionLambda);
            var projectedQuery = items.Transform(queryableCallExpression);
            return projectedQuery;
        }
        public static IQueryable SelectMany(this IQueryable items, Func<ParameterExpression, Expression> projection)
        {
            var projectionLambda = ExpressionEx.Converter(items.ElementType, projection);
            var queryableCallExpression = items.CallQueryable("SelectMany", new[] { items.ElementType, projectionLambda.ReturnType.GetEnumerableItemType() }, projectionLambda);
            var projectedQuery = items.Transform(queryableCallExpression);
            return projectedQuery;
        }
        public static IQueryable SelectProjection(this IQueryable items, LambdaExpression projection)
        {
            var queryableCallExpression = items.CallQueryable("Select", new[] { items.ElementType, projection.ReturnType }, projection);
            var projectedQuery = items.Transform(queryableCallExpression);
            return projectedQuery;
        }
        public static IQueryable Where(this IQueryable items, LambdaExpression predicate)
        {
            var res = items.Transform(items.CallQueryable("Where", Expression.Quote(predicate)));
            return res;
        }
        public static object FirstOrDefault(this IQueryable items)
        {
            return typeof(Queryable).Call("FirstOrDefault", new[] { items.ElementType }, items.AsConstantExpression()).AsLambda().Compile().DynamicInvoke();
        }

        public static IQueryable OfType(this IQueryable queryable, Type type)
        {
            return queryable.Transform(queryable.ElementType == type ? null : queryable.CallQueryable("OfType", new[] { type }));
        }
        public static IEnumerable<T> LoadAndCastAs<T>(this IQueryable query)
        {
            return ((System.Collections.IEnumerable)query).Cast<T>().ToList();
        }

        #region ContainsAllWords helpers
        const Expression nullExpression = null;
        static Expression Combine(this Expression left, Expression right, Func<Expression, Expression, Expression> op) { return left == null ? right : op(left, right); }
        static Expression Combine(this IEnumerable<Expression> expressions, Func<Expression, Expression, Expression> op) { return expressions.Where(a => a != null).Aggregate(nullExpression, (left, right) => left.Combine(right, op)); }
        static Func<ParameterExpression[], Expression> GenericLambda(Func<ParameterExpression[], Expression> f) { return f; }
        static LambdaExpression Lambda(this Func<ParameterExpression[], Expression> body, params Type[] parameterTypes)
        {
            var args = parameterTypes.EmptyIfNull().Select(t => Expression.Parameter(t)).ToArray();
            var b = body(args);
            return b == null ? null : Expression.Lambda(b, args);
        }
        static LambdaExpression Lambda(this Func<ParameterExpression, Expression> body, Type argType1) { return GenericLambda(args => body(args[0])).Lambda(argType1); }
        static LambdaExpression Lambda(this Func<ParameterExpression, ParameterExpression, Expression> body, Type argType1, Type argType2) { return GenericLambda(args => body(args[0], args[1])).Lambda(argType1, argType2); }
        #endregion

        public static IOrderedQueryable OrderBy(this IQueryable source, LambdaExpression keySelector, bool firstKey, bool descending)
        {
            var methodName = firstKey ? "OrderBy" : "ThenBy";
            if (descending)
                methodName += "Descending";

            var expression = source.CallQueryable(methodName, new[] { source.ElementType, keySelector.ReturnType }, keySelector);
            var newQuery = source.Transform(expression);
            return (IOrderedQueryable)newQuery;
        }

        public static IQueryable OrderBy(this IQueryable source, IEnumerable<OrderByKeySelector> keys, bool orderByHasntBeenAppliedYet)
        {
            var res =
                keys
                .EmptyIfNull()
                .Aggregate(
                    new { query = source, firstKey = orderByHasntBeenAppliedYet },
                    (acc, sortBy) => new
                    {
                        query = (IQueryable)acc.query.OrderBy(sortBy.KeySelector, acc.firstKey, sortBy.Descending),
                        firstKey = false
                    }
                )
                .query;

            return res;
        }

        public static IQueryable<T> DistinctBy<T, K>(this IQueryable<T> query, Expression<Func<T, K>> selector)
        {
            return (IQueryable<T>)query.GroupBy(selector).Select(e => e.FirstOrDefault());
        }
    }

    public enum PropertyToSearchMethod
    {
        StartsWith,
        Contains
    }

    public class PropertyToSearch
    {
        internal PropertyToSearch(LambdaExpression property, PropertyToSearchMethod? method = null)
        {
            this.Property = property;
            this.Method = method;
        }
        public LambdaExpression Property { get; private set; }
        public PropertyToSearchMethod? Method { get; private set; }
    }

    public class PropertyToSearch<TEntity, TProperty> : PropertyToSearch
    {
        internal PropertyToSearch(Expression<Func<TEntity, TProperty>> property, PropertyToSearchMethod? method = null) : base(property, method) { }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
        public static implicit operator PropertyToSearch<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            return new PropertyToSearch<TEntity, TProperty>(property);
        }
    }

    public class OrderByKeySelector
    {
        public LambdaExpression KeySelector { get; set; }
        public bool Descending { get; set; }
    }

    public static class GenericGroupJoinItem
    {
        public static MemberInitExpression CreateNewExpression(Expression element, Expression joinedElements)
        {
            var joinedElementsItemType = joinedElements.Type.GetEnumerableItemType();
            var genericGroupJoinItemType = typeof(GenericGroupJoinItem<,>).MakeGenericType(element.Type, joinedElementsItemType);
            var constr = genericGroupJoinItemType.GetConstructor(new Type[] { });
            return Expression.MemberInit(Expression.New(constr), Expression.Bind(genericGroupJoinItemType.GetProperty("Element"), element), Expression.Bind(genericGroupJoinItemType.GetProperty("JoinedElements"), joinedElements));
        }
    }

    public static class GenericGroupJoinElement
    {
        public static MemberInitExpression CreateNewExpression(Expression left, Expression right)
        {
            var genericGroupJoinElementType = typeof(GenericGroupJoinElement<,>).MakeGenericType(left.Type, right.Type);
            var constr = genericGroupJoinElementType.GetConstructor(new Type[] { });
            return Expression.MemberInit(
                Expression.New(constr),
                Expression.Bind(genericGroupJoinElementType.GetProperty("Left"), left),
                Expression.Bind(genericGroupJoinElementType.GetProperty("Right"), right)
            );
        }
    }

    public class GenericGroupJoinElement<TLeft, TRight>
    {
        public GenericGroupJoinElement() { }
        public GenericGroupJoinElement(TLeft left, TRight right)
        {
            this.Left = left;
            this.Right = right;
        }
        public TLeft Left { get; set; }
        public TRight Right { get; set; }
    }

    public class GenericGroupJoinItem<TElement, TJoinedElement> : GenericGroupJoinElement<TElement, IEnumerable<TJoinedElement>>
    {
        public GenericGroupJoinItem() { }
        public GenericGroupJoinItem(TElement element, IEnumerable<TJoinedElement> joinedElements) : base(element, joinedElements) { }
        public TElement Element { get { return this.Left; } set { this.Left = value; } }
        public IEnumerable<TJoinedElement> JoinedElements { get { return this.Right; } set { this.Right = value; } }
    }

    public class GenericJoinItem<TLeft, TRight> : GenericGroupJoinElement<TLeft, TRight>
    {
        public GenericJoinItem() { }
        public GenericJoinItem(TLeft left, TRight right) : base(left, right) { }
    }

    public static class GenericJoinItem
    {
        public static MemberInitExpression CreateNewExpression(Expression left, Expression right)
        {
            //var rightType = right.Type;
            var genericJoinItemType = typeof(GenericJoinItem<,>).MakeGenericType(left.Type, right.Type);
            var constr = genericJoinItemType.GetConstructor(new Type[] { });
            return Expression.MemberInit(Expression.New(constr), Expression.Bind(genericJoinItemType.GetProperty("Left"), left), Expression.Bind(genericJoinItemType.GetProperty("Right"), right));
        }
    }
}
