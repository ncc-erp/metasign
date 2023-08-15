using System;
using System.Collections.Generic;
using System.Text;
using NccCore.Uitls;

namespace NccCore.Extension
{
    public static class DictionaryEx
    {
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultGetter)
        {
            return dictionary.FindValue(key).GetValue(() => defaultGetter(key));
        }

        public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            {
                var res = dictionary.FindValue(key);
                if (res.IsSome)
                    return res.Value;
            }

            {
                var res = valueFactory();
                dictionary[key] = res;
                return res;
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.FindValue(key).GetValueOrDefault();
        }

        public static Option<TValue> FindValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                return Option<TValue>.None;

            TValue value;
            if (!dictionary.TryGetValue(key, out value))
                return Option<TValue>.None;
            else
                return Option.Some(value);
        }

        public static bool ContainsKeyEx<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return key == null ? false : dict.ContainsKey(key);
        }

        public static bool AddIfMissing<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, value);
            return true;
        }
    }
}
