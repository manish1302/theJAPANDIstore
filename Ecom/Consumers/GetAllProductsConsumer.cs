//using Dapper;
//using Ecom.Commands;
//using Ecom.Models;
//using MassTransit;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;

//namespace Ecom.Consumers
//{
//    public class GetAllProductsConsumer : IConsumer<GetAllProductsCommand>
//    {
//        private IConfiguration _config;
//        public GetAllProductsConsumer(IConfiguration config)
//        {
//            _config = config;
//        }
//        public async Task Consume(ConsumeContext<GetAllProductsCommand> context)
//        {
//            try
//            {
//                List<Product> products;
//                using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
//                {
//                    await conn.OpenAsync();
//                    products = (List<Product>)await conn.QueryAsync<Products>("select * from Products");
//                }
                    
//                await context.RespondAsync<GetAllProductsResult>(new
//                {
//                    isSuccessful = true,
//                    UserId = context.Message.UserId,
//                    CorrelationId = context.Message.CorrelationId,
//                    Products = products
//                });
//            }
//            catch(Exception ex)
//            {
//                throw ex;
//            }
//        }
//    }
//}


//using Dapper;
//using Ecom.Commands;
//using Ecom.Models;
//using MassTransit;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using System;

//namespace Ecom.Controllers
//{

//    [Route("[controller]")]
//    [ApiController]
//    public class MainController : ControllerBase
//    {
//        private readonly IConfiguration _config;
//        private readonly IRequestClient<GetAllProductsCommand> _getAllProductsClient;

//        public MainController(IConfiguration config, IRequestClient<GetAllProductsCommand> getAllProductsClient)
//        {
//            _config = config;
//            _getAllProductsClient = getAllProductsClient;
//        }

//        [HttpGet("[action]")]
//        public async Task<ActionResult<List<Product>>> GetAllProducts()
//        {
//            //using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString"));
//            var result = await _getAllProductsClient.GetResponse<GetAllProductsResult>(new { CorrelationId = new Guid().ToString(), UserId = 1 });
//            //var products = await conn.QueryAsync<Products>("select * from Products");
//            return Ok(result);
//        }
//    }
//}
