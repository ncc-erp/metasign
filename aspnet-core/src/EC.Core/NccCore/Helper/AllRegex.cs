using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NccCore.Helper
{
    public class AllRegex
    {
        public static Regex HashtagRegex = new Regex(@"(^|\s)(#\w?[^\s\@#$%^&*()=+.,\[{\]};:'><]+)");
        public static string RemoveHtmltags = @"/<[^>]*>/g";
        public static string Remove2Space = @"/\s{2,}/g";
        
    }
}
