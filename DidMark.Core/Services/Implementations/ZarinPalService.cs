using DidMark.Core.DTO.ZarinPal.Payment;
using DidMark.Core.DTO.ZarinPal.ReFound;
using DidMark.Core.DTO.ZarinPal.UnVerification;
using DidMark.Core.DTO.ZarinPal.Verification;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using DidMark.Core.Services.Interfaces;


namespace DidMark.Core.Services.Implementations
{
    public class ZarinPalService : IZarinPalService
    {
        private readonly string _merchantId;
        private readonly string _paymentUrl;
        private readonly string _verifyUrl;
        private readonly string _unVerifiedUrl;
        private readonly string _reFoundUrl;
        private readonly string _reFoundToken;
        private readonly string _gateWayUrl;

        private readonly bool _isSandBox;

        public ZarinPalService(IConfiguration configuration)
        {
            _isSandBox = bool.Parse(configuration["ZarinPal:IsSandBox"] ?? "false");

            if (_isSandBox)
            {
                _merchantId = configuration["ZarinPal-SandBox:merchant"];
                _paymentUrl = configuration["ZarinPal-SandBox:paymentUrl"];
                _verifyUrl = configuration["ZarinPal-SandBox:verifyUrl"];
                _gateWayUrl = configuration["ZarinPal-SandBox:StartPay"];
            }
            else
            {
                _merchantId = configuration["ZarinPal:merchant"];
                _paymentUrl = configuration["ZarinPal:paymentUrl"];
                _verifyUrl = configuration["ZarinPal:verifyUrl"];
                _gateWayUrl = configuration["ZarinPal:StartPay"];
                _unVerifiedUrl = configuration["ZarinPal:unVerifiedUrl"];
                _reFoundUrl = configuration["ZarinPal:reFoundUrl"];
                _reFoundToken = configuration["ZarinPal:reFoundToken"];
            }
        }

        public async Task<PaymentResponseData> CreatePaymentRequest(decimal amount,
            string description,
            string callBackUrl,
            string mobile = null, string email = null)
        {
            var client = new RestClient(_paymentUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var body = new PaymentRequest
            {
                Mobile = mobile,
                CallbackUrl = callBackUrl,
                Description = description,
                Email = email,
                Amount = amount * 10, // تومان → ریال
                MerchantId = _merchantId
            };

            request.AddJsonBody(JsonConvert.SerializeObject(body));
            var response = await client.ExecuteAsync(request);

            var result = JsonConvert.DeserializeObject<PaymentResponse>(response.Content);
            if (result?.Data.Status == 100)
                result.Data.GateWayUrl = _gateWayUrl + result.Data.Authority;

            return result?.Data;
        }

        public async Task<FinallyVerificationResponse> CreateVerificationRequest(string authority, decimal price)
        {
            var client = new RestClient(_verifyUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var body = new VerificationRequest
            {
                Amount = price * 10, // تومان → ریال
                MerchantId = _merchantId,
                Authority = authority
            };

            request.AddJsonBody(JsonConvert.SerializeObject(body));
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<VerificationResponse>(response.Content);
                return new FinallyVerificationResponse
                {
                    Message = null,
                    CardPan = result?.Data.CardPan,
                    RefId = result.Data.RefId,
                    Status = result.Data.Status
                };
            }
            else
            {
                var result = JsonConvert.DeserializeObject<ErrorVerificationResponse>(response.Content);
                return new FinallyVerificationResponse
                {
                    Message = result.Errors.Message,
                    RefId = 0,
                    Status = result.Errors.Code
                };
            }
        }

        public async Task<UnVerificationFinallyResponse> GetUnVerificationRequests()
        {
            if (_isSandBox) throw new InvalidOperationException("این متد در حالت Sandbox پشتیبانی نمی‌شود.");

            var client = new RestClient(_unVerifiedUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var body = new UnVerificationRequest { MerchantId = _merchantId };
            request.AddJsonBody(JsonConvert.SerializeObject(body));
            var response = await client.ExecuteAsync(request);

            var result = JsonConvert.DeserializeObject<UnVerificationResponse>(response.Content);
            return result?.Data;
        }

        public async Task<ReFoundResponse> ReFoundRequest(string authority)
        {
            if (_isSandBox) throw new InvalidOperationException("این متد در حالت Sandbox پشتیبانی نمی‌شود.");

            var client = new RestClient(_reFoundUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", $"Bearer {_reFoundToken}");
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new ReFoundRequest
            {
                Authority = authority,
                MerchantId = _merchantId
            });

            var result = await client.ExecuteAsync<ReFoundResponse>(request);
            return result.Data;
        }
    }
}
