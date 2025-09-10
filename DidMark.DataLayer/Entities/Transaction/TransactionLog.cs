using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DidMark.DataLayer.Entities.Transaction
{
    public class TransactionLog : BaseEntity
    {
        #region properties

        public long UserId { get; set; }
        public decimal PaymentAmount { get; set; }
        public Guid PaymentLinkId { get; set; }
        public long? RefId { get; set; }
        public string? Authority { get; set; }
        public string? CardPan { get; set; }
        public string? PaymentErrorMessage { get; set; }
        public string? PaymentGateway { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionFor TransactionFor { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        #endregion

        #region Relations
        public virtual User User { get; set; }
        #endregion

    }
    public enum TransactionFor : short
    {
        CourseOrder = 1,
    }

    public enum TransactionStatus : short
    {
        Pending = 0,
        PaymentSuccess = 1,
        PaymentError = 2,
        CancelPayment = 3
    }
    public enum PaymentGateway
    {
        ZarinPal
    }
}
