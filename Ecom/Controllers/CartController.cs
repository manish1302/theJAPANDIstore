using Dapper;
using Ecom.Data;
using Ecom.Models;
using MassTransit.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

namespace Ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CartController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("AddToCart")]
        public async Task<ActionResult<Cart>> AddToCart([FromBody] CartInputModel cartInput)
        {
            using(var db = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await db.OpenAsync();
                Cart cart = await db.QueryFirstOrDefaultAsync<Cart>(@"Insert into Cart (UserEmailId, ProductId) 
                    values (@Email, @Pid); Select * from cart where UserEmailId = @Email and Productid = @Pid", new {Email = cartInput.UserEmailId, Pid = cartInput.ProductId});

                return Ok(cart);
            }
        }

        [HttpGet("GetCartItems/{emailid}")]
        public async Task<ActionResult<List<CartProduct>>> GetCartItems(string emailid)
        {
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                string query = "select Products.Id, Products.Name, Products.ProductCode, Products.Stock, Products.Price from Products inner join Cart on Products.Id = Cart.ProductId where Cart.UserEmailId = @email";
                var result = (List<CartProduct>)await conn.QueryAsync<CartProduct>(query, new {email = emailid});
                return Ok(result);
            }

        }

        [HttpPost("RemoveFromCart")]
        public async void RemoveFromCart([FromBody] CartInputModel cartInputModel)
        {
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                await conn.QueryAsync("SET ROWCOUNT 1 delete from cart where userEmailId = @EmailId and productId = @Id SET ROWCOUNT 0", new { EmailId = cartInputModel.UserEmailId, Id = cartInputModel.ProductId });
            }
        }

        [HttpGet("ClearCart/{emailId}")]
        public async Task<bool> ClearCart(string emailId)
        {   
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                try
                {
                    await conn.OpenAsync();
                    await conn.QueryAsync("delete from Cart where UserEmailId = @emailId", new { emailId = emailId });
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
