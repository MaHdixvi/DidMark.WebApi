namespace DidMark.Core.DTO.Orders
{
    public class TransactionPaymentSuccess
    {
        public long TransactionId { get; set; }
        public string Authority { get; set; }
        public long RefId { get; set; }
        public string CardPen { get; set; }
        public string SuccessCallBack { get; set; }
    }
}