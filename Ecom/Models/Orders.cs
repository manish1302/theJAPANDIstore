namespace Ecom.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public string UserEmailId { get; set; } = string.Empty;
        public List<OrderItems> OrderItems { get; set; }
        public OrderStatus status { get; set; }
    }

    public class OrderItems
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int quantity { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }
}
