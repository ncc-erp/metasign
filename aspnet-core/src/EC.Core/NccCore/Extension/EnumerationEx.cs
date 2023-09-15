
using NccCore.Uitls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace NccCore.Extension
{
    public static class EnumerationEx
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.EmptyIfNull().Any();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> SeparateWith<T>(this IEnumerable<T> items, T separator)
        {
            var first = true;
            foreach (var item in items.EmptyIfNull())
            {
                if (first)
                    first = false;
                else
                    yield return separator;

                yield return item;
            }
        }

        public static Option<T> FirstOrNone<T>(this IEnumerable<T> items)
        {
            foreach (var item in items.EmptyIfNull())
            {
                return Option.Some(item);
            }

            return Option<T>.None;
        }

        public static IEnumerable Cast(this IEnumerable self, Type innerType)
        {
            var methodInfo = typeof(Enumerable).GetMethod("Cast");
            var genericMethod = methodInfo.MakeGenericMethod(innerType);
            return genericMethod.Invoke(null, new[] { self }) as IEnumerable;
        }

        public static IEnumerable CastFromJson(object o, Type innerType)
        {
            var list = o as ICollection<Newtonsoft.Json.Linq.JToken>;
            return list.Select(s => s.ToObject(innerType));
        }
    }
}
