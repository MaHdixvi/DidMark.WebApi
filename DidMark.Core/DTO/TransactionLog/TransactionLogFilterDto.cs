using DidMark.Core.DTO.Orders;
using DidMark.Core.DTO.Paging;
using DidMark.Core.DTO.Products.ProductCategory;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Entities.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DidMark.Core.DTO.Products
{
    public class FilterTransactionsDTO : BasePaging
    {
        public long? UserId { get; set; }
        public TransactionStatus? Status { get; set; }
        public TransactionFor? TransactionFor { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<TransactionLogDto>? Transactions { get; set; }
        public TransactionOrderBy OrderBy { get; set; }
        public decimal TotalPayment { get; set; }

        public FilterTransactionsDTO SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.ActivePage = paging.ActivePage;
            this.EndPage = paging.EndPage;
            this.PageCount = paging.PageCount;
            this.StartPage = paging.StartPage;
            this.TakeEntity = paging.TakeEntity;
            this.SkipEntity = paging.SkipEntity;
            return this;
        }

        public FilterTransactionsDTO SetTransactions(List<DidMark.DataLayer.Entities.Transaction.TransactionLog> transactions)
        {
            this.Transactions = transactions?.Select(t => new TransactionLogDto
            {
                Id = t.Id,
                UserId = t.UserId,
                UserFullName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}" : string.Empty,
                PaymentAmount = t.PaymentAmount,
                PaymentLinkId = t.PaymentLinkId,
                RefId = t.RefId,
                Authority = t.Authority,
                CardPan = t.CardPan,
                PaymentErrorMessage = t.PaymentErrorMessage,
                PaymentGateWay = t.PaymentGateway,
                Status = t.Status,
                TransactionFor = t.TransactionFor,
                CreateDate = t.CreateDate,
                PaymentDate = t.PaymentDate

            }).ToList();

            this.TotalPayment = transactions?.Where(t => t.Status == TransactionStatus.PaymentSuccess)
                                             .Sum(t => t.PaymentAmount) ?? 0;

            return this;
        }

        public enum TransactionOrderBy
        {
            AmountAsc,
            AmountDes,
            CreateDateAsc,
            CreateDateDes,
            Status,
        }
    }
}