using Ecom.Models;

namespace Ecom.Data
{
    public class OrderProducts
    {
        public int OrderId { get; set; }
        public List<Products> Products{ get; set; }
        public int Status { get; set; }
    }
}
