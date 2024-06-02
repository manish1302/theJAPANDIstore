using Dapper;
using Ecom.Data;
using Ecom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Ecom.Controllers
{
    [ApiController]
    [Route("/Api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;
        public OrderController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("ConfirmOrder")]
        public async Task<IActionResult> ConfirmOrder([FromBody] OrderViewModel orderViewModel)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                //var ans = await conn.QueryFirstOrDefaultAsync<int>("Insert into Orders (UserEmailId, ProductId, Quantity, OrderDate) values (@emailId, @Id, @quantity, @date); select Id from Orders;", new { emailId = orderViewModel.EmailId, Id = orderViewModel.Pid, quantity = orderViewModel.quantity, date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") });
                //return Ok(ans);
                var OrdId = conn.ExecuteScalar<int>("Insert into Orders (UserEmailId, OrderDate, Status) values" +
                    "(@email, @date, @status); select scope_identity()", new { email = orderViewModel.EmailId, date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), status = orderViewModel.Status });

                foreach (var item in orderViewModel.OrderItems)
                {
                    item.OrderId = OrdId;
                    await conn.ExecuteAsync("Insert into OrderItems (OrderId, ProductId, Quantity) values (@OrderId, @ProductId, @quantity)",new {OrderId = item.OrderId, ProductId = item.ProductId, quantity = item.quantity});
                }

                return Ok(OrdId);
            }
        }

        [HttpGet("GetOrders/{userEmailId}")]
        public async Task<ActionResult<List<OrderProducts>>> GetOrders(string userEmailId)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                //var result = await conn.QueryAsync<Products>("select * from orders where UserEmailId = @email order by OrderDate desc", new {email =  userEmailId});
                //return Ok(result);
                List<OrderProducts> result = new List<OrderProducts>();
                var query1 = "select Id from orders where UserEmailId = @user";
                var OrdIdList = await conn.QueryAsync<int>(query1, new { user = userEmailId });
                foreach(var item in OrdIdList)
                {
                    OrderProducts Order = new OrderProducts();
                    List<Products> OrderProds = new List<Products>();
                    Order.OrderId = item;
                    var OrderByIdList = await conn.QueryAsync<OrderItems>("select * from OrderItems where OrderId = @Id", new { Id = item });
                    foreach(var product in OrderByIdList)
                    {
                        var ProductDetails = await conn.QueryFirstOrDefaultAsync<Products>("Select * from products where Id = @Pid", new { Pid = product.ProductId });
                        ProductDetails.Stock = product.quantity;
                        OrderProds.Add(ProductDetails);
                    }
                    Order.Products = OrderProds;    
                    result.Add(Order);
                }
                return Ok(result);
            }
        }
    }
}
