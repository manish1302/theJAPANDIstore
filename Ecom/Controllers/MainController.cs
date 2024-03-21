using Ecom.Commands;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        public readonly IRequestClient<GetAllProductsCommand> _getAllProductsClient;

        MainController (IRequestClient<GetAllProductsCommand> getAllProductsClient)
        {
            _getAllProductsClient = getAllProductsClient;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _getAllProductsClient.GetResponse<GetAllProductsResult>(new { CorrelationId = new Guid().ToString(), UserId = 1 });
            return Ok(result.Message);
        }
    }
}
