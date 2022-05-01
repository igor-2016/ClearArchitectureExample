using ApplicationServices.Interfaces.Requests;
using ECom.Types.Extensions;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class RemoveItemFromReplacementsCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder Order { get; set; }
        public IRemoveItemCollectReplacementRequest Request { get; set; }
    }

    public class RemoveItemFromReplacementsCommandHandler : IRequestHandler<RemoveItemFromReplacementsCommand, TraceableOrder>
    {
        private readonly ILogger _logger;

        public RemoveItemFromReplacementsCommandHandler(
            ILogger<ChangeItemFromReplacementsCommandHandler> logger
            )
        {
            _logger = logger;
        }

        public Task<TraceableOrder> Handle(RemoveItemFromReplacementsCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            using (_logger.LoggingScope(order.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.RemoveItemFromReplacementsCommandName, data);

                var item = order.Items.First(x => x.Id == data.ItemId);

                if (item.ReplacementLagers.EmptyIfNull().Any())
                {
                    item.ReplacementLagers = "";
                }

                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.RemoveItemFromReplacementsCommandName + " After",
                    item.ReplacementLagers);

                return Task.FromResult(order);
            }
        }
    }
}
