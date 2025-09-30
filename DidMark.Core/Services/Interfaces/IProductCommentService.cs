using DidMark.Core.DTO.Products.ProductComment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IProductCommentService
    {
        #region Comment Management
        Task<bool> AddComment(AddProductCommentDTO commentDto, string userIp);
        Task<bool> UpdateComment(EditProductCommentDTO commentDto);
        Task<bool> DeleteComment(long commentId);
        Task<ProductCommentDto> GetCommentById(long commentId);
        Task<FilterProductCommentsDTO> FilterComments(FilterProductCommentsDTO filter);
        Task<List<ProductCommentDto>> GetProductComments(long productId, bool onlyApproved = true);
        Task<List<ProductCommentDto>> GetCommentReplies(long parentCommentId);
        #endregion

        #region Admin Operations
        Task<bool> ApproveComment(long commentId);
        Task<bool> RejectComment(long commentId);
        Task<bool> MarkAsRead(long commentId);
        Task<int> GetUnreadCommentsCount();
        Task<double> GetProductAverageRating(long productId);
        Task<int> GetProductCommentsCount(long productId);
        #endregion

        void Dispose();
    }
}