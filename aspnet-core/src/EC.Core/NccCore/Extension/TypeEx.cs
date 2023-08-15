using System;
using System.Collections.Generic;
using System.Text;

namespace NccCore.Extension
{
    public static class TypeEx
    {
        public static bool IsAssignableTo(this Type type, Type destType)
        {
            return destType.IsAssignableFrom(type);
        }

        public static bool IsGenericOf(this Type tp, Type genericType)
        {
            return tp.IsGenericType && tp.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsProxyType(this Type tp)
        {
            return tp.Assembly.IsDynamic;
        }

        public static Type SkipProxyType(this Type tp)
        {
            if (tp.IsProxyType())
                return tp.BaseType;

            return tp;
        }

        public static object ChangeType(this Type type, object value)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = innerType.ChangeType(value);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }

        public static object ChangeType(System.Reflection.PropertyInfo property, object value)
        {
            var targetType = property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                ? Nullable.GetUnderlyingType(property.PropertyType)
                : property.PropertyType;
            return value == null ? null : Convert.ChangeType(value, targetType);
        }

        /// <summary>
        /// Get Object from Json object
        /// </summary>
        /// <param name="type">Type convert to</param>
        /// <param name="value">Json value</param>
        /// <returns></returns>
        public static object GetFromJson(this Type type, object value)
        {
            var json = value as Newtonsoft.Json.Linq.JToken;
            if (json != null)
                return json.ToObject(type);
            var data = type.ChangeType(value);
            return data;
        }
    }
}
