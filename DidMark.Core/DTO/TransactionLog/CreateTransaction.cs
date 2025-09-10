
using DidMark.DataLayer.Entities.Transaction;

namespace DidMark.Core.DTO.Payment
{
    public class CreateTransaction
    {
        public long UserId { get; set; }
        public decimal PaymentAmount { get; set; }
        public long LinkId { get; set; }
        public PaymentGateway PaymentGateway { get; set; }
        public TransactionFor TransactionFor { get; set; }
    }
}