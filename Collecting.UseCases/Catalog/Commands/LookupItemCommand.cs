using ApplicationServices.Interfaces.Requests;
using Catalog.Interfaces;
using Collecting.Interfaces;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Types.Exceptions;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class LookupItemCommand : IRequest<List<TraceableOrderItem>>
    {
        public Guid BasketGuid { get; set; }

        public TraceableOrder Order { get; set; }
        public ILookupItemRequest Request { get; set; }
    }

    public class LookupItemCommandHandler : IRequestHandler<LookupItemCommand, List<TraceableOrderItem>>
    {
        private readonly ILogger _logger;
        private readonly ICatalogService _catalogService;
        private readonly IMediator _mediator;
        private readonly ITransformService _transformService;

        private readonly IEntityMappingService _entityMappingService;

        public LookupItemCommandHandler(
            ILogger<LookupItemCommandHandler> logger,
            ICatalogService catalogService,
            ITransformService transformService,
            IEntityMappingService entityMappingService,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _catalogService = catalogService;
            _entityMappingService = entityMappingService;
            _transformService = transformService;
        }

      
        public async Task<List<TraceableOrderItem>> Handle(LookupItemCommand request, CancellationToken cancellationToken)
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
                        throw new CollectException(CollectErrors.OrderNotFound, "Ордер не найден!");

                    
                    var catalogItem = await _catalogService.GetCatalogItem(order, data.LagerId, cancellationToken);

                    var items = await _mediator.Send(new ExtractItemsCommand()
                    {
                        Order = order,
                        CatalogItem = catalogItem,
                    });

                    if (items.Count == 0)
                    {
                        if (data.ItemId.HasValue)
                            throw new CollectException(CollectErrors.SkuNotFoundInProductSet, "Артикул отсутствует в наборе");

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
