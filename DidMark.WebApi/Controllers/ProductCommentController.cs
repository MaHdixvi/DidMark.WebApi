using DidMark.Core.DTO.Products.NewFolder;
using DidMark.Core.DTO.Products.ProductComment;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCommentController : SiteBaseController
    {
        #region Fields
        private readonly IProductCommentService _commentService;

        #endregion

        #region Constructor
        public ProductCommentController(IProductCommentService commentService)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }
        #endregion

        #region Public Actions

        /// <summary>
        /// دریافت کامنت‌های یک محصول
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductComments(long productId, [FromQuery] bool onlyApproved = true)
        {
            if (productId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول نامعتبر است" });

            var comments = await _commentService.GetProductComments(productId, onlyApproved);
            return JsonResponseStatus.Success(comments);
        }

        /// <summary>
        /// افزودن کامنت جدید
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddComment([FromBody] AddProductCommentDTO commentDto)
        {
            if (!ModelState.IsValid)
                return JsonResponseStatus.BadRequest(new { message = "داده‌های ورودی معتبر نیستند" });

            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            var result = await _commentService.AddComment(commentDto, userIp);
            if (!result)
                return JsonResponseStatus.Error(new { message = "خطا در ثبت کامنت" });

            return JsonResponseStatus.Success(new { message = "کامنت با موفقیت ثبت شد و در انتظار تایید می‌باشد" });
        }

        /// <summary>
        /// دریافت پاسخ‌های یک کامنت
        /// </summary>
        [HttpGet("replies/{parentCommentId}")]
        public async Task<IActionResult> GetCommentReplies(long parentCommentId)
        {
            if (parentCommentId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه کامنت والد نامعتبر است" });

            var replies = await _commentService.GetCommentReplies(parentCommentId);
            return JsonResponseStatus.Success(replies);
        }

        /// <summary>
        /// دریافت امتیاز متوسط محصول
        /// </summary>
        [HttpGet("rating/{productId}")]
        public async Task<IActionResult> GetProductRating(long productId)
        {
            if (productId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه محصول نامعتبر است" });

            var averageRating = await _commentService.GetProductAverageRating(productId);
            var commentsCount = await _commentService.GetProductCommentsCount(productId);

            return JsonResponseStatus.Success(new
            {
                averageRating,
                commentsCount
            });
        }

        #endregion

        #region Admin Actions

        /// <summary>
        /// فیلتر کامنت‌ها (برای ادمین)
        /// </summary>
        [HttpGet("admin/filter")]
        public async Task<IActionResult> FilterComments([FromQuery] FilterProductCommentsDTO filter)
        {
            if (filter == null)
                return JsonResponseStatus.BadRequest(new { message = "داده‌های فیلتر ارائه نشده است" });

            filter.TakeEntity = 20;
            var result = await _commentService.FilterComments(filter);
            return JsonResponseStatus.Success(result);
        }

        /// <summary>
        /// تایید کامنت
        /// </summary>
        [HttpPost("admin/approve/{commentId}")]
        public async Task<IActionResult> ApproveComment(long commentId)
        {
            if (commentId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه کامنت نامعتبر است" });

            var result = await _commentService.ApproveComment(commentId);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "کامنت یافت نشد" });

            return JsonResponseStatus.Success(new { message = "کامنت با موفقیت تایید شد" });
        }

        /// <summary>
        /// رد کامنت
        /// </summary>
        [HttpPost("admin/reject/{commentId}")]
        public async Task<IActionResult> RejectComment(long commentId)
        {
            if (commentId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه کامنت نامعتبر است" });

            var result = await _commentService.RejectComment(commentId);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "کامنت یافت نشد" });

            return JsonResponseStatus.Success(new { message = "کامنت با موفقیت رد شد" });
        }

        /// <summary>
        /// علامت‌گذاری کامنت به عنوان خوانده شده
        /// </summary>
        [HttpPost("admin/mark-read/{commentId}")]
        public async Task<IActionResult> MarkAsRead(long commentId)
        {
            if (commentId <= 0)
                return JsonResponseStatus.BadRequest(new { message = "شناسه کامنت نامعتبر است" });

            var result = await _commentService.MarkAsRead(commentId);
            if (!result)
                return JsonResponseStatus.NotFound(new { message = "کامنت یافت نشد" });

            return JsonResponseStatus.Success(new { message = "کامنت با موفقیت علامت‌گذاری شد" });
        }

        /// <summary>
        /// تعداد کامنت‌های خوانده نشده
        /// </summary>
        [HttpGet("admin/unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _commentService.GetUnreadCommentsCount();
            return JsonResponseStatus.Success(new { unreadCount = count });
        }

        #endregion
    }
}