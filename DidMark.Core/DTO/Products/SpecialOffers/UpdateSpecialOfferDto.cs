namespace DidMark.Core.DTO.Products.SpecialOffers
{
    public class UpdateSpecialOfferDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
}
