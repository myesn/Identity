using System;
using System.Collections.Generic;
using System.Text;

namespace Upo.Identity
{
    public class PagingContext
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }
}