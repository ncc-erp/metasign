using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NccCore.Extension
{
    public static class StringEx
    {
        public static int LengthEx(this string value)
        {
            return value == null ? 0 : value.Length;
        }
        public static bool IsNumeric(this string input)
        {
            decimal value;
            return decimal.TryParse(input, out value);
        }

        public static bool IsJson(this string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}") || input.StartsWith("[") && input.EndsWith("]");
        }

        public static bool IsInteger(this string input)
        {
            int value;
            return int.TryParse(input, out value);
        }

        public static string Append(this string input, string value)
        {
            return string.Concat(input, value);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static string Concatenate(this IEnumerable<string> strings, string separator)
        {
            return string.Concat(strings.SeparateWith(separator));
        }

        /// <summary>
		/// Get a substring of the first N characters.
		/// </summary>
		/// <example>
		/// Implementation: "cat".Truncate(1);
		/// Output: "c"
		/// </example>
        public static string Truncate(this string source, int length)
        {
            return source.Length <= length ? source : source.Substring(0, length);
        }

        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        /// <example>
        /// Implementation: "cat".Truncate(1);
        /// Output: "c"
        /// </example>
        public static string Elipsis(this string source, int length)
        {
            return source.EmptyIfNull().Length <= length ? source : source.Substring(0, length) + "...";
        }

        //from Newtonsoft.Json.Utilities.StringUtilities
        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                bool hasNext = (i + 1 < s.Length);
                if ((i == 0 || !hasNext) || char.IsUpper(s[i + 1]))
                {
                    char lowerCase;
#if !(NETFX_CORE || PORTABLE)
                    lowerCase = char.ToLower(s[i], System.Globalization.CultureInfo.InvariantCulture);
#else
                    lowerCase = char.ToLower(s[i]);
#endif

                    sb.Append(lowerCase);
                }
                else
                {
                    sb.Append(s.Substring(i));
                    break;
                }
            }

            return sb.ToString();
        }

        public static string EmptyIfNull(this string value)
        {
            return value ?? String.Empty;
        }

        public static string NullIfEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static string Alt(this string value, string altValue)
        {
            return value.HasValue() ? value : altValue;
        }

        public static string Alt(this string value, Func<string> altValue)
        {
            return value.HasValue() ? value : altValue();
        }

        public static bool HasValue(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }

        public static string ToUpperEx(this string value)
        {
            return value == null ? value : value.ToUpper();
        }

        public static string ToLowerEx(this string value)
        {
            return value == null ? null : value.ToLower();
        }

        /// <summary>
		/// Determines if the string is made up of alpha numeric characters only (Spaces are allowed)
		/// </summary>
		/// <param name="s">The string to test for alpha numeric only characters</param>
		/// <returns></returns>
		public static bool IsAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value)) return true;

            var r = new Regex("^(\\w| )+$");
            return r.IsMatch(value);
        }

        public static bool IsNumberCharsAt(this string value, int startIndex, int length)
        {
            int n;
            return value.LengthEx() > startIndex + length && int.TryParse(value, out n);
        }

        public static bool IsNumberCharAt(this string value, int index)
        {
            return value.LengthEx() > index && char.IsNumber(value[index]);
        }

        /// <summary>
		/// Determines if the string is made up of alpha numeric characters only (Spaces are allowed)
		/// </summary>
		/// <param name="s">The string to test for alpha numeric only characters</param>
		/// <returns></returns>
		public static bool IsValidFirstLastName(this string value)
        {
            if (string.IsNullOrEmpty(value)) return true;

            var r = new Regex("^(\\w|[- '\\(\\)])+$");
            return r.IsMatch(value);
        }

        /// <summary>
		/// Determines if the string is made up of ASCII only characters
		/// </summary>
		/// <param name="s">The string to test for ASCII only characters</param>
		/// <returns></returns>
		public static bool IsValidAscii(this string s)
        {
            for (var i = (s == null ? 0 : s.Length) - 1; i >= 0; i--)
            {
                var c = s[i];

                //(IS) have to consider \r and \n as they are part of the 'valid' input
                switch (c)
                {
                    case '\n':
                    case '\r':
                    case '\t':
                        continue;
                }

                var inValidRange = ((int)c).BetweenEx(32, 126);
                if (!inValidRange)
                    return false;
            }

            return true;
        }

        public static string Pascalize(this string input)
        {
            return Regex.Replace(input, "(?:^|_)(.)", match => match.Groups[1].Value.ToUpper());
        }

        /// <summary>
        /// Same as Pascalize except that the first character is lower case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Camelize(this string input)
        {
            var word = Pascalize(input);
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }
    }
}
