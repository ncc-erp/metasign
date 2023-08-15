using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NccCore.Extension
{
    public static class MemberInfoEx
    {
        public static IEnumerable<T> OfExactType<T>(this IEnumerable<T> attributes)
            where T : Attribute
        {
            return attributes.Where(a => a.GetType() == typeof(T));
        }

        public static object GetValue(this MemberInfo m, object obj, bool memberCanBeStatic = false)
        {
            var fi = m as FieldInfo;
            if (fi != null)
                memberCanBeStatic = fi.IsStatic;

            if (m == null || (!memberCanBeStatic && obj == null))
                return null;

            if (fi != null)
                return fi.GetValue(obj);

            {
                var vg = m as PropertyInfo;
                if (vg != null)
                    return vg.GetValue(obj, null);
            }

            return null;
        }

        public static void SetValue(this MemberInfo m, object instance, object value)
        {
            if (instance == null)
                return;

            {
                var vg = m as PropertyInfo;
                if (vg != null)
                    vg.SetValue(instance, value, null);
            }

            {
                var vg = m as FieldInfo;
                if (vg != null)
                    vg.SetValue(instance, value);
            }
        }

        public static Type Type(this MemberInfo m)
        {
            {
                var vg = m as PropertyInfo;
                if (vg != null)
                    return vg.PropertyType;
            }

            {
                var vg = m as FieldInfo;
                if (vg != null)
                    return vg.FieldType;
            }

            return null;
        }

        public static bool IsReadonly(this MemberInfo m)
        {
            {
                var vg = m as PropertyInfo;
                if (vg != null)
                    return !vg.CanWrite;
            }

            {
                var vg = m as FieldInfo;
                if (vg != null)
                    return vg.IsInitOnly;
            }

            return true;
        }

        public static PropertyInfo GetPropertyEx(this Type type, string propertyName)
        {
            return type.GetProperties().Where(p => p.Name == propertyName).OrderBy(p => p.DeclaringType == type ? 0 : 1).First();
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return type.GetProperties();
        }

        public static bool DoesTypeSupportInterface(this Type type, Type inter)
        {
            if (inter.IsAssignableFrom(type))
                return true;
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == inter))
                return true;
            return false;
        }

        public static string GetPropertyName<TType, TDataType>(this TType type, System.Linq.Expressions.Expression<Func<TType, TDataType>> propertyNameGetter)
        {
            var property = propertyNameGetter.Body as System.Linq.Expressions.MemberExpression;
            if (property == null)
            {
                throw new ArgumentException("Invalid expression format");
            }

            return property.Member.Name;
        }

        public static bool IsAsync(this MethodInfo methodInfo)
        {
            Type asyncAttrType = typeof(AsyncStateMachineAttribute);
            return methodInfo.GetCustomAttribute(asyncAttrType) != null;
        }
    }
}
