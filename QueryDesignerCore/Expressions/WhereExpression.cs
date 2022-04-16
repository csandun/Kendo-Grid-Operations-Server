using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryDesignerCore.Expressions
{
    /// <summary>
    /// Constructor expressions for Where methods.
    /// </summary>
    internal static class WhereExpression
    {
        /// <summary>
        /// String type.
        /// </summary>
        private static readonly Type StringType = typeof(string);


        /// <summary>
        /// Integer type.
        /// </summary>
        private static readonly Type IntType = typeof(int);

        /// <summary>
        /// Expression type.
        /// </summary>
        private static readonly Type ExpType = typeof(Expression);

        /// <summary>
        /// Queryable type.
        /// </summary>
        private static readonly Type QueryableType = typeof(Queryable);


        /// <summary>
        /// Binary AndAlso method for expression.
        /// </summary>
        private static readonly MethodInfo AndExpMethod = ExpType.GetRuntimeMethod("And", new[] { ExpType, ExpType });

        /// <summary>
        /// Binary OrElse method for expression.
        /// </summary>
        private static readonly MethodInfo OrExpMethod = ExpType.GetRuntimeMethod("Or", new[] { ExpType, ExpType });

        /// <summary>
        /// Info about "StartsWith" method.
        /// </summary>
        private static readonly MethodInfo StartsMethod = StringType.GetRuntimeMethod("StartsWith", new[] { StringType });

        /// <summary>
        /// Info about "Contains" method.
        /// </summary>
        private static readonly MethodInfo ContainsMethod = StringType.GetRuntimeMethod("Contains", new[] { StringType });

        /// <summary>
        /// Info about "EndsWith" method.
        /// </summary>
        private static readonly MethodInfo EndsMethod = StringType.GetRuntimeMethod("EndsWith", new[] { StringType });

        /// <summary>
        /// Info about AsQueryableMethod.
        /// </summary>
        private static readonly MethodInfo AsQueryableMethod = QueryableType.GetRuntimeMethods().FirstOrDefault(
                method => method.Name == "AsQueryable" && method.IsStatic);

        /// <summary>
        /// Info about "Any" method with one parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionAny = QueryableType.GetRuntimeMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 1);

        /// <summary>
        /// Info about "Any" method with two parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionAny2 = typeof(Queryable).GetRuntimeMethods().Single(
                method => method.Name == "Any" && method.IsStatic &&
                method.GetParameters().Length == 2);

        /// <summary>
        /// Info about "Any" method with one parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionCount = QueryableType.GetRuntimeMethods().Single(
                method => method.Name == "Count" &&
                method.GetParameters().Length == 1);

        /// <summary>
        /// Info about "Any" method with two parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionCount2 = QueryableType.GetRuntimeMethods().Single(
                method => method.Name == "Count" &&
                method.GetParameters().Length == 2);


        /// <summary>
        /// Info about "FirstOrDefault" method with one parameter for collection.
        /// </summary>
        private static readonly MethodInfo CollectionFirstOrDefault = QueryableType.GetRuntimeMethods().Single(
                method => method.Name == "FirstOrDefault" &&
                method.GetParameters().Length == 1);

        /// <summary>
        /// Info about "FirstOrDefault" method with two parameter for collection.
        /// </summary>
        //private static readonly MethodInfo CollectionFirstOrDefault2 = QueryableType.GetRuntimeMethods().Single(
        //        method => method.Name == "FirstOrDefault" &&
        //        method.GetParameters().Length == 2);




        /// <summary>
        /// Info about "Length" property.
        /// </summary>
        private static readonly PropertyInfo LengthProperty = StringType.GetProperty("Length");

        /// <summary>
        /// Info about method of constructing expressions.
        /// </summary>
        private static readonly MethodInfo ExpressionMethod = typeof(WhereExpression).GetRuntimeMethods().FirstOrDefault(m => m.Name == "GetExpression");

        /// <summary>
        /// Info about method of constructing expressions.
        /// </summary>
        private static readonly MethodInfo ExpressionTreeMethod = typeof(WhereExpression).GetRuntimeMethods().FirstOrDefault(m => m.Name == "GetTreeExpression");


        /// <summary>
        /// Info about avaliable methods for filter types.
        /// </summary>
        private static readonly Dictionary<string, List<MethodInfo>> FilterTypeAvaliableMethods = new Dictionary<string, List<MethodInfo>>()
        {
            {"any", new List<MethodInfo>(){CollectionAny,CollectionAny2 } },
            {"notany", new List<MethodInfo>(){CollectionAny,CollectionAny2 } },
            {"countequals", new List<MethodInfo>(){ CollectionCount, CollectionCount2 } },
            {"countgreaterthan", new List<MethodInfo>(){ CollectionCount, CollectionCount2 } },
            {"countgreaterthanorequal", new List<MethodInfo>(){ CollectionCount, CollectionCount2 } },
            {"countlessthan", new List<MethodInfo>(){ CollectionCount, CollectionCount2 } },
            //{WhereFilterType.CountLessThanOrEqual, new List<MethodInfo>(){ CollectionCount, CollectionCount2 } },
            //{WhereFilterType.FirstOrDefaultIsNull, new List<MethodInfo>(){ CollectionFirstOrDefault, CollectionFirstOrDefault2 } },
            //{WhereFilterType.FirstOrDefaultNotNull, new List<MethodInfo>(){ CollectionFirstOrDefault, CollectionFirstOrDefault2 } },
        };

        /// <summary>
        /// Available types for conversion.
        /// </summary>
        private static readonly Type[] AvailableCastTypes =
        {
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(bool),
            typeof(bool?),
            typeof(byte?),
            typeof(sbyte?),
            typeof(short),
            typeof(short?),
            typeof(ushort),
            typeof(ushort?),
            typeof(int),
            typeof(int?),
            typeof(uint),
            typeof(uint?),
            typeof(long),
            typeof(long?),
            typeof(ulong),
            typeof(ulong?),
            typeof(Guid),
            typeof(Guid?),
            typeof(double),
            typeof(double?),
            typeof(float),
            typeof(float?),
            typeof(decimal),
            typeof(decimal?),
            typeof(char),
            typeof(char?),
            typeof(string)
        };

        /// <summary>
        /// Get final expression for filter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="filter">Filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Final expression.</returns>
        public static Expression<Func<T, bool>> GetExpression<T>(this WhereFilter filter, string suffix = "")
        {
            var e = Expression.Parameter(typeof(T), "e" + suffix);


            var exs = GetExpressionForField(e, filter, suffix + "0");
            return Expression.Lambda<Func<T, bool>>(exs, e);
        }

        /// <summary>
        /// Get final expression for tree filter.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="filter">Tree filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Final expression.</returns>
        public static Expression<Func<T, bool>> GetTreeExpression<T>(this Filter filter, string suffix = "")
        {
            var e = Expression.Parameter(typeof(T), "e" + suffix);
            return Expression.Lambda<Func<T, bool>>(GetExpressionForTreeField(e, filter, suffix), e);
        }

        /// <summary>
        /// Construct expressions chain for tree filter.
        /// </summary>
        /// <param name="e">Parameter expression.</param>
        /// <param name="filter">Tree filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Expression chain.</returns>
        private static Expression GetExpressionForTreeField(Expression e, Filter filter, string suffix)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (filter.Logic == "none")
                return GetExpressionForField(e, filter, suffix + "0");


            var mi = filter.Logic == "and" ? AndExpMethod : OrExpMethod;
            if (!(filter.Filters?.Any() ?? false))
            {
                if (filter.OperandsOfCollections != null &&
                   (filter.OperandsOfCollections.Filters?.Any() ?? false))
                {
                    if (String.IsNullOrWhiteSpace(filter.Field))
                        return GetExpressionForTreeField(e, filter.OperandsOfCollections, suffix + 0);
                    else
                    {
                        var fieldExp = GetExpressionForField(e, new WhereFilter
                        {
                            Field = filter.Field,
                            Operator = filter.Operator,
                            Value = filter.Value
                        }, suffix + 0);
                        var expCollection = GetExpressionForTreeField(e, filter.OperandsOfCollections, suffix + 0);
                        return (BinaryExpression)mi.Invoke(null, new object[] { fieldExp, expCollection });
                    }

                }
                else
                {
                    throw new ArgumentException("Filter operands with operator type different from TreeFilterType.None cannot be empty.");
                }

            }

            var i = 0;
            var exp = String.IsNullOrWhiteSpace(filter.Field) ?
                GetExpressionForTreeField(e, filter.Filters[i], suffix + i) :
                GetExpressionForField(e, new WhereFilter
                {
                    Field = filter.Field,
                    Operator = filter.Operator,
                    Value = filter.Value
                }, suffix + i);


            // if field is  null or white space it starts from one because we already added zeroth element.
            if (String.IsNullOrWhiteSpace(filter.Field))
                i = 1;

            for (int z = i; z < filter.Filters.Count; z++)
            {
                var args = new object[] { exp, GetExpressionForTreeField(e, filter.Filters[z], suffix + z) };
                exp = (BinaryExpression)mi.Invoke(null, args);
            }

            if (filter.OperandsOfCollections != null &&
               (filter.OperandsOfCollections.Filters?.Any() ?? false))
            {
                exp = (BinaryExpression)mi.Invoke(null, new object[] { GetExpressionForTreeField(e, filter.OperandsOfCollections, suffix + 0) });
            }

            return exp;

        }

        /// <summary>
        /// Construct expressions chain between WhereFilters.
        /// </summary>
        /// <param name="e">Parameter expression.</param>
        /// <param name="filter">Tree filter for query.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Expression chain.</returns>
        private static Expression GetExpressionForField(Expression e, WhereFilter filter, string suffix)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (filter.Operator == "none" || string.IsNullOrWhiteSpace(filter.Field))
                throw new ArgumentException("Filter type cannot be None for single filter.");
            var s = filter.Field.Split('.');
            Expression prop = e;
            string prev = "";
            foreach (var t in s)
            {
                if (IsEnumerable(prop))
                {
                    prop = AsQueryable(prop);

                    var generic = ExpressionMethod.MakeGenericMethod(
                        prop.Type.GenericTypeArguments.Single());


                    string temp = String.Join(".", s);
                    int index = temp.IndexOf(prev);
                    string c = (index < 0)
                        ? temp
                        : temp.Remove(index,  prev.Length + 1);

                    object[] pars = {
                    new WhereFilter
                    {
                        Field = c,
                        Operator = filter.Operator,
                        Value = filter.Value
                    }, suffix
                    };
                    var expr = (Expression)generic.Invoke(null, pars);

                    var call = Expression.Call(
                        CollectionAny2.MakeGenericMethod(
                        prop.Type.GenericTypeArguments.First()),
                        prop,
                        expr);

                    return call;

                }
                prop = Expression.Property(prop, GetDeclaringProperty(prop, t));
                prev = t;
            }
            var exp = GenerateExpressionOneField(prop, filter);
            return exp;
        }


        /// <summary>
        /// Construct bool-expression between different expression and a filter.
        /// </summary>
        /// <param name="prop">Different expression.</param>
        /// <param name="filter">Filter for query.</param>
        /// <returns>Expression with filter.</returns>
        private static Expression GenerateExpressionOneField(Expression prop, WhereFilter filter)
        {
            switch (filter.Operator)
            {
                case "eq":
                    return Expression.Equal(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case "neq":
                    return Expression.NotEqual(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case "lt":
                    return Expression.LessThan(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case "gt":
                    return Expression.GreaterThan(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case "lte":
                    return Expression.LessThanOrEqual(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case "gte":
                    return Expression.GreaterThanOrEqual(
                        prop,
                        ToStaticParameterExpressionOfType(TryCastFieldValueType(filter.Value, prop.Type), prop.Type));

                case "startswith":
                    return Expression.Call(prop, StartsMethod, Expression.Constant(filter.Value.ToString(), StringType));

                case "nostartswith":
                    return Expression.Not(
                        Expression.Call(prop, StartsMethod, Expression.Constant(filter.Value.ToString(), StringType)));

                case "contains":
                    return Expression.Call(prop, ContainsMethod, Expression.Constant(filter.Value.ToString(), StringType));

                case "doesnotcontain":
                    return Expression.Not(
                        Expression.Call(prop, ContainsMethod, Expression.Constant(filter.Value.ToString(), StringType)));

                case "endswith":
                    return Expression.Call(prop, EndsMethod, Expression.Constant(filter.Value.ToString(), StringType));

                case "doesnotendwith":
                    return Expression.Not(
                        Expression.Call(prop, EndsMethod, Expression.Constant(filter.Value.ToString(), StringType)));

                case "any":
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);

                    return CreateMethodCall(prop, filter);

                case "doesnotany":
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    return Expression.Not(CreateMethodCall(prop, filter));

                case "isnull":
                    return Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type));

                case "isnotnull":
                    return Expression.Not(
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type)));

                case "isempty":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type));

                case "isnotempty":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.Not(
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type)));

                case "isnullorempty":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.OrElse(
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type)),
                        Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type)));

                case "isnotnullorempty":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.Not(
                        Expression.OrElse(
                            Expression.Equal(prop, ToStaticParameterExpressionOfType(null, prop.Type)),
                            Expression.Equal(prop, ToStaticParameterExpressionOfType(string.Empty, prop.Type))));


                // collections 

                case "countequals": //CountEquals
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);

                    return Expression.Equal(CreateMethodCall(prop, filter), Expression.Constant(filter.Value, IntType));

                case "countgreaterthan": //CountGreaterThan
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);

                    return Expression.GreaterThan(CreateMethodCall(prop, filter), Expression.Constant(filter.Value, IntType));

                case "countgreaterthanorequal":
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    return Expression.GreaterThanOrEqual(CreateMethodCall(prop, filter), Expression.Constant(filter.Value, IntType));

                case "countlessthan":
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    return Expression.LessThan(CreateMethodCall(prop, filter), Expression.Constant(filter.Value, IntType));

                case "countlessthanorequal":
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    return Expression.LessThanOrEqual(CreateMethodCall(prop, filter), Expression.Constant(filter.Value, IntType));

                case "lengthequals":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.Equal(Expression.Property(prop, LengthProperty), Expression.Constant(filter.Value, IntType));

                case "lengthgreaterthan":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.GreaterThan(Expression.Property(prop, LengthProperty), Expression.Constant(filter.Value, IntType));

                case "lengthgreaterthanOrequal":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.GreaterThanOrEqual(Expression.Property(prop, LengthProperty), Expression.Constant(filter.Value, IntType));

                case "lengthlessthan":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.LessThan(Expression.Property(prop, LengthProperty), Expression.Constant(filter.Value, IntType));

                case "lengthlessthanorequal":
                    if (prop.Type != typeof(string))
                        throw new InvalidCastException($"{filter.Operator} can be applied to String type only");
                    return Expression.LessThanOrEqual(Expression.Property(prop, LengthProperty), Expression.Constant(filter.Value, IntType));

                case "firstordefaultisnull":
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    return Expression.Equal(CreateMethodCall(prop, filter), ToStaticParameterExpressionOfType(null, prop.Type));

                case "firstordefaultnotnull":
                    if (IsEnumerable(prop))
                        prop = AsQueryable(prop);
                    return Expression.Not(Expression.Equal(CreateMethodCall(prop, filter), ToStaticParameterExpressionOfType(null, prop.Type)));

                default:
                    return prop;
            }
        }

        /// <summary>
        /// Create method call with different expression
        /// </summary>
        /// <param name="prop">Different expression.</param>
        /// <param name="filter">Filter for query.</param>
        /// <returns>Method call expression.</returns>
        private static MethodCallExpression CreateMethodCall(Expression prop, WhereFilter filter)
        {
            var methods = FilterTypeAvaliableMethods[filter.Operator];


            var mf = methods[0].MakeGenericMethod(
                     prop.Type.GenericTypeArguments.First());

            var mf2 = methods[1].MakeGenericMethod(
                       prop.Type.GenericTypeArguments.First());


            return filter.OperandsOfCollections == null ?
                    Expression.Call(mf, prop) :
                    Expression.Call(mf2, prop, CreateCollectionMethodExpression(filter.OperandsOfCollections, prop.Type.GenericTypeArguments.First()));

        }


        /// <summary>
        /// Create expression for collection methods
        /// </summary>
        /// <param name="filter">Filter value.</param>
        /// <param name="type">Conversion to type.</param>
        /// <param name="suffix">Suffix vor variable.</param>
        /// <returns>Converted value.</returns>
        private static Expression CreateCollectionMethodExpression(Filter filter, Type type, string suffix = "")
        {

            var generic = ExpressionTreeMethod.MakeGenericMethod(type);

            object[] pars = {
                     filter
                     ,suffix+"0"
                    };
            var expr = (Expression)generic.Invoke(null, pars);

            return expr;

        }


        /// <summary>
        /// Value type filter field conversion.
        /// </summary>
        /// <param name="value">Filter value.</param>
        /// <param name="type">Conversion to type.</param>
        /// <returns>Converted value.</returns>
        private static object TryCastFieldValueType(object value, Type type)
        {
            if (value == null || (!AvailableCastTypes.Contains(type) && !type.GetTypeInfo().IsEnum))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}.");

            var valueType = value.GetType();

            if (valueType == type)
                return value;

            if (type.GetTypeInfo().BaseType == typeof(Enum))
                return Enum.Parse(type, Convert.ToString(value));


            var s = Convert.ToString(value);
            object res;

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GenericTypeArguments[0];
                res = Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(type));
            }
            else if (type == typeof(string))
            {
                return value.ToString();
            }
            else
            {
                res = Activator.CreateInstance(type);
            }

            var argTypes = new[] { StringType, type.MakeByRefType() };
            object[] args = { s, res };
            var tryParse = type.GetRuntimeMethod("TryParse", argTypes);

            if (!(bool)(tryParse?.Invoke(null, args) ?? false))
                throw new InvalidCastException($"Cannot convert value to type {type.Name}.");

            return args[1];
        }

        /// <summary>
        /// Creates parameters expression of static value.
        /// </summary>
        /// <returns>The static parameter expression of type.</returns>
        /// <param name="obj">Filter value.</param>
        /// <param name="type">Type of value.</param>
        private static Expression ToStaticParameterExpressionOfType(object obj, Type type)
            => Expression.Convert(
                Expression.Property(
                    Expression.Constant(new { obj }),
                    "obj"),
                type);

        /// <summary>
        /// Cast IEnumerable to IQueryable.
        /// </summary>
        /// <param name="prop">IEnumerable expression</param>
        /// <returns>IQueryable expression.</returns>
        private static Expression AsQueryable(Expression prop)
        {
            return Expression.Call(
                        AsQueryableMethod.MakeGenericMethod(prop.Type.GenericTypeArguments.Single()),
                        prop);
        }


        /// <summary>
        /// Expression type is IEnumerable.
        /// </summary>
        /// <param name="prop">Verifiable expression.</param>
        /// <returns>Result of checking.</returns>
        private static bool IsEnumerable(Expression prop)
        {
            return prop.Type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name == "IEnumerable") != null;
        }

        /// <summary>
        /// Get property from class in which it is declared.
        /// </summary>
        /// <param name="e">Parameter expression.</param>
        /// <param name="name">Name of property.</param>
        /// <returns>Property info.</returns>
        private static PropertyInfo GetDeclaringProperty(Expression e, string name)
        {
            var t = e.Type;
            var p = t.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (p == null)
            {
                throw new InvalidOperationException(string.Format("Property '{0}' not found on type '{1}'", name, t));
            }

            if (t != p.DeclaringType)
            {
                p = p.DeclaringType.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            return p;
        }
    }
}
