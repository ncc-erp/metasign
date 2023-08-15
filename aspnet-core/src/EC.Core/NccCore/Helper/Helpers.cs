using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NccCore.Helper
{
    public class Helpers
    {
        public static T EnsureNotNull<T>(ref T field) where T : new()
        {
            if (field == null)
                field = new T();

            return field;
        }

        public static T Lock<T>(object lockObj, Func<T> f)
        {
            lock (lockObj) { return f(); };
        }

        public static T GetOrCreate<T>(ref T value, object lockObject, Func<T> factory) where T : class
        {
            if (value != null)
                return value;

            lock (lockObject)
            {
                if (value != null)
                    return value;

                value = factory();
                return value;
            }
        }

        public static T FailWith<T>(string message)
        {
            return FailWith<T>(new Exception(message));
        }
        public static T FailWith<T>(Exception exception)
        {
            throw exception;
        }

        /// <summary>
        /// Strips the HTML tags from the content. Also removes the extra spaces if any.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string StripHTMLContent(string content)
        {
            var pass1 = Regex.Replace(content, @"<[^>]+>|&nbsp;", " ");
           var pass2= Regex.Replace(pass1, @"\s{2,}", " ");
            return Regex.Replace(pass2, @"\n", " ");
        }
        public static string StripHtmlContentForHashtag(string content)
        {
            var a = content.Replace(AllRegex.RemoveHtmltags, " ");
                
             var b = a.Replace(AllRegex.Remove2Space," ");
            return b;
        }
    }
}
