using MediatR;
using Microsoft.Extensions.Logging;

namespace Collecting.UseCases
{
    public class StartCollectingCommandHandlerBase : CollectingCommandHandlerBase
    {

        public StartCollectingCommandHandlerBase( IMediator mediator, ILogger logger) : 
            base(mediator, logger)
        {
           
        }

       

    }
}
