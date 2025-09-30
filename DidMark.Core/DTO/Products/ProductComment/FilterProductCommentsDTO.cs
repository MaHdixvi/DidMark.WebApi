using DidMark.Core.DTO.Paging;

namespace DidMark.Core.DTO.Products.ProductComment

{
    public class FilterProductCommentsDTO : BasePaging
    {
        public long? ProductId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsRead { get; set; }
        public bool? HasRating { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ProductName { get; set; }

        public List<ProductCommentDto> Comments { get; set; }

        public FilterProductCommentsDTO()
        {
            Comments = new List<ProductCommentDto>();
        }

        public FilterProductCommentsDTO SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.PageCount = paging.PageCount;
            this.ActivePage = paging.ActivePage;
            this.StartPage = paging.StartPage;
            this.EndPage = paging.EndPage;
            this.TakeEntity = paging.TakeEntity;
            this.SkipEntity = paging.SkipEntity;
            return this;
        }

        public FilterProductCommentsDTO SetComments(List<ProductCommentDto> comments)
        {
            this.Comments = comments;
            return this;
        }
    }
}