
using DidMark.DataLayer.Entities.Transaction;

namespace DidMark.Core.DTO.Orders
{
    public class TransactionLogDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserFullName { get; set; }
        public decimal PaymentAmount { get; set; }
        public Guid PaymentLinkId { get; set; }
        public long? RefId { get; set; }
        public string Authority { get; set; }
        public string CardPan { get; set; }
        public string PaymentErrorMessage { get; set; }
        public string PaymentGateWay { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionFor TransactionFor { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}