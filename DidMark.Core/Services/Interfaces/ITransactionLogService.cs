using DidMark.Core.DTO.Orders;
using DidMark.Core.DTO.Payment;
using DidMark.Core.DTO.Products;
using DidMark.Core.DTO.TransactionLog;
using DidMark.DataLayer.Entities.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface ITransactionLogService
    {
        Task<long> CreateTransaction(CreateTransaction command);
        Task PaymentSuccess(TransactionPaymentSuccess command);
        Task PaymentError(TransactionPaymentError command);
        Task<TransactionLog> GetTransactionById(long transactionId);
        Task<FilterTransactionsDTO> GetTransactionsByFilter(FilterTransactionsDTO filter);
        Task<int> GetCancelTransactionsCount(CancelTransactionFilterDto filter);
    }
}
