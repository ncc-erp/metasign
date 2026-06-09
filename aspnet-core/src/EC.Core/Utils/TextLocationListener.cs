using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using EC.Utils.Dto;

namespace EC.Utils
{
    public class TextLocationListener : IRenderListener
    {
        private readonly int _page;
        private readonly string _anchorTag;
        public TextPosition FoundPosition { get; private set; }

        private readonly List<TextFragment> _fragments = new List<TextFragment>();

        public class TextFragment
        {
            public string Text { get; set; }
            public float MinX { get; set; }
            public float MaxX { get; set; }
            public float Y { get; set; }
            public float Height { get; set; }
        }

        public TextLocationListener(int page, string anchorTag)
        {
            _page = page;
            _anchorTag = anchorTag;
        }

        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderImage(ImageRenderInfo renderInfo) { }

        public void RenderText(TextRenderInfo renderInfo)
        {
            string text = renderInfo.GetText();
            if (string.IsNullOrEmpty(text)) return;

            var baseline = renderInfo.GetBaseline();
            var startPoint = baseline.GetStartPoint();
            var endPoint = baseline.GetEndPoint();

            float minX = Math.Min(startPoint[0], endPoint[0]);
            float maxX = Math.Max(startPoint[0], endPoint[0]);
            float y = startPoint[1];

            float height = 10f;
            try
            {
                var ascent = renderInfo.GetAscentLine().GetStartPoint();
                var descent = renderInfo.GetDescentLine().GetStartPoint();
                height = Math.Abs(ascent[1] - descent[1]);
            }
            catch {}

            if (height <= 0) height = 10f;

            _fragments.Add(new TextFragment
            {
                Text = text,
                MinX = minX,
                MaxX = maxX,
                Y = y,
                Height = height
            });
        }

        public void FindPositionFromFragments()
        {
            if (!_fragments.Any())
            {
                return;
            }

            string target = _anchorTag.ToLowerInvariant().Replace(" ", "");

            var sorted = _fragments.OrderByDescending(f => f.Y).ToList();

            var lines = new List<List<TextFragment>>();
            foreach (var fragment in sorted)
            {
                var line = lines.FirstOrDefault(l => Math.Abs(l[0].Y - fragment.Y) < 3.0f);
                if (line == null)
                {
                    line = new List<TextFragment>();
                    lines.Add(line);
                }
                line.Add(fragment);
            }

            foreach (var line in lines)
            {
                var orderedLine = line.OrderBy(f => f.MinX).ToList();

                var normalizedLineText = "";
                var charIndexToFragment = new List<int>();

                for (int i = 0; i < orderedLine.Count; i++)
                {
                    var f = orderedLine[i];
                    string normalizedFragmentText = f.Text.ToLowerInvariant().Replace(" ", "");
                    normalizedLineText += normalizedFragmentText;
                    for (int j = 0; j < normalizedFragmentText.Length; j++)
                    {
                        charIndexToFragment.Add(i);
                    }
                }

                int matchIndex = normalizedLineText.IndexOf(target);
                if (matchIndex >= 0)
                {
                    int startFragIdx = charIndexToFragment[matchIndex];
                    int endFragIdx = charIndexToFragment[matchIndex + target.Length - 1];

                    var startFrag = orderedLine[startFragIdx];
                    var endFrag = orderedLine[endFragIdx];

                    float startX = startFrag.MinX;
                    float endX = endFrag.MaxX;

                    FoundPosition = new TextPosition
                    {
                        Page = _page,
                        X = startX,
                        Y = startFrag.Y,
                        Width = Math.Abs(endX - startX),
                        Height = startFrag.Height,
                        PageHeight = 0 
                    };

                    return;
                }
            }
        }
    }
}
