using Dapper;
using Ecom.Commands;
using Ecom.Models;
using MassTransit;
using Microsoft.Data.SqlClient;

namespace Ecom.Consumers
{
    public class GetAllProductsConsumer : IConsumer<GetAllProductsCommand>
    {
        private ILogger<GetAllProductsConsumer> _logger;
        private IConfiguration _config;
        public GetAllProductsConsumer(ILogger<GetAllProductsConsumer> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task Consume(ConsumeContext<GetAllProductsCommand> context)
        {
            try
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString"));
                var products = await conn.QueryAsync<Products>("select * from Products");
                await context.RespondAsync<GetAllProductsResult>(new
                {
                    isSuccessful = true,
                    UserId = context.Message.UserId,
                    CorrelationId = context.Message.CorrelationId,
                    Products = products
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
