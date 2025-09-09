using System.Threading.Tasks;
using DidMark.Core.DTO.Payment;
using DidMark.Core.Services.Interfaces;
using Zarinpal.Class;

namespace DidMark.Core.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly string _merchantId = "00000000-0000-0000-0000-000000000000"; // 🔹 مرچنت تستی
        private readonly bool _sandbox = true; // 🔹 اگر true باشه sandbox استفاده میشه

        public async Task<PaymentResponseDto> RequestPaymentAsync(PaymentRequestDto dto)
        {
            var payment = new Payment(dto.Amount);
            var result = await payment.PaymentRequest(_merchantId, dto.Description, "", "", dto.CallbackUrl);

            if (result.Status == 100)
            {
                var gatewayBaseUrl = _sandbox
                    ? "https://sandbox.zarinpal.com/pg/StartPay/"
                    : "https://www.zarinpal.com/pg/StartPay/";

                return new PaymentResponseDto
                {
                    IsSuccess = true,
                    Authority = result.Authority,
                    GatewayUrl = gatewayBaseUrl + result.Authority
                };
            }

            return new PaymentResponseDto
            {
                IsSuccess = false,
                ErrorMessage = "خطا در ایجاد پرداخت. کد: " + result.Status
            };
        }

        public async Task<VerifyPaymentResponseDto> VerifyPaymentAsync(VerifyPaymentRequestDto dto)
        {
            var payment = new Payment(dto.Amount);
            var verification = await payment.Verification(dto.Authority, _merchantId);

            if (verification.Status == 100)
            {
                return new VerifyPaymentResponseDto
                {
                    IsSuccess = true,
                    RefId = verification.RefId
                };
            }

            return new VerifyPaymentResponseDto
            {
                IsSuccess = false,
                ErrorMessage = "پرداخت تایید نشد. کد: " + verification.Status
            };
        }
    }
}
