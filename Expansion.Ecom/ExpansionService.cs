using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Extensions;
using Basket.Interfaces;
using Collecting.Interfaces;
using Collecting.Interfaces.Enums;
using DomainServices.Interfaces;
using ECom.Entities.Models.Workflow;
using ECom.Types.Delivery;
using ECom.Types.DTO;
using ECom.Types.Orders;
using ECom.Types.Requests;
using ECom.Types.ServiceBus;
using ECom.Types.TimeSlots;
using ECom.Types.Views;
using Entities;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using Expansion.Ecom.Extensions;
using Expansion.Interfaces;
using Expansion.Interfaces.Clients;
using Expansion.Interfaces.Dto;
using Expansion.Interfaces.Dto.Requests;
using Expansion.Interfaces.Enums;
using Utils;
using Workflow.Interfaces;

namespace Expansion.Ecom
{
    public class ExpansionService : IExpansionService
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IBasketService _basketService;
        private readonly IWorkflowToExpansionClient _workflowToExpansionClient;
        private readonly ICollectingToExpansionClient _collectingToExpansionClient;
        private readonly ICollectingService _collectingService;
        private readonly IWorkflowService _workflowService;
        private readonly ICommonAppService _expansionApp;
        private readonly ITransformService _transformService;

        public ExpansionService(IDateTimeService dateTimeService, IBasketService basketService,
            IWorkflowToExpansionClient workflowToExpansionClient,//,
            ITransformService transformService,
            ICollectingService collectingService,
            IWorkflowService workflowService,
            ICollectingToExpansionClient collectingToExpansionClient,
            ICommonAppService expansionApp
            )
        {
            _dateTimeService = dateTimeService;
            _basketService = basketService;
            
            _workflowToExpansionClient = workflowToExpansionClient;
            _collectingService = collectingService;
            _workflowService = workflowService;
            _collectingToExpansionClient = collectingToExpansionClient;
            _expansionApp = expansionApp;
            _transformService = transformService;
        }

        
        /// <summary>
        /// Основной тест кэйс работы системы по параллельной передаче ордера в ECom
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ExpansionException"></exception>
        public async Task<CreateOrderAndCollectResponse> DoTestScenarioCreateOrderAndCollectWithReplacements(
            CreateOrderAndCollectRequest request,
            CancellationToken cancellationToken)
        {
            var result = new CreateOrderAndCollectResponse()
            {
                StartedAt = _dateTimeService.Now,
            };

            var fozzPicker = new Picker()
            {
                Inn = request.PickerInn
            };



            try
            {
                if (request.UseCases == CreateOrderAndCollectUseCases.None)
                    return result;

                // create new ecom order and time slot
                if (request.UseCases.Allow(CreateOrderAndCollectUseCases.CreateNewEComOrderAndTimeSlot))
                {
                    result.StartedUsesCase = CreateOrderAndCollectUseCases.CreateNewEComOrderAndTimeSlot;
                    // ok
                    var createEComOrderResult = await CreateNewEcomOrder(request, cancellationToken);
                    result.NewEComOrder = createEComOrderResult.Item1;
                    result.TimeSlot = createEComOrderResult.Item2;
                    result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CreateNewEComOrderAndTimeSlot);
                    var basketId = result.NewEComOrder.Id;


                    #region CheckForNewExpansionOrderFromEComOrder

                    if (request.UseCases.Allow(CreateOrderAndCollectUseCases.CheckForNewExpansionOrderFromEComOrder))
                    {
                        result.StartedUsesCase = CreateOrderAndCollectUseCases.CheckForNewExpansionOrderFromEComOrder;

                        // wait workflow!!!
                        result.CreateNewEComOrderWorkflowState = await WaitForWorkFlowState(basketId,
                           new[] { (int)FozzyOrderStatus.Status913 }, cancellationToken); //при статусе оплаты онлайн зависает в статусе FozzyOrderStatus.Status924 
                        if ((result.CreateNewEComOrderWorkflowState?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status913)
                            throw new ExpansionException(ExpansionErrors.WaitForCreateNewCollectingOrderError);
                        // and wait workflow

                        result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CheckForNewExpansionOrderFromEComOrder);

                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.CheckNewTraceableOrder))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.CheckNewTraceableOrder;

                            result.NewExpansionOrder = await
                                    _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken);
                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CheckNewTraceableOrder);
                        }

                    }

                    #endregion CheckForNewExpansionOrderFromEComOrder

                    #region CheckNewFozzyOrderInFozzyCollectingService

                    if ((result.NewExpansionOrder != null)
                        && request.UseCases.Allow(CreateOrderAndCollectUseCases.CheckNewFozzyOrderInFozzyCollectingService))
                    {
                        result.StartedUsesCase = CreateOrderAndCollectUseCases.CheckNewFozzyOrderInFozzyCollectingService;


                        // wait workflow!!!
                        var waitForState = await WaitForWorkFlowState(basketId,
                                new[] { (int)FozzyOrderStatus.Status913 }, cancellationToken);
                        if ((waitForState?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status913)
                            throw new ExpansionException(ExpansionErrors.WaitForCreateNewCollectingOrderError);
                        // end

                        result.NewFozzyOrder = await _collectingService.GetFozzyOrder(result.NewExpansionOrder.OrderNumber,
                            cancellationToken);

                        result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CheckNewFozzyOrderInFozzyCollectingService);
                    }

                    #endregion CheckNewFozzyOrderInFozzyCollectingService

                    #region StartCollecting
                    // start  collecting
                    if ((result.NewFozzyOrder != null) && request.UseCases.Allow(CreateOrderAndCollectUseCases.StartCollecting))
                    {
                        result.StartedUsesCase = CreateOrderAndCollectUseCases.StartCollecting;

                        // здесь нужно подождать запроса Чернова !!!!
                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.EmulateFozzyCollectingService;

                            var prepareStartedFozzyOrder = StartCollectingEmulation(result.NewFozzyOrder.MakeCopy(), fozzPicker);

                            var confirmResult = await _collectingToExpansionClient.ChangeCollecting(prepareStartedFozzyOrder, cancellationToken);
                            if (confirmResult.errorCode != 0)
                                throw new ExpansionException(ExpansionErrors.FozzyOrderError);

                            result.StartedExpansionOrder =
                                _expansionApp.ToView(await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken));

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService);
                        }

                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.CheckStartedFozzyOrderInFozzyCollectingService))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.CheckStartedFozzyOrderInFozzyCollectingService;

                            // wait workflow!!!
                            var waitForState = await WaitForWorkFlowState(basketId,
                                new[] { (int)FozzyOrderStatus.Status15 }, cancellationToken);
                            if ((waitForState?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status15)
                                throw new ExpansionException(ExpansionErrors.WaitForStartCollectingOrderError);

                            // end

                            // вдруг выше EmulateFozzyCollectingService = 0
                            if (result.StartedExpansionOrder == null)
                                result.StartedExpansionOrder =
                                    _expansionApp.ToView(await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken));


                            result.StartedFozzyOrder = await _collectingService.GetFozzyOrder(result.StartedExpansionOrder.OrderNumber,
                                cancellationToken);

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CheckStartedFozzyOrderInFozzyCollectingService);
                        }
                        result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.StartCollecting);
                    }

                    #endregion StartCollecting

                    #region CancelCollecting

                    if ((result.StartedFozzyOrder != null) && request.UseCases.Allow(CreateOrderAndCollectUseCases.CancelCollecting))
                    {
                        result.CancelExpansionOrder = await _workflowToExpansionClient.CancelCollecting(basketId, cancellationToken);
                        result.Cancelled = true;
                    }

                    #endregion CancelCollecting


                    #region OfferReplacements
                    // replacements ------------------------------------------
                    if ((result.StartedFozzyOrder != null) && request.UseCases.Allow(CreateOrderAndCollectUseCases.OfferReplacements))
                    {

                        result.StartedUsesCase = CreateOrderAndCollectUseCases.OfferReplacements;

                        // здесь нужно подождать запроса Чернова !!!!
                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.EmulateFozzyCollectingService;
                            // подготовим замены от сервиса сборки
                            var partialCollectedFozzyOrder = CollectingEmulation(result.StartedFozzyOrder.MakeCopy(), request.CollectingWithReplacementType,
                            FozzyOrderStatus.Status914);

                            var prepareOfferReplacementsFozzyOrder = ReplacementEmulation(partialCollectedFozzyOrder, request.ReplacementsType,
                                FozzyOrderStatus.Status914);

                            result.OfferReplacementsFozzyOrder = prepareOfferReplacementsFozzyOrder;

                            // как будто сервис сборки вызвал наc
                            var confirmResult = await _collectingToExpansionClient.ChangeCollecting(prepareOfferReplacementsFozzyOrder, cancellationToken);
                            if (confirmResult.errorCode != 0)
                                throw new ExpansionException(ExpansionErrors.FozzyOrderErrorAfterOfferReplacements, confirmResult.errorMessage);

                            var offerReplacementsOrder = await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken);

                            // здесь должны быть замены
                            result.OfferReplacementsExpansionOrder = _expansionApp.ToView(offerReplacementsOrder);

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService);
                        }

                        // wait workflow!!!
                        // подождали пока Workflow отправит это всё в FzClient
                        var workflowSentDataToFzClient = await WaitForWorkFlowState(basketId,
                            new[] { (int)FozzyOrderStatus.Status914 }, cancellationToken);
                        if ((workflowSentDataToFzClient?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status914)
                            throw new ExpansionException(ExpansionErrors.WaitForSendOfferReplacementsToOperator);
                        // end

                        #region AcceptReplacements

                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.AcceptReplacements))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.AcceptReplacements;

                            // здесь нужно подождать запроса fzClient !!!!
                            if (request.UseCases.Allow(CreateOrderAndCollectUseCases.EmulateAcceptFromFzClient))
                            {
                                result.StartedUsesCase = CreateOrderAndCollectUseCases.EmulateAcceptFromFzClient;
                                // отвечаем за оператора, которого нет ещё
                                var forPreparedAcceptedExpansionOrder = await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken);
                                // это эмуляция ответа оператора
                                result.AcceptingExpansionOrder = AcceptReplacementByOperatorEmulation(forPreparedAcceptedExpansionOrder, request.AcceptReplacementsType);

                                // подтверждаем сохранение оператором в ECom (оператор когда-то сам сохранит изменения)
                                var acceptedForBasket = _transformService.ToBasketCollectingItems(result.AcceptingExpansionOrder);
                                var acceptedForBasketJson = acceptedForBasket.ToJson();
                                await _basketService.UpdateCollectingInfo(basketId, acceptedForBasket, cancellationToken);

                                // вместо оператора (fzClient) позовём ECom.Workflow
                                await _workflowService.DefaultAction(basketId,
                                    (int)FozzyOrderStatus.Status914, (int)FozzyOrderStatus.Status915, cancellationToken);

                                result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.EmulateAcceptFromFzClient);
                            }
                            // wait workflow!!!
                            var workflowSentAcceptToExpansion = await WaitForWorkFlowState(basketId,
                                new[] { (int)FozzyOrderStatus.Status915 }, cancellationToken);
                            if ((workflowSentAcceptToExpansion?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status915)
                                throw new ExpansionException(ExpansionErrors.WaitForSendAcceptReplacementsTExpansion);
                            // end

                            // получили подтверждение от workflow
                            result.AcceptedExpansionOrder = await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken);

                            result.AcceptedFozzyOrder = await _collectingService.GetFozzyOrder(result.StartedExpansionOrder.OrderNumber,
                                        cancellationToken);

                            #region StartCollectingAgain

                            if (request.UseCases.Allow(CreateOrderAndCollectUseCases.StartCollectingAgain))
                            {
                                result.StartedUsesCase = CreateOrderAndCollectUseCases.StartCollectingAgain;

                                // здесь нужно подождать запроса Чернова !!!!
                                if (request.UseCases.Allow(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService))
                                {
                                    result.StartedUsesCase = CreateOrderAndCollectUseCases.EmulateFozzyCollectingService;
                                    // приступаем к старту сборки START AGAIN!
                                    var prepareStartedFozzyOrderSecondTime = StartCollectingEmulation(result.AcceptedFozzyOrder.MakeCopy(), fozzPicker);

                                    var confirmResult = await _collectingToExpansionClient.ChangeCollecting(prepareStartedFozzyOrderSecondTime, cancellationToken);
                                    if (confirmResult.errorCode != 0)
                                        throw new ExpansionException(ExpansionErrors.FozzyOrderErrorStartAfterReplacements, confirmResult.errorMessage);

                                    result.StartedAfterReplacementsExpansionOrder =
                                        _expansionApp.ToView(await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken));

                                    result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService);

                                }
                                // wait workflow!!! NEW
                                var workflowStartAgain = await WaitForWorkFlowState(basketId,
                                    new[] { (int)FozzyOrderStatus.Status15 }, cancellationToken);
                                if ((workflowStartAgain?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status15)
                                    throw new ExpansionException(ExpansionErrors.WaitForStartAgainCollectingOrderError);
                                // end

                                result.StartedAfterReplacementsFozzyOrder = await _collectingService.GetFozzyOrder(result.StartedExpansionOrder.OrderNumber,
                                            cancellationToken);

                                result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.StartCollectingAgain);
                            }

                            #endregion StartCollectingAgain

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.AcceptReplacements);

                        }

                        #endregion AcceptReplacements

                        result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.OfferReplacements);

                    }

                    //end replacements ----------------------------------------
                    #endregion OfferReplacements

                    #region DoneCollecting

                    // done collecting
                    if ((result.StartedFozzyOrder != null) && request.UseCases.Allow(CreateOrderAndCollectUseCases.DoneCollecting))
                    {
                        result.StartedUsesCase = CreateOrderAndCollectUseCases.DoneCollecting;
                        // если вдруг были замены, то сформируем собранный ордер с последнего стартонутого ордера
                        var startedFozzyOrder = result.StartedAfterReplacementsFozzyOrder ?? result.StartedFozzyOrder;

                        // здесь нужно подождать запроса Чернова !!!!
                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.EmulateFozzyCollectingService;

                            var prepareCollectedFozzyOrder = CollectingEmulation(startedFozzyOrder.MakeCopy(), request.CollectingType,
                            FozzyOrderStatus.Status911);


                            var confirmResult = await _collectingToExpansionClient.ChangeCollecting(prepareCollectedFozzyOrder, cancellationToken);
                            if (confirmResult.errorCode != 0)
                                throw new ExpansionException(ExpansionErrors.FozzyOrderErrorAfterDoneCollecting,
                                    confirmResult.errorMessage);

                            var collectedOrder = await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken);

                            result.CollectedExpansionOrder = _expansionApp.ToView(collectedOrder);

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService);
                        }

                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.CheckCollectedFozzyOrderInFozzyCollectingService))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.CheckCollectedFozzyOrderInFozzyCollectingService;


                            // wait workflow!!! DONE !!!
                            var workflowSentAcceptToExpansion = await WaitForWorkFlowState(basketId,
                                new[] { (int)FozzyOrderStatus.Status911 }, cancellationToken);
                            if ((workflowSentAcceptToExpansion?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status911)
                                throw new ExpansionException(ExpansionErrors.WaitForDoneCollectingOrderError);
                            // end

                            result.CollectedFozzyOrder = await _collectingService.GetFozzyOrder(result.NewExpansionOrder.OrderNumber,
                                cancellationToken);

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CheckCollectedFozzyOrderInFozzyCollectingService);
                        }

                        result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.DoneCollecting);

                        //check ecom picket qty!
                        if ((result.CollectedFozzyOrder != null) && request.UseCases.Allow(CreateOrderAndCollectUseCases.CheckCollectedEcomOrder))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.CheckCollectedEcomOrder;

                            result.CollectedEComOrder = await _basketService.GetEComOrder(basketId, cancellationToken);

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CheckCollectedEcomOrder);
                        }
                    }

                    #endregion DoneCollecting

                    #region ReadyToCheckCollecting


                    if ((result.CollectedFozzyOrder != null) && request.UseCases.Allow(CreateOrderAndCollectUseCases.ReadyToCheckCollecting))
                    {
                        result.StartedUsesCase = CreateOrderAndCollectUseCases.ReadyToCheckCollecting;

                        // здесь нужно подождать запроса Чернова !!!!
                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.EmulateFozzyCollectingService;
                            var readyToCheckFozzyOrder = result.CollectedFozzyOrder.MakeCopy();
                            readyToCheckFozzyOrder.GetOrder().orderStatus = FozzyOrderStatus.Status922.ToInt().ToString();

                            // emulate
                            var confirmResult = await _collectingToExpansionClient.ChangeCollecting(readyToCheckFozzyOrder, cancellationToken);
                            if (confirmResult.errorCode != 0)
                                throw new ExpansionException(ExpansionErrors.FozzyOrderError);

                            result.ReadyToCheckCollectingExpansionOrder =
                                _expansionApp.ToView(await _expansionApp.GetOrderWithItemsByBasketId(basketId, cancellationToken));

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.EmulateFozzyCollectingService);
                        }

                        if (request.UseCases.Allow(CreateOrderAndCollectUseCases.CheckReadyToCheckCollectingFozzyOrderInFozzyCollectingService))
                        {
                            result.StartedUsesCase = CreateOrderAndCollectUseCases.CheckReadyToCheckCollectingFozzyOrderInFozzyCollectingService;

                            // wait workflow!!
                            var workflowSentReadyToCheckCollecting = await WaitForWorkFlowState(basketId,
                                new[] { (int)FozzyOrderStatus.Status922 }, cancellationToken);
                            if ((workflowSentReadyToCheckCollecting?.MerchantStateId ?? 0) != (int)FozzyOrderStatus.Status922)
                                throw new ExpansionException(ExpansionErrors.WaitForReadyToCheckCollectingOrderError);
                            // end

                            result.ReadyToCheckCollectingFozzyOrder = await _collectingService.GetFozzyOrder(result.NewExpansionOrder.OrderNumber,
                                cancellationToken);

                            result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.CheckReadyToCheckCollectingFozzyOrderInFozzyCollectingService);
                        }

                        result.UsesCasesPassed.Add(CreateOrderAndCollectUseCases.ReadyToCheckCollecting);
                    }

                    #endregion ReadyToCheckCollecting

                }
            }
            //catch (PresentationException<EComError> ex)
            //{

            //}
            catch (DescribedException ex)
            {
                result.BusinessErrors.Add(ex.GetEComExceptionResponse(EComErrorType.Warning).EComError);
            }
            catch (Exception ex)
            {
                result.UnhandledErrors.Add(ex.GetEComExceptionResponse(EComErrorType.Error).EComError);
            }
            finally
            {
                result.Request = request;
                result.CompletedAt = _dateTimeService.Now;
            }

            return result;
        }


    
        // common use case
        /// <summary>
        /// Сформировать и отправить текущее состояние ордера в сервис сборки 
        /// </summary>
        /// <param name="basketId"></param>
        /// <param name="statusId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OrderData> SendOrderToFozzyCollectingService(Guid basketId, int statusId,  CancellationToken cancellationToken)
        {
            return await _workflowToExpansionClient.SendOrderToFozzyCollectingService(basketId, statusId, cancellationToken);
        }


        // common use case
        /// <summary>
        /// Сформировать и отправить обновление Presta веб сайта 
        /// </summary>
        /// <param name="basketId"></param>
        /// <param name="statusId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OrderData> SendOrderToFozzyWebSite(Guid basketId, int statusId, CancellationToken cancellationToken)
        {
            return await _workflowToExpansionClient.SendOrderToFozzyWebSite(basketId, statusId, cancellationToken);
        }


        /// <summary>
        /// Проверить в ECom.Workflow текущее состояние ордера
        /// </summary>
        /// <param name="basketId"></param>
        /// <param name="merchantStatusIds"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="times"></param>
        /// <param name="delayInSeconds"></param>
        /// <returns></returns>
        public async Task<WorkflowOrderState?> WaitForWorkFlowState(Guid basketId, int[] merchantStatusIds,
            CancellationToken cancellationToken, int times = 10, int delayInSeconds = 5)
        {
            WorkflowOrderState state = null;
            int attemps = 0;
            while (attemps < times)
            {
                try
                {
                    state = await _workflowService.GetOrderCurrentState(basketId, cancellationToken);
                }
                catch (Exception ex)
                {
                }

                if (state != null && merchantStatusIds.Any(p => p == state.MerchantStateId))
                    return state;
                await Task.Delay(delayInSeconds * 1000);
                attemps++;
            }
            return state;
        }


        /// <summary>
        /// Сэмулировать старт сборки Start Collecting
        /// </summary>
        /// <param name="fozzyOrder"></param>
        /// <param name="picker"></param>
        /// <returns></returns>
        public OrderData StartCollectingEmulation(OrderData fozzyOrder, Picker picker)
        {
            var order = fozzyOrder.GetOrder();
            order.globalUserId = picker.Inn;

            order.orderStatus = FozzyOrderStatus.Status15.ToInt().ToString();
            return fozzyOrder;
        }

        /// <summary>
        /// Сэмулировать окончание сборки Done Collecting 
        /// </summary>
        /// <param name="fozzyOrder"></param>
        /// <param name="emulationType"></param>
        /// <param name="fozzyOrderStatus"></param>
        /// <returns></returns>
        public OrderData CollectingEmulation(OrderData fozzyOrder, CollectingEmulationType emulationType, 
            FozzyOrderStatus fozzyOrderStatus)
        {
            fozzyOrder.GetOrder().orderStatus = fozzyOrderStatus.ToInt().ToString();
            if (emulationType == CollectingEmulationType.None)
                return fozzyOrder;

            foreach(var item in fozzyOrder.orderLines)
            {
                item.pickerQuantity = emulationType == CollectingEmulationType.CollectPartialAll ?  "1" :
                    (HasReplacement(item) ? item.pickerQuantity : item.orderQuantity);
                if (emulationType == CollectingEmulationType.CollectAndPartial)
                    break;
            }

            return fozzyOrder;
        }

        public bool HasReplacement(FzShopOrderLines orderLine)
        {
            var hasReplacements = !string.IsNullOrEmpty(orderLine.replacementLagers);
            return hasReplacements;
        }



        public OrderData ReplacementEmulation(OrderData fozzyOrder, ReplacementsEmulationType emulationType, 
            FozzyOrderStatus fozzyOrderStatus)
        {
            fozzyOrder.GetOrder().orderStatus = fozzyOrderStatus.ToInt().ToString();

            if (emulationType == ReplacementsEmulationType.None)
                return fozzyOrder;

            foreach(var item in fozzyOrder.orderLines)
            {
                if(item.pickerQuantity != item.orderQuantity)
                {
                    item.replacementLagers = GetReplacementItems((Items)item.lagerId)
                        .Select(x => (int)x).ToList().ToCommaString();
                }
            }

            return fozzyOrder;
        }

        public TraceableOrder AcceptReplacementByOperatorEmulation(TraceableOrder order, AcceptReplacementsEmulationType emulationType)
        {
            if (emulationType == AcceptReplacementsEmulationType.None)
                return order;

            if (emulationType == AcceptReplacementsEmulationType.SetFizedForAll)
            {
                foreach (var item in order.Items)
                {
                    if (item.ReplacementOnLagerId != null && item.OrderQuantity == 0)
                    {
                        item.OrderQuantity = 1;
                    }
                }
            }
            else
            {
                throw new NotImplementedException($"AcceptReplacementsEmulationType: {emulationType} не реализован");
            }

            return order;
        }



        /// <summary>
        /// Создать ордер и взять свободный таймслот в ECom 
        /// </summary>
        /// <param name="filialId"></param>
        /// <param name="merchant"></param>
        /// <param name="origin"></param>
        /// <param name="basketType"></param>
        /// <param name="addressType"></param>
        /// <param name="deliveryType"></param>
        /// <param name="itemsCombinations"></param>
        /// <param name="orderPaymentType"></param>
        /// <param name="business"></param>
        /// <param name="createBasketInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(BasketView, TimeSlotView)> CreateNewEcomOrder(
           Filials filialId,
           Merchant merchant,
           RequestOrigin origin,
           BasketType basketType,
           DeliveryAddressType addressType,
           DeliveryType deliveryType,
           ItemsCombinations itemsCombinations,
           OrderPaymentType orderPaymentType,
           Business business,
           CreateBasketInfo createBasketInfo,
           CancellationToken cancellationToken)
        {
            string barcode = createBasketInfo.OtherInfo.ExternalOrderId ?? Guid.NewGuid().ToString();

            var timeSlotOn = _dateTimeService.Now;
            var timeSlot = await _basketService.GetTimeSlot((int)filialId, deliveryType, timeSlotOn, merchant, cancellationToken);


            var createBasketRequest = GetCreateBasketRequest(
                filialId, 
                merchant, 
                origin, 
                basketType, 
                addressType, 
                deliveryType,
                createBasketInfo.LoyalityInfo,
                createBasketInfo.LocationInfo,
                createBasketInfo.ContactInfo
                );

            var deliveryInfo = createBasketRequest.DeliveryInfo;

            var changeOwnerRequest = GetChangeOwnerRequest(createBasketInfo.LoyalityInfo.Owner);

            var setFilialDeliveryRequest = GetSetFilialDeliveryRequest(filialId, deliveryInfo);

            var itemsToAdd = GetAddItems(GetItemsByCombination(itemsCombinations).ToList());

            var props = GetOrderPropertiesRequest(
                timeSlot.TimeFrom, 
                timeSlot.TimeSlotId,
                orderPaymentType,
                createBasketInfo.LoyalityInfo,
                createBasketInfo.CompanyInfo,
                createBasketInfo.CustomerInfo,
                createBasketInfo.OtherInfo);

            var getPropsCloseRequest = GetPropsCloseRequest(props, business, deliveryInfo, barcode, origin);

            var basketView = await _basketService.CreateOrder(createBasketRequest, changeOwnerRequest, setFilialDeliveryRequest,
                     itemsToAdd, getPropsCloseRequest, cancellationToken);

            return (basketView, timeSlot);
        }


        /// <summary>
        /// Создание ордера ECom через подготовленный запрос
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(BasketView, TimeSlotView)> CreateNewEcomOrder(CreateOrderAndCollectRequest request, CancellationToken cancellationToken)
        {
            return await CreateNewEcomOrder(
                request.Filial,
                request.Merchant,
                request.Origin,
                request.BasketType,
                request.AddressType,
                request.DeliveryType,
                request.ItemsInCase,
                request.PaymentType,
                request.Business,
                request.CreateBasketInfo,
                cancellationToken
                );
        }


        /// <summary>
        /// Шаблон на создание запроса на выполнения тестового кэйса
        /// </summary>
        /// <param name="filial"></param>
        /// <param name="merchant"></param>
        /// <param name="origin"></param>
        /// <param name="owner"></param>
        /// <param name="basketType"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="userId"></param>
        /// <param name="addressType"></param>
        /// <param name="deliveryType"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="accurate"></param>
        /// <param name="comments"></param>
        /// <param name="contactName"></param>
        /// <param name="itemsCombinations"></param>
        /// <param name="accessToken"></param>
        /// <param name="url"></param>
        /// <param name="contragentFullName"></param>
        /// <param name="contragentOKPO"></param>
        /// <param name="customerEmail"></param>
        /// <param name="customerName"></param>
        /// <param name="customerPhone"></param>
        /// <param name="feedbackPhone"></param>
        /// <param name="orderPaymentType"></param>
        /// <param name="notes"></param>
        /// <param name="business"></param>
        /// <param name="pickupDate"></param>
        /// <param name="externalOrderId"></param>
        /// <returns></returns>
        public static CreateOrderAndCollectRequest GetCreateOrderAndCollectRequest(
           Filials filial,
           Merchant merchant,
           RequestOrigin origin,
           string owner,
           BasketType basketType,
           string phoneNumber,
           string userId,
           DeliveryAddressType addressType,
           DeliveryType deliveryType,
           string latitude,
           string longitude,
           bool accurate,
           string comments,
           string contactName,
           ItemsCombinations itemsCombinations,
            string accessToken, string url,
           string contragentFullName, string contragentOKPO,
           string customerEmail, string customerName, string customerPhone,
           string feedbackPhone, 
           OrderPaymentType orderPaymentType,
           string notes,
           Business business,
           DateTime pickupDate,
            string? externalOrderId = null
           )
        {
            var createOrderAndCollectRequest = new CreateOrderAndCollectRequest();
            createOrderAndCollectRequest.AddressType = addressType;
            createOrderAndCollectRequest.BasketType = basketType;
            createOrderAndCollectRequest.Business = business;
            createOrderAndCollectRequest.PickupDate = pickupDate;
            createOrderAndCollectRequest.PaymentType = orderPaymentType;
            createOrderAndCollectRequest.ItemsInCase = itemsCombinations;
            createOrderAndCollectRequest.Merchant = merchant;
            createOrderAndCollectRequest.Filial = filial;
            createOrderAndCollectRequest.Origin = origin;
            createOrderAndCollectRequest.Owner = owner;
            createOrderAndCollectRequest.DeliveryType = deliveryType;

            var createBaskerInfo = createOrderAndCollectRequest.CreateBasketInfo;
            createBaskerInfo.LoyalityInfo.Owner = owner;
            createBaskerInfo.LoyalityInfo.Url = url;
            createBaskerInfo.LoyalityInfo.UserId = userId;
            createBaskerInfo.LoyalityInfo.AccessToken = accessToken;

            createBaskerInfo.LocationInfo.Longitude = longitude;
            createBaskerInfo.LocationInfo.Latitude = latitude;
            createBaskerInfo.LocationInfo.Accurate = accurate;
            createBaskerInfo.LocationInfo.Comments = comments;

            createBaskerInfo.CustomerInfo.FeedbackPhone = feedbackPhone;
            createBaskerInfo.CustomerInfo.CustomerName = customerName;
            createBaskerInfo.CustomerInfo.CustomerEmail = customerEmail;
            createBaskerInfo.CustomerInfo.CustomerName = customerName;
            createBaskerInfo.CustomerInfo.CustomerPhone = customerPhone;

            createBaskerInfo.ContactInfo.PhoneNumber = phoneNumber;
            createBaskerInfo.ContactInfo.ContactName = contactName;
            //createBaskerInfo.ContactInfo.

            createBaskerInfo.CompanyInfo.ContragentFullName = contragentFullName;
            createBaskerInfo.CompanyInfo.ContragentOKPO = contragentOKPO;

            createBaskerInfo.OtherInfo.Notes    = notes;
            createBaskerInfo.OtherInfo.ExternalOrderId = externalOrderId;

            return createOrderAndCollectRequest;
                //await CreateNewEcomBasket(
                //filialId,
                //merchant,
                //origin,
                //basketType,
                //addressType,
                //deliveryType,
                //itemsCombinations,
                //orderPaymentType,
                //business,
                //createBaskerInfo,
                //cancellationToken
                //);
        }


        #region Basket Creation Details

        public static IEnumerable<(Items lager, decimal qty, decimal price)> GetItemsByCombination(ItemsCombinations itemsCombinations)
        {

            switch (itemsCombinations)
            {
                case ItemsCombinations.OneItem:
                    return new (Items, decimal, decimal)[]
                    {
                        (Items.CocaCola, 10, 20.5m),
                    };

                case ItemsCombinations.TwoItems:
                    return new (Items, decimal, decimal)[]
                    {
                        (Items.CocaCola, 10, 20.5m),
                        (Items.Vine, 2, 240.2m)
                    };

                default: return new (Items, decimal, decimal)[0];
            }
        }

        public static IEnumerable<Items> GetReplacementItems(Items itemToReplace)
        {

            switch (itemToReplace)
            {
                case Items.CocaCola:
                    return new List<Items>(new[]
                    {
                        Items.Water_440815
                    });

                case Items.Vine:
                    return new List<Items>(new[]
                    {
                        Items.Viski_37400,
                        Items.Viski_2712
                    });

                case Items.Viski_2712:
                    return new List<Items>(new[]
                    {
                        Items.Viski_37400
                    });

                case Items.Viski_37400:
                    return new List<Items>(new[]
                    {
                        Items.Viski_2712,
                        Items.Vine
                    });

                case Items.Water_440815:
                    return new List<Items>(new[]
                    {
                        Items.Water_599434,
                        Items.CocaCola
                    });

                case Items.Water_599434:
                    return new List<Items>(new[]
                    {
                        Items.Water_440815,
                        Items.CocaCola
                    });

                default: return new List<Items>();
            }
        }



        public ICreateBasketRequest GetCreateBasketRequest(
            Filials filial,
            Merchant merchant,
            RequestOrigin origin,
            BasketType basketType,
            DeliveryAddressType addressType,
            DeliveryType deliveryType,
             LoyalityInfo loyalityInfo,
             LocationInfo locationInfo,
             ContactInfo contactInfo
            )
        {
            return new CreateBasketRequest()
            {
                FilialId = (int)filial,
                MerchantId = (int)merchant,
                Origin = origin,
                Owner = "", //loyalityInfo.Owner,
                Type = basketType,
                PhoneNumber = contactInfo.PhoneNumber,
                UserId = loyalityInfo.UserId,
                DeliveryInfo = GetDeliveryInfo(addressType, deliveryType, contactInfo, locationInfo)
            };
        }

        public DeliveryInfo GetDeliveryInfo(DeliveryAddressType addressType,
            DeliveryType deliveryType,
            ContactInfo contactInfo,
            LocationInfo locationInfo)
        {
            var locality = addressType == DeliveryAddressType.Coordinates
                ? "Київ, Героїв Дніпра вулиця, 34"
                : "";

            return new DeliveryInfo()
            {
                AddressType = addressType,
                City = "Киев",
                DeliveryType = deliveryType,
                BuildingPart = "",
                Street = "Героїв Дніпра вулиця",
                House = "34",
                Porch = "3",
                Floor = "2",
                Flat = "1",
                Company = "",
                ContactPhone = contactInfo.ContactPhone,
                Latitude = locationInfo.Latitude, //.ToString(_locality),
                Longitude = locationInfo.Longitude, //.ToString(_locality),
                Accurate = locationInfo.Accurate,
                Comments = locationInfo.Comments, // "Координати потребують уточнення. ",
                Locality = locality,
                ContactName = contactInfo.ContactName,
                Region = "Киевская область"
            };
        }


        public IChangeOwnerRequest GetChangeOwnerRequest(string owner)
        {
            return new ChangeOwnerRequest()
            {
                Owner = owner
            };
        }

        public ISetFilialDeliveryRequest GetSetFilialDeliveryRequest(Filials filialId, DeliveryInfo deliveryInfo)
        {
            return new SetFilialDeliveryRequest()
            {
                FilialId = (int)filialId,
                DeliveryInfo = deliveryInfo
            };
        }

        public IAddItemRequest GetAddItem((Items lager, decimal qty, decimal price) input)
        {
            return new AddItemRequest()
            {
                LagerId = (int)input.lager,
                Price = input.price,
                Qty = input.qty,
            };
        }

        public IEnumerable<IAddItemRequest> GetAddItems(IEnumerable<(Items lager, decimal qty, decimal price)> input)
        {
            foreach (var item in input)
            {
                yield return GetAddItem(item);
            }
        }

        public OrderPropertiesRequest GetOrderPropertiesRequest(
            DateTime pickupDate,
            int timeSlotId,
            OrderPaymentType orderPaymentType,
            LoyalityInfo loyalityInfo,
            CompanyInfo companyInfo,
            CustomerInfo customerInfo,  
            OtherInfo otherInfo)
        {
            return new OrderPropertiesRequest()
            {
                AccessToken = loyalityInfo.AccessToken,
                ContragentFullName = companyInfo.ContragentFullName,
                ContragentOKPO = companyInfo.ContragentOKPO,
                CustomerEmail = customerInfo.CustomerEmail,
                PickupDate = pickupDate,
                Url = loyalityInfo.Url,
                FeedbackPhone = customerInfo.FeedbackPhone,
                PaymentType = orderPaymentType,
                CustomerName = customerInfo.CustomerName,
                CustomerPhone = customerInfo.CustomerPhone,
                TimeSlotId = timeSlotId,
                Notes = otherInfo.Notes,
                UserId = loyalityInfo.UserId,
            };
        }

        public IPropsCloseRequest GetPropsCloseRequest(OrderPropertiesRequest props, Business business,
            DeliveryInfo deliveryInfo, string externalOrderId, RequestOrigin requestOrigin)
        {
            return new PropsCloseRequest()
            {
                OrderProps = props,
                AccessToken = props.AccessToken,
                Business = Enum.GetName(typeof(Business), (int)business),
                DeliveryInfo = deliveryInfo,
                ExternalOrderId = externalOrderId,
                Origin = requestOrigin,
            };
        }


        


        #endregion Basket Creation Details

    }
}