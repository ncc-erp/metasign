using System.Collections.Generic;

namespace NccCore.Paging
{
    public class PagingResult<T>
    {
        public long TotalItems { get; set; }
        public List<T> Items { get; set; }
    }
}
