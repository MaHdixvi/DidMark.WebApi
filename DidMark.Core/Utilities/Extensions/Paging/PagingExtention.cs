using DidMark.Core.DTO.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Utilities.Extensions.Paging
{
    public static class PagingExtention
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> queryable, BasePaging pager)
        { return queryable.Skip(pager.SkipEntity).Take(pager.TakeEntity); }
    }
}