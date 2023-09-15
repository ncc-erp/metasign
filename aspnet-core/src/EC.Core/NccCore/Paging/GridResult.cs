using System;
using System.Collections.Generic;
using System.Text;

namespace NccCore.Paging
{
    public class GridResult<T> where T : class
    {
        public int TotalCount { get; set; }
        public IReadOnlyList<T> Items { get; set; }
        public bool IsSearch { get; set; }

        public GridResult(IReadOnlyList<T> items, int total, bool isSearch)
        {
            Items = items;
            TotalCount = total;
            IsSearch = isSearch;
        }
    }
}
