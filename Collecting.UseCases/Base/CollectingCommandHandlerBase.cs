using Entities.Models.Expansion;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Collecting.UseCases
{
    public class CollectingCommandHandlerBase
    {
        protected IMediator _mediator;
        protected ILogger _logger;
        public CollectingCommandHandlerBase(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

       


      

       


        //public async Task SetPickerId(FozzyOrder order)
        //{
        //    var inn = order.GlobalUserId; // strange property name, but ok it's INN 

        //    if (!string.IsNullOrEmpty(inn))
        //    {
        //        try
        //        {
        //            var InnAndGlobalUserId = await _mediator.Send(new GetGlobalUserIdByInnCommand()
        //            {
        //                Inn = inn
        //            });

        //            view.PickerUserId = InnAndGlobalUserId.globalUserId;
        //            view.PickerName = InnAndGlobalUserId.peopleFullName;
        //            view.UserInn = inn;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError("{msg} {error} {stackTrace} {inn}", "SetPickerId error", ex.GetBaseException().Message, ex.StackTrace, inn);
        //            throw;
        //        }
        //    }
        //    else
        //    {
        //        _logger.LogWarning("{msg} {data}", "Inn not found!", fozzyOrder.XmlSerialize());
        //    }
        //}

        // TODO move to ApplicationService interfaces 
        /*
        public async Task<bool> TrySaveOrderToFozzyShopCollectingService(Guid orderId, OrderData order, string source = "")
        {
            try
            {
                _logger.LogInformation("{msg} {data}", source, order.XmlSerialize());

                if (order.IsCreatedInFozzyShop())
                {
                    //TODO add Polly.Retry!!
                    var result = await _mediator.Send(new FozzyShopSaveOrderComand()
                    {
                        BasketGuid = orderId,
                        Order = order
                    });

                    return result.Result.IsSuccess;
                }
                else
                {
                    throw new CollectException(CollectErrors.NotFozzyShopOrder, "TrySaveOrderToFozzyShopCollectingService");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{msg} {comment} {error} {StackTrace} {data}", source, "TrySaveOrderToFozzyShopCollectingService error",
                         ex.FullMessage(), ex.StackTrace, order.XmlSerialize());
            }
            return false;
        }

        public async Task<bool> TrySaveOrderToFozzyShopSite(Guid orderId, OrderData order, string source = "")
        {
            try
            {
                _logger.LogInformation("{msg} {data}", source, order.XmlSerialize());

                if (order.IsCreatedInFozzyShop())
                {

                    //TODO add Polly.Retry!!
                    var result = await _mediator.Send(new FozzyShopSaveOrderToSiteComand()
                    {
                        BasketGuid = orderId,
                        Order = order
                    });

                    return result.IsSuccess;
                }
                else
                {
                    throw new CollectException(CollectErrors.NotFozzyShopOrder, "TrySaveOrderToFozzyShopSite");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{msg} {comment} {error} {StackTrace} {data}", source, "TrySaveOrderToFozzyShopSite error",
                         ex.FullMessage(), ex.StackTrace, order.XmlSerialize());
            }
            return false;
        }

        public async Task<OrderData> CreateFozzyShopOrder(EcomToFozzyOrderCreateAdaptedInfo adaptedInfo,
           ICollectToFozzyShopCreateOrderStrategy strategy)
        {
            // create fozz order
            var createdFozzyShopOrder = await _mediator.Send(new CreateFozzyShopOrderComand()
            {
                //Order = source,
                EComOrder = adaptedInfo,
                CreateStrategy = strategy,
            });

            return createdFozzyShopOrder;
        }

        public async Task<OrderData> UpdateFozzyShopOrder(FzShopCollectOrderView source, EcomToFozzyOrderUpdateAdaptedInfo adaptedInfo,
          ICollectToFozzyShopUpdateOrderStrategy strategy)
        {
            // update fozz order
            var updatedFozzyShopOrder = await _mediator.Send(new UpdateFozzyShopOrderComand()
            {
                Order = source,
                EComOrder = adaptedInfo,
                UpdateStrategy = strategy,
            });

            return updatedFozzyShopOrder;
        }

        public async Task<OrderData> UpdateFozzyShopOrder(FzShopCollectOrderView source, OrderData oldFozzyShopOrder, 
            FozzyShopOrderStatus newStatus, DateTime modified, ICollectToFozzyShopUpdateOrderStrategy strategy)
        {
            var oldOrder = oldFozzyShopOrder.GetOrder();

            var info = EcomToFozzyOrderUpdateAdaptedInfo.CreateFromCollectingViewAndFozzyShopOrder(source, oldOrder, newStatus,
                await GetUserInn(source.PickerUserId, source.BasketGuid), modified);

            return await UpdateFozzyShopOrder(source, info, strategy);
        }



        public Task<FzShopCollectOrderView> UpdateEComCollectView(OrderData source, FzShopCollectOrderView viewToUpdate,
           IFozzyShopToCollectUpdateOrderStrategy strategy)
        {
            return Task.FromResult(strategy.UpdateView(source, viewToUpdate));
        }


        public async Task<string> GetUserInn(int? PickerUserId, Guid basketGuid)
        {
            if (PickerUserId.HasValue)
            {
                var innAndGlobalUserId = await _mediator.Send(new GetInnByGlobalUserIdCommand()
                {
                    GlobalUserId = PickerUserId.Value,
                    BasketGuid = basketGuid
                });
                return innAndGlobalUserId.peopleInn;
            }
            else
            {
                _logger.LogWarning("{msg} {data}", "GetUserInn: Picker not found", basketGuid);
            }
            return string.Empty;
        }


        

        public async Task SetPickerInn(OrderData fozzyOrder, FzShopCollectOrderView order)
        {
            var globalUserId = order.PickerUserId; 

            if (globalUserId.HasValue)
            {
                try
                {
                    var InnAndGlobalUserId = await _mediator.Send(new GetInnByGlobalUserIdCommand()
                    {
                        GlobalUserId = globalUserId.Value,
                        BasketGuid = order.BasketGuid
                    });

                   foreach(var line in fozzyOrder.orderLines)
                   {
                      line.globalUserId = InnAndGlobalUserId.peopleInn;
                   }
                }
                catch (Exception ex)
                {
                    _logger.LogError("{msg} {error} {stackTrace}", "GetPickerId error", ex.GetBaseException().Message, ex.StackTrace);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("{msg} {data}", "Global User Id (Picker id) not found!", order.JsonSerialize());
            }
        }
        */
    }
}
