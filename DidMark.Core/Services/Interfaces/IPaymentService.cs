using DidMark.Core.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// درخواست پرداخت و گرفتن لینک درگاه
        /// </summary>
        Task<PaymentResponseDto> RequestPaymentAsync(PaymentRequestDto dto);

        /// <summary>
        /// تایید و وریفای پرداخت بعد از بازگشت از درگاه
        /// </summary>
        Task<VerifyPaymentResponseDto> VerifyPaymentAsync(VerifyPaymentRequestDto dto);
    }
}
