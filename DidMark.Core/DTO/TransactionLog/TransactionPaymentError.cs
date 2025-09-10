namespace DidMark.Core.DTO.Orders
{
    public class TransactionPaymentError
    {
        public long TransactionId { get; set; }
        public string ErrorMessage { get; set; }
        public string Authority { get; set; }
        public long RefId { get; set; }
        public bool Canceled { get; set; } = false;
    }

}