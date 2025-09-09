using DidMark.Core.DTO.Payment;
using DidMark.Core.Services.Implementations;
using DidMark.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class PaymentController : SiteBaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPayment([FromBody] PaymentRequestDto dto)
        {
            var result = await _paymentService.RequestPaymentAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyPayment([FromQuery] string authority, [FromQuery] int amount)
        {
            var result = await _paymentService.VerifyPaymentAsync(new VerifyPaymentRequestDto
            {
                Authority = authority,
                Amount = amount
            });

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}

