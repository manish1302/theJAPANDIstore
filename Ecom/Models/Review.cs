namespace Ecom.Models
{
    public class Reviews
    {
        public int Id { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; } = string.Empty;
        public string Name {  get; set; } = string.Empty;
    }
}
