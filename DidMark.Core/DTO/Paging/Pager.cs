using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Paging
{
    public class Pager
    {
        public static BasePaging Build(int pageCount, int pageNumber, int Take)
        {
            if (pageNumber <= 1) pageNumber = 1;
            return new BasePaging()
            {
                ActivePage = pageNumber,
                PageCount = pageCount,
                PageId = pageNumber,
                TakeEntity = Take,
                SkipEntity = (pageNumber - 1) * Take,
                StartPage = pageNumber - 3 <= 0 ? 1 : pageNumber - 3,
                EndPage = pageNumber + 3 > pageCount ? pageCount : pageNumber + 3,
            };
        }
    }
}
