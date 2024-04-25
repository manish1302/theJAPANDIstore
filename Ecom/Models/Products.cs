namespace Ecom.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }
        public decimal? Discount {get; set;}
        public string ProductCode { get; set; }
        public int Category {  get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}
        public int Popularity { get; set; }
        public decimal Rating { get; set; }

    }
}
