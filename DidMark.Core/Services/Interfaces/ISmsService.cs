using DidMark.Core.DTO.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendRegistrationSmsAsync(string phoneNumber, string firstName, string lastName, string nationalCode, string collectionName, string userPhoneNumber);
        Task<bool> SendActivationCodeSmsAsync(string phoneNumber, string activationCode);
        Task<bool> SendActivatedAccountSmsAsync(string phoneNumber);
        Task<bool> SendActivatedEmailSmsAsync(string phoneNumber);
        Task<bool> WaitForActivationAsync(string userName, DateTime dateTime);
        Task<bool> SendOrderSummarySmsAsync(string phoneNumber, List<OrderBasketDetail> items, decimal totalPrice);
        Task<bool> SendForgotPasswordCodeSmsAsync(string phoneNumber, string resetCode);
        Task<bool> SendCustomSmsAsync(string phoneNumber, string message);


    }
}
