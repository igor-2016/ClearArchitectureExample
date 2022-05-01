using Collecting.Interfaces;
using Entities.Models;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class LookupOrderByOrderNumberCommand : IRequest<TraceableOrder>
    {
        public int ExternalOrderNumber { get; set; }
    }

    public class LookupOrderByOrderNumberCommandHandler : IRequestHandler<LookupOrderByOrderNumberCommand, TraceableOrder>
    {
        private readonly ILogger<LookupOrderByOrderNumberCommandHandler> _logger;
        private readonly ICollectingService _collectingService;


        public LookupOrderByOrderNumberCommandHandler(
            ICollectingService collectingService,
            ILogger<LookupOrderByOrderNumberCommandHandler> logger
            )
        {
            _logger = logger;
            _collectingService = collectingService;
        }

        public async Task<TraceableOrder> Handle(LookupOrderByOrderNumberCommand request, CancellationToken cancellationToken)
        {
            var externalOrderId = request.ExternalOrderNumber.ToString();

            _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.LookupOrderByOrderIdCommandName, externalOrderId);

            return await _collectingService.GetOrderByOrderNumber(externalOrderId, cancellationToken);
        }
    }
}
