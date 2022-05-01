using ApplicationServices.Interfaces.Enums;
using Catalog.Interfaces;
using Collecting.Interfaces;
using Collecting.Interfaces.Enums;
using Collecting.Interfaces.Requests;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;
using Utils;
using Utils.Attributes;

namespace Collecting.UseCases
{
    [ApmTrace]
    public class AllChangesCommand : IRequest<Unit>
    {
        public ICollectingChangeRequest Request { get; set; }
    }

    public class AllChangesCommandHandler : CollectingCommandHandlerBase,
        IRequestHandler<AllChangesCommand, Unit>
    {

        private readonly ITransformService _transformService;

        private readonly ICollectingService _collectingService;
        private readonly ICatalogService _catalogService;

        public AllChangesCommandHandler(
            ILogger<AllChangesCommandHandler> logger,
            IMediator mediator,
            ITransformService transformService,
            ICollectingService collectingService,
            ICatalogService catalogService
            ) : base (mediator, logger)
        {
            _transformService = transformService;
            _collectingService = collectingService;
            _catalogService = catalogService;
        }

        
        #region process delegates

        private async Task<CatalogInfo> GetCatalogSingleItemInfo(int lagerId, int filialId, Merchant merchant,
            CancellationToken cancellationToken, DeliveryType ecomDeliveryType = DeliveryType.Unknown, Guid? basketGuid = null)
        {
            return await _catalogService.GetCatalogItems(lagerId, filialId, (int)merchant, cancellationToken, 
                (int)ecomDeliveryType, basketGuid);
        }

        private async Task<Picker> PickerInfoExtractorByInn(string inn, CancellationToken cancellationToken)
        {
            return await _collectingService.GetFozzyPickerByInn(inn, cancellationToken);
        }


        //TODO
        private void NoCheckFilledCalculator(IEnumerable<TraceableOrderItem> items)
        {

        }


        //TODO
        private void DefaultRowNumCalculator(IEnumerable<TraceableOrderItem> items)
        {
            var rowId = 1;
            foreach (var item in items)
            {
                item.RowNum = rowId;
                rowId++;
            }
        }



        #endregion process delegates


        public async Task<Unit> Handle(AllChangesCommand request, CancellationToken cancellationToken)
        {
            var currentOrder = request.Request.CurrentOrder;
            var fozzyOrder = request.Request.ChangedOrder;

            using (_logger.LoggingScope(currentOrder.BasketId))
            {
                _logger.LogInformation("{msg} {data}", "Aggregated Change collecting", request.Request);

                var currentPicker = currentOrder.GetPicker(); 

                var changedOrder = await _transformService.FromFozzyOrder(
                    currentOrder,
                    currentPicker,
                    fozzyOrder,
                    GetCatalogSingleItemInfo,
                    PickerInfoExtractorByInn,
                    NoCheckFilledCalculator,
                    DefaultRowNumCalculator,
                    _transformService.GetLogisticType, cancellationToken
                    );


                var status = (FozzyOrderStatus)changedOrder.OrderStatus;

                //var currentOrder = await _mediator.Send(new LoadCollectOrderCommand()
                //{
                //    BasketGuid = order.BasketId
                //});

                if (status == FozzyOrderStatus.Status15)
                {
                    if (currentOrder.CollectingState == (int)OrderCollectingState.UpCollecting)
                    {
                        await _mediator.Send(new RestartCollectingCommand()
                        {
                            Request = new CollectingStartRequest()
                            {
                                CurrentOrder = currentOrder,
                                ChangedOrder = changedOrder,
                            }
                        });
                    }
                    else
                    {
                        await _mediator.Send(new StartCollectingCommand()
                        {
                            Request = new CollectingStartRequest()
                            {
                                CurrentOrder = currentOrder,
                                ChangedOrder = changedOrder,
                            }
                        });
                    }
                }
                else if (status == FozzyOrderStatus.Status914)
                {
                    await _mediator.Send(new OfferReplacementsCommand()
                    {
                        Request = new CollectingOnApprovementRequest()
                        {
                            CurrentOrder = currentOrder,
                            ChangedOrder = changedOrder,
                        }
                    });
                }
                else if (status == FozzyOrderStatus.Status911)
                {


                    await _mediator.Send(new DoneCollectingCommand()
                    {
                        Request = new CollectingDoneRequest()
                        {
                            CurrentOrder = currentOrder,
                            ChangedOrder = changedOrder,
                        }
                    });
                }
                else if (status == FozzyOrderStatus.Status922)
                {
                    await _mediator.Send(new ReadyToCheckCollectingCommand()
                    {
                        Request = new DefaultCollectingRequest()
                        {
                            CurrentOrder = currentOrder,
                            ChangedOrder = changedOrder,
                        }
                    });
                }
                else // default  
                {
                    await _mediator.Send(new DefaultCollectingCommand()
                    {
                        Request = new CollectingAnyRequest()
                        {
                            CurrentOrder = currentOrder,
                            ChangedOrder = changedOrder,
                        }
                    });
                }

                return Unit.Value;
            }

        }
    }
  
}
