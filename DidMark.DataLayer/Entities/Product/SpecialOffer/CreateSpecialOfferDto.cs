namespace DidMark.Core.DTO.Products
{
    public class CreateSpecialOfferDto
    {
        public string Title { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
    }
}
