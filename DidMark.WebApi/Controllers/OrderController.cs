using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Extensions.Identity;
using DidMark.DataLayer.Entities.Account;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class OrderController : SiteBaseController
    {
        #region Constructor
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;

        }
        #endregion
        #region Add Product To Order
        [HttpGet(template: "add_order")]
        public async Task<ActionResult> AddProductToOrder(long productId, int count)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.GetUserId();
                await _orderService.AddProductToOrder(userId, productId, count);
                return JsonResponseStatus.Success(returnData: new
                {
                    message = "محصول با موفقیت به سبد خرید اضافه شد",
                    returnData = await _orderService.GetUserBasketDetails(userId),
                });
            }
            return JsonResponseStatus.Error(returnData: new { message = "برای افزودن محصول به سبد خرید ,ابتدا  ثبت نام/ورود  کنید" });

        }
        #endregion

        #region User Basket Details
        [HttpGet("get-order-details")]
        public async Task<IActionResult> GetUserBasketDetails()
        {
            if (User.Identity.IsAuthenticated)
            {
                var details = await _orderService.GetUserBasketDetails(User.GetUserId());
                return JsonResponseStatus.Success(details);
            }
            return JsonResponseStatus.Error();
        }

        #endregion

        #region remove order detail from basket
        [HttpGet("remove-order-details/{detailId}")]
        public async Task<IActionResult> RemoveOrderDetail(int detailId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var useropenorder = await _orderService.GetUserOpenOrder(User.GetUserId());
                var detail = useropenorder.OrderDetails.SingleOrDefault(x => x.Id == detailId);
                if (detail != null)
                {
                    await _orderService.DeleteOrderDetails(detail);
                    return JsonResponseStatus.Success(await _orderService.GetUserBasketDetails(User.GetUserId()));
                }
            }
            return JsonResponseStatus.Error();
        }

        #endregion
    }
}
