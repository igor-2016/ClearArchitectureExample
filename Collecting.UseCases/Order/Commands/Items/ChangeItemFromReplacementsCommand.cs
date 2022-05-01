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
    public class ChangeItemFromReplacementsCommand : IRequest<TraceableOrder>
    {
        public TraceableOrder Order { get; set; }
        public IChangeItemCollectReplacementRequest Request { get; set; }
    }

    public class ChangeItemFromReplacementsCommandHandler : IRequestHandler<ChangeItemFromReplacementsCommand, TraceableOrder>
    {
        private readonly ILogger _logger;

        public ChangeItemFromReplacementsCommandHandler(
            ILogger<ChangeItemFromReplacementsCommandHandler> logger
            )
        {
            _logger = logger;
        }

        public Task<TraceableOrder> Handle(ChangeItemFromReplacementsCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            using (_logger.LoggingScope(order.BasketId))
            {
                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.ChangeItemFromReplacementsCommandName, data);

                var item = order.Items.First(x => x.Id == data.ItemId);

                var changedReplacements = data.Replacements.EmptyIfNull();


                if (!changedReplacements.Any())
                {
                    item.ReplacementLagers = "";
                }
                else
                {
                    item.ReplacementLagers = changedReplacements.ToCommaString();
                }

                _logger.LogInformation("{msg} {data}", FozzyConsts.CommandInfo.ChangeItemFromReplacementsCommandName + " After", 
                    item.ReplacementLagers);

                return Task.FromResult(order);
            }
        }
    }
}
