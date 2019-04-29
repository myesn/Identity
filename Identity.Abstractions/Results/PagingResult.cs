using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity
{
    public class PagingResult
    {
        public object Value { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}