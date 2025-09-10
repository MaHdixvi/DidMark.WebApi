using DidMark.Core.DTO.ZarinPal.Payment;
using DidMark.Core.DTO.ZarinPal.ReFound;
using DidMark.Core.DTO.ZarinPal.UnVerification;
using DidMark.Core.DTO.ZarinPal.Verification;
using DidMark.Core.Services.Implementations;


namespace DidMark.Core.Services.Interfaces
{
    public interface IZarinPalService
    {
        Task<PaymentResponseData> CreatePaymentRequest(decimal amount,
            string description,
            string callBackUrl,
            string mobile = null, string email = null);
        Task<FinallyVerificationResponse> CreateVerificationRequest(string authority, decimal price);
        Task<UnVerificationFinallyResponse> GetUnVerificationRequests();
        Task<ReFoundResponse> ReFoundRequest(string authority);
    }
}