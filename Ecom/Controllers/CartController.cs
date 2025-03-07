﻿using Dapper;
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
                await db.QueryAsync(@"Insert into Cart (UserEmailId, ProductId) 
                    values (@Email, @Pid); Select * from cart where UserEmailId = @Email and Productid = @Pid", new {Email = cartInput.UserEmailId, Pid = cartInput.ProductId});
                string query = "select Products.Id, Products.Name, Products.ProductCode, Products.Stock, Products.Price from Products inner join Cart on Products.Id = Cart.ProductId where Cart.UserEmailId = @email";
                var result = (List<CartProduct>)await db.QueryAsync<CartProduct>(query, new { email = cartInput.UserEmailId });
                return Ok(result);
            }
        }

        [HttpPost("Like")]
        public async Task<ActionResult<List<CartProduct>>> Like([FromBody] CartInputModel likeInput)
        {
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                await conn.QueryAsync(@"Insert into Likes (emailId, ProductId)
                  values (@Email, @Pid);", new { Email = likeInput.UserEmailId, Pid = likeInput.ProductId });
                string query = "select Products.Id, Products.Name, Products.ProductCode, Products.Stock, Products.Price from Products inner join Likes on Products.Id = Likes.ProductId where Likes.emailId = @email";
                var result = (List<CartProduct>)await conn.QueryAsync<CartProduct>(query, new { email = likeInput.UserEmailId });
                return Ok(result);
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

        [HttpGet("GetLikeItems/{emailid}")]
        public async Task<ActionResult<List<CartProduct>>> GetLikeItems(string emailid)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                string query = "select Products.Id, Products.Name, Products.ProductCode, Products.Stock, Products.Price from Products inner join Likes on Products.Id = Likes.ProductId where Likes.emailId = @email";
                var result = (List<CartProduct>)await conn.QueryAsync<CartProduct>(query, new { email = emailid });
                return Ok(result);
            }
        }

        [HttpPost("RemoveFromCart")]
        public async Task<ActionResult<List<CartProduct>>> RemoveFromCart([FromBody] CartInputModel cartInputModel)
        {
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                await conn.QueryAsync("SET ROWCOUNT 1 delete from cart where userEmailId = @EmailId and productId = @Id SET ROWCOUNT 0", new { EmailId = cartInputModel.UserEmailId, Id = cartInputModel.ProductId });
                string query = "select Products.Id, Products.Name, Products.ProductCode, Products.Stock, Products.Price from Products inner join Cart on Products.Id = Cart.ProductId where Cart.UserEmailId = @email";
                var result = (List<CartProduct>)await conn.QueryAsync<CartProduct>(query, new { email = cartInputModel.UserEmailId });
                return Ok(result);
            }
        }

        [HttpPost("RemoveCompleteFromCart")]
        public async Task<ActionResult<List<CartProduct>>> RemoveCompleteFromCart([FromBody] CartInputModel cartInputModel)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                await conn.QueryAsync("delete from cart where userEmailId = @EmailId and productId = @Id", new { EmailId = cartInputModel.UserEmailId, Id = cartInputModel.ProductId });
                string query = "select Products.Id, Products.Name, Products.ProductCode, Products.Stock, Products.Price from Products inner join Cart on Products.Id = Cart.ProductId where Cart.UserEmailId = @email";
                var result = (List<CartProduct>)await conn.QueryAsync<CartProduct>(query, new { email = cartInputModel.UserEmailId });
                return Ok(result);
            }
        }

        [HttpPost("RemoveLike")]
        public async Task<ActionResult<List<CartProduct>>> RemoveLike([FromBody] CartInputModel likeInput)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                await conn.QueryAsync("SET ROWCOUNT 1 delete from Likes where emailid = @EmailId and productId = @Id SET ROWCOUNT 0", new { EmailId = likeInput.UserEmailId, Id = likeInput.ProductId });
                var result = (List<CartProduct>)await conn.QueryAsync<CartProduct>("select Products.Id, Products.Name, Products.ProductCode, Products.Stock, Products.Price from Products inner join Likes on Products.Id = Likes.ProductId where Likes.emailId = @email", new {email = likeInput.UserEmailId});
                return result;
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

        [HttpPost("IsLikedProduct")]
        public async Task<bool> IsLikedProduct(CartInputModel likeInput)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                var LikeID = await conn.QueryFirstOrDefaultAsync<int>("select Id from Likes where productid = @Pid and emailid = @emailid", new { Pid = likeInput.ProductId, emailid = likeInput.UserEmailId });
                return LikeID > 0 ? true : false;
            }
        }
    }
}
