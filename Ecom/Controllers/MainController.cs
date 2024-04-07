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
    }

}
