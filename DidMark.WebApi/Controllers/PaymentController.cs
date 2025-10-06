using Microsoft.AspNetCore.Mvc;
using DidMark.Core.DTO.Payment;
using System.Threading.Tasks;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.DTO.ZarinPal.Payment;
using DidMark.Core.Utilities.Extensions.Identity;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Authorization;
using DidMark.DataLayer.Entities.Transaction;
using DidMark.Core.DTO.Orders;

namespace DidMark.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ITransactionLogService _transactionService;
        private readonly IZarinPalService _zarinPalService;

        public PaymentController(
            IOrderService orderService,
            ITransactionLogService transactionService,
            IZarinPalService zarinPalService)
        {
            _orderService = orderService;
            _transactionService = transactionService;
            _zarinPalService = zarinPalService;

        }

        private long GetCurrentUserId()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                throw new UnauthorizedAccessException("کاربر وارد نشده است");

            return User.GetUserId();
        }


        #region Create Transaction

        [HttpPost("create-transaction")]
        [Authorize]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransaction command)
        {
            var userId = GetCurrentUserId();

            var order = await _orderService.GetUserOpenOrderAsync(userId);
            if (order == null)
                return BadRequest(new { message = "سفارشی یافت نشد" });

            command.UserId = userId;
            command.PaymentAmount = order.TotalPrice;
            command.LinkId = order.Id;
            command.PaymentGateway = PaymentGateway.ZarinPal;

            var transactionId = await _transactionService.CreateTransaction(command);

            var payment = await _zarinPalService.CreatePaymentRequest(
                command.PaymentAmount,
                $"پرداخت سفارش شماره {transactionId}",
                $"https://yourdomain.com/checkout/verify/{transactionId}");

            if (payment?.Status == 100)
                return Ok(new { redirectUrl = payment.GateWayUrl });

            return BadRequest(new { message = "خطا در ایجاد درخواست پرداخت" });
        }

        #endregion

        #region Verify Transaction

        [HttpGet("verify/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> VerifyTransaction(long transactionId, [FromQuery] string authority, [FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status) || status.ToLower() == "nok" || string.IsNullOrWhiteSpace(authority))
            {
                await _transactionService.PaymentError(new TransactionPaymentError
                {
                    TransactionId = transactionId,
                    Authority = authority,
                    RefId = 0,
                    ErrorMessage = "پرداخت لغو شد یا با خطا مواجه شد",
                    Canceled = true
                });
                return Ok(new { message = "پرداخت لغو شد" });
            }

            var transaction = await _transactionService.GetTransactionById(transactionId);
            if (transaction == null) return NotFound();

            if (transaction.UserId != GetCurrentUserId())
                return BadRequest(new { message = "کاربر معتبر نیست" });

            if (transaction.Status == TransactionStatus.PaymentSuccess)
                return BadRequest(new { message = "تراکنش قبلاً موفق بوده است" });

            try
            {
                var verifyResult = await _zarinPalService.CreateVerificationRequest(authority, transaction.PaymentAmount);
                if (verifyResult.Status == 100)
                {
                    await _transactionService.PaymentSuccess(new TransactionPaymentSuccess
                    {
                        TransactionId = transactionId,
                        Authority = authority,
                        CardPen = verifyResult.CardPan,
                        RefId = verifyResult.RefId,
                        SuccessCallBack = ""
                    });

                    // تکمیل سفارش و اختصاص دوره یا محصول
                    var order = await _orderService.GetUserOpenOrderAsync(transaction.UserId);
                    await _orderService.MarkOrderAsPaidAsync(order.Id, verifyResult.RefId);

                    return Ok(new { message = "پرداخت با موفقیت انجام شد" });
                }
                else
                {
                    await _transactionService.PaymentError(new TransactionPaymentError
                    {
                        TransactionId = transactionId,
                        Authority = authority,
                        RefId = verifyResult.RefId,
                        ErrorMessage = verifyResult.Message
                    });
                    return BadRequest(new { message = "پرداخت ناموفق بود" });
                }
            }
            catch (Exception ex)
            {
                await _transactionService.PaymentError(new TransactionPaymentError
                {
                    TransactionId = transactionId,
                    Authority = authority,
                    RefId = 0,
                    ErrorMessage = ex.Message
                });
                return StatusCode(500, new { message = "خطا در پردازش تراکنش", detail = ex.Message });
            }
        }

        #endregion

    }
}
