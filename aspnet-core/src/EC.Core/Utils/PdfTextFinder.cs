using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using EC.Utils.Dto;

namespace EC.Utils
{
    public class PdfTextFinder
    {
        public static TextPosition FindAnchorPosition(byte[] pdfBytes, string anchorTag)
        {
            using (var reader = new PdfReader(pdfBytes))
            {
                var parser = new PdfReaderContentParser(reader);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    var listener = new TextLocationListener(page, anchorTag);
                    parser.ProcessContent(page, listener);
                    
                    listener.FindPositionFromFragments();
                    
                    if (listener.FoundPosition != null)
                    {
                        var pageSize = reader.GetPageSize(page);
                        listener.FoundPosition.PageHeight = pageSize.Height;
                        return listener.FoundPosition;
                    }
                }
            }
            return null;
        }

        public static List<string> ScanAnchorTags(string base64Str)
        {
            var pdfBytes = GetPdfBytes(base64Str);
            return ScanAnchorTags(pdfBytes);
        }

        public static List<string> ScanAnchorTags(byte[] pdfBytes)
        {
            var tags = new List<string>();
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return tags;
            }

            try
            {
                var anchorRegex = new System.Text.RegularExpressions.Regex(
                    @"<<[^<>]+>>",
                    System.Text.RegularExpressions.RegexOptions.None
                );

                using (var reader = new PdfReader(pdfBytes))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        string text = PdfTextExtractor.GetTextFromPage(reader, page, new SimpleTextExtractionStrategy());
                        if (!string.IsNullOrEmpty(text))
                        {
                            var matches = anchorRegex.Matches(text);
                            foreach (System.Text.RegularExpressions.Match match in matches)
                            {
                                string matchedText = match.Value;
                                if (!string.IsNullOrEmpty(matchedText) && !tags.Contains(matchedText))
                                {
                                    tags.Add(matchedText);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return tags;
        }

        private static byte[] GetPdfBytes(string base64Str)
        {
            if (string.IsNullOrEmpty(base64Str)) return Array.Empty<byte>();
            int commaIndex = base64Str.IndexOf(',');
            if (commaIndex >= 0)
            {
                base64Str = base64Str.Substring(commaIndex + 1);
            }
            return Convert.FromBase64String(base64Str);
        }
    }
}
