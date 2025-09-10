using DidMark.Core.DTO.Orders;
using DidMark.Core.DTO.Products;
using DidMark.DataLayer.Entities.Transaction;
using DidMark.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DidMark.Core.DTO.Payment;
using DidMark.DataLayer.Repository;
using DidMark.Core.DTO.TransactionLog;

namespace DidMark.Core.Services.Implementations
{
    public class TransactionLogService : ITransactionLogService
    {
        private readonly IGenericRepository<TransactionLog> _transactionRepository;

        public TransactionLogService(IGenericRepository<TransactionLog> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        #region Create Transaction

        public async Task<long> CreateTransaction(CreateTransaction command)
        {
            var transaction = new TransactionLog
            {
                UserId = command.UserId,
                PaymentAmount = command.PaymentAmount,
                PaymentLinkId = Guid.NewGuid(),
                TransactionFor = command.TransactionFor,
                PaymentGateway = command.PaymentGateway.ToString(),
                Status = TransactionStatus.Pending,
                CreateDate = DateTime.UtcNow
            };

            await _transactionRepository.AddEntity(transaction);
            await _transactionRepository.SaveChanges();
            return transaction.Id;
        }

        #endregion

        #region Payment Success & Error

        public async Task PaymentSuccess(TransactionPaymentSuccess command)
        {
            var transaction = await GetTransactionById(command.TransactionId);
            if (transaction == null) throw new Exception("Transaction not found");

            transaction.Status = TransactionStatus.PaymentSuccess;
            transaction.RefId = command.RefId;
            transaction.CardPan = command.CardPen;
            transaction.Authority = command.Authority;
            transaction.PaymentDate = DateTime.UtcNow;

            _transactionRepository.UpdateEntity(transaction);
            await _transactionRepository.SaveChanges();
        }

        public async Task PaymentError(TransactionPaymentError command)
        {
            var transaction = await GetTransactionById(command.TransactionId);
            if (transaction == null) throw new Exception("Transaction not found");

            transaction.Status = command.Canceled ? TransactionStatus.CancelPayment : TransactionStatus.PaymentError;
            transaction.RefId = command.RefId;
            transaction.Authority = command.Authority;
            transaction.PaymentErrorMessage = command.ErrorMessage;
            transaction.PaymentDate = DateTime.UtcNow;

            _transactionRepository.UpdateEntity(transaction);
            await _transactionRepository.SaveChanges();
        }

        #endregion

        #region Get Transaction / Filter

        public async Task<TransactionLog> GetTransactionById(long transactionId)
        {
            return await _transactionRepository.GetEntitiesQuery()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<FilterTransactionsDTO> GetTransactionsByFilter(FilterTransactionsDTO filter)
        {
            var query = _transactionRepository.GetEntitiesQuery()
                .Include(t => t.User)
                .AsQueryable();

            if (filter.UserId.HasValue)
                query = query.Where(t => t.UserId == filter.UserId.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.TransactionFor.HasValue)
                query = query.Where(t => t.TransactionFor == filter.TransactionFor.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(t => t.CreateDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.CreateDate <= filter.EndDate.Value);

            // Paging
            var skip = (filter.PageId - 1) * filter.TakeEntity;
            var transactions = await query
                .OrderByDescending(t => t.CreateDate)
                .Skip(skip)
                .Take(filter.TakeEntity)
                .ToListAsync();

            return filter.SetTransactions(transactions);
        }

        #endregion

        #region Cancel Transactions Count

        public async Task<int> GetCancelTransactionsCount(CancelTransactionFilterDto filter)
        {
            var query = _transactionRepository.GetEntitiesQuery()
                .Where(t => t.Status == TransactionStatus.PaymentError);

            if (filter.TransactionFor.HasValue)
                query = query.Where(t => t.TransactionFor == filter.TransactionFor.Value);

            if (filter.Status.HasValue && filter.Status != TransactionStatus.Pending)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (!string.IsNullOrEmpty(filter.StartDate))
            {
                var startDate = DateTime.Parse(filter.StartDate);
                query = query.Where(t => (t.PaymentDate != null && t.PaymentDate >= startDate) || t.CreateDate >= startDate);
            }

            if (!string.IsNullOrEmpty(filter.EndDate))
            {
                var endDate = DateTime.Parse(filter.EndDate);
                query = query.Where(t => (t.PaymentDate != null && t.PaymentDate <= endDate) || t.CreateDate <= endDate);
            }

            return await query.CountAsync();
        }


        #endregion
    }
}
