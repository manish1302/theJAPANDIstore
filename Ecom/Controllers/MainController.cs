using Dapper;
using Ecom.Commands;
using Ecom.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace Ecom.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        //public readonly IRequestClient<GetAllProductsCommand> _getAllProductsClient;
        private readonly IConfiguration _config;
        //private readonly IRequestClient<GetAllProductsCommand> _getAllProductsClient;

        public MainController (IConfiguration config)
        {
            //_getAllProductsClient = getAllProductsClient;
            _config = config;
            //_getAllProductsClient = getAllProductsClient;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString"));
            //var result = await _getAllProductsClient.GetResponse<GetAllProductsResult>(new { CorrelationId = new Guid().ToString(), UserId = 1 });
            var products = await conn.QueryAsync<Products>("select * from Products");
            //var result = await _getAllProductsClient.RespondAsync<GetAllProductsResult>(new
            //{
            //    isSuccessful = true,
            //    CorrelationId = new Guid().ToString(),
            //    UserId = 1,
            //    Products = products
            //});
            return Ok(products);
        }
    }
}
