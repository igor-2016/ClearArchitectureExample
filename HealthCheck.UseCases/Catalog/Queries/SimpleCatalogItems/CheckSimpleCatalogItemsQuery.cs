using ApplicationServices.Interfaces.Responses;
using Catalog.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Interfaces.Dto;
using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using Entities.Models.Expansion;
using MediatR;

namespace HealthCheck.UseCases.Catalog.Queries.SimpleCatalogItems
{

    public class CheckSimpleCatalogItemsQuery : IRequest<DataResult<CatalogInfo>>
    {
        public int LagerId { get; set; } 
        public int Filial { get; set; } 
        public Merchant Merchant { get; set; } 
        public DeliveryType EComDeliveryType { get; set; } 
    }

    public class CheckSimpleCatalogItemsQueryHandler : IRequestHandler<CheckSimpleCatalogItemsQuery, DataResult<CatalogInfo>>
    {
        private Guid? NO_BASKET = null;

        private readonly ICatalogService _catalogService;
        public CheckSimpleCatalogItemsQueryHandler(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<DataResult<CatalogInfo>> Handle(CheckSimpleCatalogItemsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var items = await _catalogService.GetCatalogItems(
                     request.LagerId,
                     request.Filial,
                     (int)request.Merchant, 
                     cancellationToken,
                     (int)request.EComDeliveryType,
                     NO_BASKET
                     );

                return DataResult<CatalogInfo>.Success(items);
            }
            catch (Exception ex)
            {
                return DataResult<CatalogInfo>.Error(ex);  
            }
        }

        protected async Task<bool> PingDatabase(IReadOnlyDataAccess dataAccess, CancellationToken cancellationToken)
        {
            bool databaseIsAvailable;
            try
            {
                await dataAccess.GetByIdWithoutIncludes<TraceableOrder, TraceableOrderDto, Guid>(Guid.NewGuid(), cancellationToken);
                databaseIsAvailable = true;
            }
            catch (DataAccessException dbEx) when (dbEx.ErrorCode == (int)DbErrors.EntityNotFound)
            {
                databaseIsAvailable = true;
            }
            return databaseIsAvailable;
        }
    }
}