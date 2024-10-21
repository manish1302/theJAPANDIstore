using Dapper;
using Ecom.Commands;
using Ecom.Data;
using Ecom.Models;
using Ecom.ViewModels;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace Ecom.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MainController : ControllerBase
    {
        private readonly IConfiguration _config;

        public MainController (IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Products>>> GetAllProducts()
        {
            List<Products> products;
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                products = (List<Products>)await conn.QueryAsync<Products>("select * from Products");
            }
            return Ok(products);
        }

        [HttpGet("[action]/{Id}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Products>>> GetProductsBySearchCriteria(int Id)
        {
            List<Products> products;
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                products = (List<Products>)await conn.QueryAsync<Products>("select * from Products where Category = @CategoryId", new { CategoryId = Id});
            }
            return Ok(products);
        }

        [HttpGet("[action]/{Id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Products>> GetProductById(int Id)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                var result = await conn.QueryFirstOrDefaultAsync<Products>("select * from products where id = @Id", new { Id = Id });
                return Ok(result);
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<Products>> GetDeals()
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                var result = (List<Products>)await conn.QueryAsync<Products>("select * from products where discount != @Id", new { Id = 0 });
                return Ok(result);
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<Products>> GetLatest()
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                var result = (List<Products>)await conn.QueryAsync<Products>("SELECT TOP 10 * FROM Products ORDER BY Id DESC");
                return Ok(result);
            }
        }
    }

}
