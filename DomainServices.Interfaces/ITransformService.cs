using DomainServices.Interfaces.Delegates;
using ECom.Entities.Models;
using ECom.Types.Collect;
using ECom.Types.Orders;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using Entities.Models.Workflow;

namespace DomainServices.Interfaces
{
    public interface ITransformService
    {

        Task<TraceableOrder> ToNewTraceableOrder(
            //ICollectableOrderInfo orderInfo,
            FozzyCollectableOrderInfo orderInfo,
            CatalogSingleItemInfoExtractor catalogSingleItemInfoExtractor,
            //PickerInfoExtractorByBasketId pickerInfoExtractorByBasketId,
            //PickerInfoExtractorByBasketIdAndLagerId pickerInfoExtractorByBasketIdAndLagerId,
            OrderOriginExtractor orderOriginExtractor,
            //OrderOriginExtractorByBasketId orderOriginExtractorByBasketId,
            RowNumCalculator rowNumCalculator,
            LogisticTypeCalculator logisticTypeCalculator, CancellationToken cancellationToken);


        Task<TraceableOrder> FromAcceptedOrder(
            FozzyCollectableOrderInfo orderInfo,
            TraceableOrder currentOrder,
            CatalogSingleItemInfoExtractor catalogSingleItemInfoExtractor,
             RowNumCalculator rowNumCalculator,
            IsFilledCalculator isFilledCalculator,
            LogisticTypeCalculator logisticTypeCalculator, CancellationToken cancellationToken);


        OrderData ToFozzyOrder(TraceableOrder order);

        List<AcceptedCollectedItem> ToCollectingItems(TraceableOrder order);

        //List<NewAcceptedCollectedItem> ToNewCollectingItems(TraceableOrder order)

        //Task<TraceableOrder> ToOrderByMergeAcceptedLines(TraceableOrder orderItemsTobeUpdated, 
        //    IList<AcceptedCollectedItem> items, CancellationToken cancellationToken)

        //Task<TraceableOrder> ToOrderByMergeNewAcceptedLines(TraceableOrder orderItemsTobeUpdated, 
        //    IList<NewAcceptedCollectedItem> items, CancellationToken cancellationToken)

        List<BasketCollectedItem> ToBasketCollectingItems(TraceableOrder order);

        //List<BasketCollectedItem> ToBasketCollectingItems(FozzyCollectableOrderInfo orderInfo);


        TraceableOrderItem FromCatalogItem(TraceableOrder order, CatalogItem catalogItem);

        string GetLogisticType(IEnumerable<CatalogInfo> catalogInfos);

        Task<TraceableOrder> FromFozzyOrder(
            TraceableOrder currentTraceableOrder,
            Picker currentPicker,
            OrderData orderData,
            CatalogSingleItemInfoExtractor catalogSingleItemInfoExtractor,
            PickerInfoExtractorByInn pickerInfoExtractorByInn,
            IsFilledCalculator isFilledCalculator,
            RowNumCalculator rowNumCalculator,
            LogisticTypeCalculator logisticTypeCalculator, CancellationToken cancellationToken);

        bool ItemIsFilled(TraceableOrder order, TraceableOrderItem item);

        Task<TraceableOrderItem> CreateNewFromCurrentMinimal(int lagerId,
            TraceableOrderItem template, IDateTimeService dateTimeService, CancellationToken cancellationToken);

    }
}