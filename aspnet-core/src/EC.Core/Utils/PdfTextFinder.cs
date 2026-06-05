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
    }
}
