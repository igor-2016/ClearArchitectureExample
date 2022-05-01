using Catalog.Interfaces;
using Catalog.Interfaces.Dto.Requests;
using Catalog.Interfaces.Exceptions;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Types.Delivery;
using Entities;
using Entities.Models.Expansion;
using Utils.Consts;
using Utils.Extensions;

namespace Catalog.Ecom
{
    public class EComCatalogService : ICatalogService
    {

        private readonly ICatalogApiClient _catalogApiClient;
        private readonly IEntityMappingService _entityMappingService;
        public EComCatalogService(ICatalogApiClient catalogApiClient,
            IEntityMappingService entityMappingService)
        {
            _catalogApiClient = catalogApiClient;
            _entityMappingService = entityMappingService;
        }

        public virtual async Task<CatalogInfo> GetCatalogItems(int lagerId, int filialId, int merchantId, CancellationToken cancellationToken, 
            int ecomDeliveryType = 0, Guid? basketGuid = null)
        {
            try
            {
                //TODO ПОКА НАДО ПЕРЕДАВАТЬ  ecomDeliveryType  = 0!
                if (ecomDeliveryType != 0)
                    ecomDeliveryType = 0;

                var request = CatalogItemsRequest.GetSimpleRequest(lagerId, filialId, merchantId,
                    (DeliveryType)ecomDeliveryType, basketGuid);
                var response = await _catalogApiClient.GetCatalogItems(request, cancellationToken);

                //if(response?.EComError != null)
                //    throw new CatalogException(CatalogErrors.CatalogServiceError, 
                //        response.EComError.ErrorMessage, $"{response.EComError.ErrorDescription}, {response.EComError.ErrorCode}");

                return response.GetSimpleCatalogItemsResponse(request);
            }
            catch (DescribedException ex)
            {
                throw ex.AddRequestedTarget(CommonConsts.Subsystem.EComCatalog);
            }
        }

        public virtual async Task<CatalogItem> GetCatalogItem(TraceableOrder order, int? lagerId, CancellationToken cancellationToken)
        {
            if (!lagerId.HasValue)
                throw new CatalogException(CatalogErrors.BarcodeOrLagerIdRequired, "LagerId не найден!");

            CatalogInfo? catalogInfo;

            try
            {
                var ecomDeliveryTypeId =
                    _entityMappingService.GetDeliveryTypeFromFozzyToEcom(order.DeliveryId) ??
                        throw new CatalogException(CatalogErrors.CannotMapDeliveryTypeFromFozzyToEcom, $"{order.DeliveryId}");


                catalogInfo = await GetCatalogItems(lagerId.Value,
                    order.FilialId, order.MerchantId, cancellationToken, ecomDeliveryTypeId, order.BasketId);

            }
            catch (Exception ex)
            {
                throw new CatalogException(CatalogErrors.SKUNotFound, ex.Message);
            }


            if (catalogInfo == null)
                throw new CatalogException(CatalogErrors.SKUNotFound, "Товар не найден");

            var catalogItems = catalogInfo.Items;

            if (catalogItems.Count == 0)
                throw new CatalogException(CatalogErrors.SKUNotFound, "Товар не найден");

            if (catalogItems.Count > 1)
                throw new CatalogException(CatalogErrors.MultipleSKUFound, "найдено более 1 позиции товара");


            return catalogInfo.GetSingleItem();
        }


    }
}
