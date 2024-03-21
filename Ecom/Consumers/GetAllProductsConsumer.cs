using Ecom.Commands;
using MassTransit;

namespace Ecom.Consumers
{
    public class GetAllProductsConsumer : IConsumer<GetAllProductsCommand>
    {
        public async Task Consume(ConsumeContext<GetAllProductsCommand> context)
        {
            
        }
    }
}
