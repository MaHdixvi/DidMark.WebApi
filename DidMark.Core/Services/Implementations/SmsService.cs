using DidMark.Core.DTO.Orders;
using DidMark.Core.DTO.SMS;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PayamakCore.Dto;
using PayamakCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DidMark.Core.Services.Implementations
{

    public class SmsService : ISmsService
    {
        private readonly IPayamakServices _payamakServices;
        private readonly string _username;
        private readonly string _password;
        private readonly string _from;
        private readonly string _apiUrl;
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public SmsService(IPayamakServices payamakServices, IConfiguration configuration)
        {
            _payamakServices = payamakServices;

            // مقداردهی مستقیم از IConfiguration
            _username = configuration.GetValue<string>("PayamakPanel:Username");
            _password = configuration.GetValue<string>("PayamakPanel:Password");
            _from = configuration.GetValue<string>("PayamakPanel:From");
            _apiUrl = configuration.GetValue<string>("PayamakPanel:ApiUrl");
        }

        public async Task<bool> SendRegistrationSmsAsync(string phoneNumber, string firstName, string lastName, string nationalCode, string collectionName, string userPhoneNumber)
        {
            var message = $"ثبت نام جدید توسط {firstName}\n {lastName} انجام شده است.\n کد ملی: {nationalCode}\n نام مجموعه: {collectionName}\n شماره کاربر: {userPhoneNumber}\n. لطفاً عدد 1 را برای تأیید و عدد 0 را برای لغو ارسال کنید.";
            return await SendSmsAsync(phoneNumber, message);
        }

        public async Task<bool> SendActivationCodeSmsAsync(string phoneNumber, string activationCode)
        {
            var message = $"برای تغییر رمز عبور خود روی این لینک بزنید https://localhost4200/login/{activationCode}";
            return await SendSmsAsync(phoneNumber, message);
        }
        public async Task<bool> SendActivatedAccountSmsAsync(string phoneNumber)
        {
            var message = $"کاربر گرامی ثبت نام شما تایید شد";
            return await SendSmsAsync(phoneNumber, message);
        }
        public async Task<bool> SendActivatedEmailSmsAsync(string phoneNumber)
        {
            var message = $"کاربر گرامی ایمیل شما تایید شد";
            return await SendSmsAsync(phoneNumber, message);
        }

        private async Task<bool> SendSmsAsync(string to, string message)
        {
            var result = await _payamakServices.SendSms(new MessageDto
            {
                From = _from,
                To = to,
                Text = message,
                username = _username,
                password = _password,
                IsFlash = false
            });

            return result.RetStatus == 1;
        }

        //public async Task<bool> WaitForActivationAsync(string userName, CancellationToken cancellationToken = default)
        //{
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        var messages = await _payamakServices.GetMyMessageList(new MyMessageRequestDto
        //        {
        //            from = _from,
        //            location = 1,
        //            index = 0,
        //            count = 1000
        //        }, cancellationToken);

        //        if (messages?.Data != null && messages.Data.Any(m => m.Body.Trim() == "1"))
        //        {
        //            return true;
        //        }

        //        await Task.Delay(5000, cancellationToken);
        //    }
        //    return false;
        //}

        public async Task<bool> WaitForActivationAsync(string userName, DateTime dateTime)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                //await Task.Delay(2000);
                var messages = await GetMyMessageList(new DTO.SMS.MyMessageRequestDto
                {
                    Username = _username,
                    Password = _password,
                    From = _from,
                    Location = 1,
                    Index = 0,
                    Count = 1,
                    DateFrom = dateTime,
                    DateTo = dateTime.AddHours(2)
                }, _cancellationTokenSource.Token);
                if (messages?.Messages.Count != 0)
                {
                    if (messages.Messages.Any(m => m.Body?.Trim() == "1" && m.Sender == "9122423630"))
                    {
                        return true;
                    }
                    if (messages.Messages.Any(m => m.Body?.Trim() == "0" && m.Sender == "9122423630"))
                    {
                        return false;
                    }
                }


                await Task.Delay(2000, _cancellationTokenSource.Token);
            }

            return false;
        }
        public async Task<bool> SendOrderSummarySmsAsync(string phoneNumber, List<OrderBasketDetail> items, decimal totalPrice)
        {
            if (items == null || items.Count == 0) return false;

            var message = "سبد خرید شما:\n";
            foreach (var item in items)
            {
                message += $"{item.ProductName} x {item.Count} - {item.Price} تومان\n";
            }
            message += $"جمع کل: {totalPrice} تومان";

            return await SendSmsAsync(phoneNumber, message);
        }


        private async Task<MyMessageResponseDto> GetMyMessageList(DidMark.Core.DTO.SMS.MyMessageRequestDto requestDto, CancellationToken cancellationToken)
        {
            var url = "https://api.payamak-panel.com/post/receive.asmx/GetMessagesByDate";
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "username", requestDto.Username },
                { "password", requestDto.Password },
                { "location", requestDto.Location.ToString() },
                { "from", requestDto.From },
                { "index", requestDto.Index.ToString() },
                { "count", requestDto.Count.ToString() },
                { "dateFrom", requestDto.DateFrom.ToString() },
                { "dateTo", requestDto.DateTo.ToString() }
            });
                try
                {
                    var response = await client.PostAsync(url, content, cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                        //Console.WriteLine("Response Body: " + responseBody);
                        XmlSerializer serializer = new XmlSerializer(typeof(MyMessageResponseDto), "http://tempuri.org/");
                        using (StringReader reader = new StringReader(responseBody))
                        {
                            return (MyMessageResponseDto)serializer.Deserialize(reader);
                        }
                    }
                    else
                    {

                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }

    }

}
