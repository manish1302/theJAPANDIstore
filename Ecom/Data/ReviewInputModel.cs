namespace Ecom.Data
{
    public class ReviewInputModel
    {
        public string EmailId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal? Rating { get; set; }
        public string? Review { get; set; }
        public int? ReviewID { get; set; }
    }
}
