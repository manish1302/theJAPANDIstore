using Dapper;
using Ecom.Data;
using Ecom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
        [AllowAnonymous]
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
        public async Task<ActionResult<List<Products>>> GetCartItems(string emailid)
        {
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                string query = "select * from Products inner join Cart on Products.Id = Cart.ProductId where Cart.UserEmailId = @email";
                var result = (List<Products>)await conn.QueryAsync<Products>(query, new {email = emailid});
                return Ok(result);
            }

        }
    }
}
