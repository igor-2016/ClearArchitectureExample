using ApplicationServices.Interfaces.Requests;
using Catalog.Interfaces;
using DomainServices.Interfaces;
using ECom.Types.Exceptions;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Utils;
using Utils.Attributes;
using Workflow.Interfaces.Exceptions;

namespace Workflow.UseCases
{
    [ApmTrace]
    public class LookupEcomItemCommand : IRequest<List<TraceableOrderItem>>
    {
        public Guid BasketGuid { get; set; }

        public TraceableOrder Order { get; set; }
        public ILookupItemRequest Request { get; set; }
    }

    public class LookupEcomItemCommandHandler : IRequestHandler<LookupEcomItemCommand, List<TraceableOrderItem>>
    {
        private readonly ILogger _logger;
        private readonly ICatalogService _catalogService;
        private readonly IMediator _mediator;
        private readonly ITransformService _transformService;

     
        public LookupEcomItemCommandHandler(
            ILogger<LookupEcomItemCommandHandler> logger,
            ICatalogService catalogService,
            ITransformService transformService,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _catalogService = catalogService;
            _transformService = transformService;
        }

      
        public async Task<List<TraceableOrderItem>> Handle(LookupEcomItemCommand request, CancellationToken cancellationToken)
        {
            var data = request.Request;
            var order = request.Order;

            using (_logger.LoggingScope(request.BasketGuid))
            {
                _logger.LogInformation("{msg} {data}", "Lookup item", data);

                Stopwatch sw = Stopwatch.StartNew();
                try
                {

                    if (order == null)
                        throw new WorkflowException(WorkflowErrors.OrderNotFound, "Ордер не найден!");


                    if (!data.ItemId.HasValue)
                        throw new WorkflowException(WorkflowErrors.SkuNotFoundEComItemId, "Ecom Id не указан");


                    var items = await _mediator.Send(new ExtractItemsByEcomIdCommand()
                    {
                        Order = order,
                        EComItemId = request.Request.ItemId.Value,
                    });

                    if (items.Count == 0)
                    {
                        var catalogItem = await _catalogService.GetCatalogItem(order, data.LagerId);


                        // создаём новый, если не найдено в корзине                      
                        var newItem = _transformService.FromCatalogItem(order, catalogItem);
                        items.Add(newItem);
                    }

                    return items;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{msg} {error} {StackTrace}", "Error occurred while LookupItem", ex.FullMessage(), ex.StackTrace);
                    throw;
                }
                finally
                {
                    sw.Stop();
                }
            }
        }
    }
}
