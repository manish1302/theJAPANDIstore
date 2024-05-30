using Ecom.Models;

namespace Ecom.Data
{
    public class OrderViewModel
    {
        public string EmailId { get; set; }
        public List<OrderItems> OrderItems { get; set; }
        public OrderStatus Status { get; set; }
    }
}
