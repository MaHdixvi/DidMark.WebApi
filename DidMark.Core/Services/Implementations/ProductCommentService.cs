using DidMark.Core.DTO.Paging;
using DidMark.Core.DTO.Products.ProductComment;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Extensions.Paging;
using DidMark.DataLayer.Entities.Comments;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class ProductCommentService : IProductCommentService
    {
        private readonly IGenericRepository<ProductComment> _commentRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public ProductCommentService(
            IGenericRepository<ProductComment> commentRepository,
            IGenericRepository<Product> productRepository)
        {
            _commentRepository = commentRepository;
            _productRepository = productRepository;
        }

        #region Comment Management

        public async Task<bool> AddComment(AddProductCommentDTO commentDto, string userIp)
        {
            try
            {
                // بررسی وجود محصول
                var product = await _productRepository.GetEntitiesQuery()
                    .AnyAsync(p => p.Id == commentDto.ProductId && !p.IsDelete);
                if (!product) return false;

                // اگر کامنت والد مشخص شده، بررسی وجود آن
                if (commentDto.ParentId.HasValue)
                {
                    var parentComment = await _commentRepository.GetEntitiesQuery()
                        .AnyAsync(c => c.Id == commentDto.ParentId.Value && !c.IsDelete);
                    if (!parentComment) return false;
                }

                var comment = new ProductComment
                {
                    ProductId = commentDto.ProductId,
                    UserName = commentDto.UserName,
                    UserEmail = commentDto.UserEmail,
                    CommentText = commentDto.CommentText,
                    Rating = commentDto.Rating,
                    ParentId = commentDto.ParentId,
                    UserIp = userIp,
                    IsApproved = false, // پیش‌فرض تایید نشده
                    IsRead = false,
                    CreateDate = DateTime.Now,
                    IsDelete = false
                };

                await _commentRepository.AddEntity(comment);
                await _commentRepository.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateComment(EditProductCommentDTO commentDto)
        {
            var comment = await _commentRepository.GetEntityById(commentDto.Id);
            if (comment == null || comment.IsDelete) return false;

            if (!string.IsNullOrEmpty(commentDto.CommentText))
                comment.CommentText = commentDto.CommentText;

            if (commentDto.Rating.HasValue)
                comment.Rating = commentDto.Rating.Value;

            if (commentDto.IsApproved.HasValue)
                comment.IsApproved = commentDto.IsApproved.Value;

            if (commentDto.IsRead.HasValue)
                comment.IsRead = commentDto.IsRead.Value;

            _commentRepository.UpdateEntity(comment);
            await _commentRepository.SaveChanges();

            return true;
        }

        public async Task<bool> DeleteComment(long commentId)
        {
            var comment = await _commentRepository.GetEntityById(commentId);
            if (comment == null) return false;

            comment.IsDelete = true;
            _commentRepository.UpdateEntity(comment);
            await _commentRepository.SaveChanges();

            return true;
        }

        public async Task<ProductCommentDto> GetCommentById(long commentId)
        {
            var comment = await _commentRepository.GetEntitiesQuery()
                .Include(c => c.Product)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Product)
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDelete);

            if (comment == null) return null;

            return new ProductCommentDto
            {
                Id = comment.Id,
                ProductId = comment.ProductId,
                UserId = comment.UserId,
                UserName = comment.UserName,
                UserEmail = comment.UserEmail,
                CommentText = comment.CommentText,
                Rating = comment.Rating,
                ParentId = comment.ParentId,
                IsApproved = comment.IsApproved,
                IsRead = comment.IsRead,
                UserIp = comment.UserIp,
                CreateDate = comment.CreateDate,
                ProductName = comment.Product?.ProductName,
                ProductImage = comment.Product?.ImageName,
                Replies = comment.Replies?
                    .Where(r => !r.IsDelete && r.IsApproved)
                    .Select(r => new ProductCommentDto
                    {
                        Id = r.Id,
                        ProductId = r.ProductId,
                        UserId = r.UserId,
                        UserName = r.UserName,
                        UserEmail = r.UserEmail,
                        CommentText = r.CommentText,
                        Rating = r.Rating,
                        ParentId = r.ParentId,
                        IsApproved = r.IsApproved,
                        IsRead = r.IsRead,
                        UserIp = r.UserIp,
                        CreateDate = r.CreateDate,
                        ProductName = r.Product?.ProductName,
                        ProductImage = r.Product?.ImageName,
                        Replies = new List<ProductCommentDto>() // پاسخ‌های سطح سوم
                    }).ToList()
            };
        }

        public async Task<FilterProductCommentsDTO> FilterComments(FilterProductCommentsDTO filter)
        {
            var commentsQuery = _commentRepository.GetEntitiesQuery()
                .Include(c => c.Product)
                .Include(c => c.Replies)
                .Where(c => !c.IsDelete)
                .AsQueryable();

            // فیلترها
            if (filter.ProductId.HasValue)
                commentsQuery = commentsQuery.Where(c => c.ProductId == filter.ProductId.Value);

            if (!string.IsNullOrEmpty(filter.UserName))
                commentsQuery = commentsQuery.Where(c => c.UserName.Contains(filter.UserName));

            if (!string.IsNullOrEmpty(filter.UserEmail))
                commentsQuery = commentsQuery.Where(c => c.UserEmail.Contains(filter.UserEmail));

            if (!string.IsNullOrEmpty(filter.ProductName))
                commentsQuery = commentsQuery.Where(c => c.Product.ProductName.Contains(filter.ProductName));

            if (filter.IsApproved.HasValue)
                commentsQuery = commentsQuery.Where(c => c.IsApproved == filter.IsApproved.Value);

            if (filter.IsRead.HasValue)
                commentsQuery = commentsQuery.Where(c => c.IsRead == filter.IsRead.Value);

            if (filter.HasRating.HasValue)
                commentsQuery = commentsQuery.Where(c => filter.HasRating.Value ? c.Rating.HasValue : !c.Rating.HasValue);

            if (filter.FromDate.HasValue)
                commentsQuery = commentsQuery.Where(c => c.CreateDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                commentsQuery = commentsQuery.Where(c => c.CreateDate <= filter.ToDate.Value);

            // فقط کامنت‌های اصلی (غیر پاسخ)
            commentsQuery = commentsQuery.Where(c => c.ParentId == null);

            // مرتب‌سازی
            commentsQuery = commentsQuery.OrderByDescending(c => c.CreateDate);

            var count = (int)Math.Ceiling(commentsQuery.Count() / (double)filter.TakeEntity);
            var pager = Pager.Build(count, filter.PageId, filter.TakeEntity);

            var comments = await commentsQuery.Paging(pager).ToListAsync();

            var commentDtos = comments.Select(comment => new ProductCommentDto
            {
                Id = comment.Id,
                ProductId = comment.ProductId,
                UserId = comment.UserId,
                UserName = comment.UserName,
                UserEmail = comment.UserEmail,
                CommentText = comment.CommentText,
                Rating = comment.Rating,
                ParentId = comment.ParentId,
                IsApproved = comment.IsApproved,
                IsRead = comment.IsRead,
                UserIp = comment.UserIp,
                CreateDate = comment.CreateDate,
                ProductName = comment.Product?.ProductName,
                ProductImage = comment.Product?.ImageName,
                Replies = comment.Replies?
                    .Where(r => !r.IsDelete && r.IsApproved)
                    .Select(r => new ProductCommentDto
                    {
                        Id = r.Id,
                        ProductId = r.ProductId,
                        UserId = r.UserId,
                        UserName = r.UserName,
                        UserEmail = r.UserEmail,
                        CommentText = r.CommentText,
                        Rating = r.Rating,
                        ParentId = r.ParentId,
                        IsApproved = r.IsApproved,
                        IsRead = r.IsRead,
                        UserIp = r.UserIp,
                        CreateDate = r.CreateDate,
                        ProductName = r.Product?.ProductName,
                        ProductImage = r.Product?.ImageName,
                        Replies = new List<ProductCommentDto>()
                    }).ToList()
            }).ToList();

            return filter.SetComments(commentDtos).SetPaging(pager);
        }

        public async Task<List<ProductCommentDto>> GetProductComments(long productId, bool onlyApproved = true)
        {
            var commentsQuery = _commentRepository.GetEntitiesQuery()
                .Include(c => c.Product)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Product)
                .Where(c => c.ProductId == productId && !c.IsDelete && c.ParentId == null);

            if (onlyApproved)
                commentsQuery = commentsQuery.Where(c => c.IsApproved);

            var comments = await commentsQuery
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();

            return comments.Select(comment => new ProductCommentDto
            {
                Id = comment.Id,
                ProductId = comment.ProductId,
                UserId = comment.UserId,
                UserName = comment.UserName,
                UserEmail = comment.UserEmail,
                CommentText = comment.CommentText,
                Rating = comment.Rating,
                ParentId = comment.ParentId,
                IsApproved = comment.IsApproved,
                IsRead = comment.IsRead,
                UserIp = comment.UserIp,
                CreateDate = comment.CreateDate,
                ProductName = comment.Product?.ProductName,
                ProductImage = comment.Product?.ImageName,
                Replies = comment.Replies?
                    .Where(r => !r.IsDelete && (onlyApproved ? r.IsApproved : true))
                    .Select(r => new ProductCommentDto
                    {
                        Id = r.Id,
                        ProductId = r.ProductId,
                        UserId = r.UserId,
                        UserName = r.UserName,
                        UserEmail = r.UserEmail,
                        CommentText = r.CommentText,
                        Rating = r.Rating,
                        ParentId = r.ParentId,
                        IsApproved = r.IsApproved,
                        IsRead = r.IsRead,
                        UserIp = r.UserIp,
                        CreateDate = r.CreateDate,
                        ProductName = r.Product?.ProductName,
                        ProductImage = r.Product?.ImageName,
                        Replies = new List<ProductCommentDto>()
                    }).ToList()
            }).ToList();
        }

        public async Task<List<ProductCommentDto>> GetCommentReplies(long parentCommentId)
        {
            var replies = await _commentRepository.GetEntitiesQuery()
                .Include(c => c.Product)
                .Where(c => c.ParentId == parentCommentId && !c.IsDelete && c.IsApproved)
                .OrderBy(c => c.CreateDate)
                .ToListAsync();

            return replies.Select(reply => new ProductCommentDto
            {
                Id = reply.Id,
                ProductId = reply.ProductId,
                UserId = reply.UserId,
                UserName = reply.UserName,
                UserEmail = reply.UserEmail,
                CommentText = reply.CommentText,
                Rating = reply.Rating,
                ParentId = reply.ParentId,
                IsApproved = reply.IsApproved,
                IsRead = reply.IsRead,
                UserIp = reply.UserIp,
                CreateDate = reply.CreateDate,
                ProductName = reply.Product?.ProductName,
                ProductImage = reply.Product?.ImageName,
                Replies = new List<ProductCommentDto>()
            }).ToList();
        }

        #endregion

        #region Admin Operations

        public async Task<bool> ApproveComment(long commentId)
        {
            var comment = await _commentRepository.GetEntityById(commentId);
            if (comment == null || comment.IsDelete) return false;

            comment.IsApproved = true;
            comment.IsRead = true;

            _commentRepository.UpdateEntity(comment);
            await _commentRepository.SaveChanges();

            return true;
        }

        public async Task<bool> RejectComment(long commentId)
        {
            var comment = await _commentRepository.GetEntityById(commentId);
            if (comment == null || comment.IsDelete) return false;

            comment.IsApproved = false;
            comment.IsRead = true;

            _commentRepository.UpdateEntity(comment);
            await _commentRepository.SaveChanges();

            return true;
        }

        public async Task<bool> MarkAsRead(long commentId)
        {
            var comment = await _commentRepository.GetEntityById(commentId);
            if (comment == null || comment.IsDelete) return false;

            comment.IsRead = true;

            _commentRepository.UpdateEntity(comment);
            await _commentRepository.SaveChanges();

            return true;
        }

        public async Task<int> GetUnreadCommentsCount()
        {
            return await _commentRepository.GetEntitiesQuery()
                .CountAsync(c => !c.IsDelete && !c.IsRead);
        }

        public async Task<double> GetProductAverageRating(long productId)
        {
            var ratings = await _commentRepository.GetEntitiesQuery()
                .Where(c => c.ProductId == productId &&
                           !c.IsDelete &&
                           c.IsApproved &&
                           c.Rating.HasValue)
                .Select(c => c.Rating.Value)
                .ToListAsync();

            if (!ratings.Any()) return 0;

            return Math.Round(ratings.Average(), 1);
        }

        public async Task<int> GetProductCommentsCount(long productId)
        {
            return await _commentRepository.GetEntitiesQuery()
                .CountAsync(c => c.ProductId == productId &&
                                !c.IsDelete &&
                                c.IsApproved);
        }

        #endregion

        #region Dispose
        public void Dispose()
        {
            _commentRepository?.Dispose();
            _productRepository?.Dispose();
        }
        #endregion
    }
}