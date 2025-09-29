using System;
using System.Collections.Generic;

namespace DidMark.Core.DTO.Products.ProductComment
{
    public class ProductCommentDto
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string CommentText { get; set; }
        public int? Rating { get; set; }
        public long? ParentId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRead { get; set; }
        public string UserIp { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public List<ProductCommentDto> Replies { get; set; }

        public ProductCommentDto()
        {
            Replies = new List<ProductCommentDto>();
        }
    }
}