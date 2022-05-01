using ApplicationServices.Interfaces.Models;
using AutoMapper;
using ECom.Entities.Models;
using Entities.Consts;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Utils;
using Workflow.Controllers.Dto.Requests;
using Workflow.Controllers.Filters.Exceptions;
using Workflow.Interfaces.Exceptions;
using Workflow.UseCases.Order;

namespace Workflow.Controllers
{

    [ApiController]
    [ApiExplorerSettings(GroupName = ExpansionConsts.Common.App.Groups.Workflow.GroupNameVersionOne)]
    [Route(ExpansionConsts.Common.App.Controllers.Workflow.WorkflowBaseRoute)]
    [SwaggerTag(ExpansionConsts.Common.App.Controllers.Workflow.Description)]
    public class WorkflowController : ControllerBase
    {
        private readonly ILogger<WorkflowController> _logger;
        private readonly IMapper _mapper;
        public WorkflowController(ILogger<WorkflowController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// ECom.Workflow зовёт это после создания ордера в ECom
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="requestHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ExpansionConsts.Common.App.Controllers.Workflow.EnterCollectingRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TraceableOrder))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [WorkflowExceptionHandler]
        public async Task<TraceableOrder> EnterCollecting(
            [FromBody] FozzyCollectableOrderInfo orderInfo,
            [FromServices] IRequestHandler<CreateOrderCommand, TraceableOrder> requestHandler, 
            CancellationToken cancellationToken)
        {
            using (_logger.LoggingScope(orderInfo.BasketGuid))
            {
                _logger.LogInformation("{msg} {data}",
                        ExpansionConsts.Common.App.Controllers.Workflow.EnterCollectingRoute, orderInfo);

                var order = await requestHandler.Handle(new CreateOrderCommand
                {
                    EComCollectableOrder = orderInfo
                }, cancellationToken);

                return order; // _mapper.Map<TraceableOrderView>(order); 
            }
        }


        /// <summary>
        /// Отмена сборки заказа (пока пустышка)
        /// </summary>
        /// <param name="basketGuid">Guid корзины/заказа для отмены</param>
        /// <returns></returns>
        [HttpPost(ExpansionConsts.Common.App.Controllers.Workflow.CancelCollectingRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TraceableOrder))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [WorkflowExceptionHandler]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<TraceableOrder> CancelCollecting(
            Guid basketGuid,
            [FromServices] IRequestHandler<CancelOrderCommand, TraceableOrder> cancelOrderCommandHandler,
            CancellationToken cancellationToken)
        {
            using (_logger.LoggingScope(basketGuid))
            {
                _logger.LogInformation("{msg} {data}",
                  ExpansionConsts.Common.App.Controllers.Workflow.CancelCollectingRoute, basketGuid);

                var order = await cancelOrderCommandHandler.Handle(
                    new CancelOrderCommand()
                    {
                        BasketGuid = basketGuid
                    }, cancellationToken);

                return order; // _mapper.Map<TraceableOrderView>(order);
            }
        }



        /// <summary>
        /// После согласования заказа оператор заполняет Qty для замен (а также может добавить новые позиции)  
        /// и переводит заказ в состояние 915, подтверждая состав сборки (текущее состояние 914).
        /// </summary>
        /// <returns></returns>
        [HttpPost(ExpansionConsts.Common.App.Controllers.Workflow.AcceptCollectingRoute, Name = "AcceptCollecting")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TraceableOrderView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [WorkflowExceptionHandler]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<TraceableOrder> AcceptCollecting(
            Guid basketGuid, 
            [FromBody] FozzyCollectableOrderInfo orderInfo,
            [FromServices] IRequestHandler<AcceptReplacementsCommand, TraceableOrder> requestHandler,
            CancellationToken cancellationToken)
        {
            using (_logger.LoggingScope(basketGuid))
            {
                _logger.LogInformation("{msg} {data}",
                        ExpansionConsts.Common.App.Controllers.Workflow.AcceptCollectingRoute, orderInfo);

                var order = await requestHandler.Handle(new AcceptReplacementsCommand()
                {
                    Request = new CollectingAcceptAfterApproveRequest()
                    {
                        AcceptedOrder = orderInfo,
                        BasketGuid = basketGuid
                    }
                }, cancellationToken);

                return order;//_mapper.Map<TraceableOrderView>(order); 
            }
        }

        /// <summary>
        /// Оповестить веб сайт об измемениях (любых). Зовёт его ECom.Workflow
        /// </summary>
        /// <param name="basketGuid"></param>
        /// <param name="newStatusId"></param>
        /// <param name="requestHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ExpansionConsts.Common.App.Controllers.Workflow.SendOrderToFozzyShopWebSiteRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderData))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [WorkflowExceptionHandler]
        public async Task<OrderData> SendOrderToFozzyShopSite(
            [FromRoute] Guid basketGuid,
            [FromRoute] int newStatusId,
            [FromServices] IRequestHandler<SendOrderToFozzyShopSiteComand, OrderData> requestHandler,
            CancellationToken cancellationToken)
        {
            using (_logger.LoggingScope(basketGuid))
            {
                _logger.LogInformation("{msg} {data} {newStatusId}",
                    ExpansionConsts.Common.App.Controllers.Workflow.SendOrderToFozzyShopWebSiteRoute, basketGuid, newStatusId);

                var order = await requestHandler.Handle(
                    new SendOrderToFozzyShopSiteComand
                    {
                        BasketId = basketGuid,
                        NewStatusId = newStatusId,
                    }, cancellationToken);

                return order; // _mapper.Map<TraceableOrderView>(order);
            }
        }

        /// <summary>
        /// Оповестить Collecting.Fozzy Web Service сайт об измемения (сборки). Зовёт его ECom.Workflow
        /// </summary>
        /// <param name="basketGuid"></param>
        /// <param name="newStatusId"></param>
        /// <param name="requestHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ExpansionConsts.Common.App.Controllers.Workflow.SendOrderToFozzyShopCollectingServiceRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(OrderData))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [WorkflowExceptionHandler]
        public async Task<OrderData> SendOrderToFozzyShopColectingService(
            [FromRoute] Guid basketGuid,
            [FromRoute] int newStatusId,
            [FromServices] IRequestHandler<SendOrderToFozzyShopCollectingServiceComand, OrderData> requestHandler,
            CancellationToken cancellationToken)
        {
            using (_logger.LoggingScope(basketGuid))
            {
                _logger.LogInformation("{msg} {data} {newStatusId}",
                       ExpansionConsts.Common.App.Controllers.Workflow.SendOrderToFozzyShopCollectingServiceRoute, basketGuid, newStatusId);

                var order = await requestHandler.Handle(
                     new SendOrderToFozzyShopCollectingServiceComand
                     {
                         BasketId = basketGuid,
                         NewStatusId = newStatusId,
                     }, cancellationToken);

                return order; // _mapper.Map<TraceableOrderView>(order);
            }
        }



        [HttpGet(ExpansionConsts.Common.App.Controllers.Workflow.GetOrderRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TraceableOrderView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [WorkflowExceptionHandler]
        public async Task<TraceableOrderView> GetOrderByBasketId(
            Guid basketGuid,
            [FromServices] IRequestHandler<LoadTraceableOrderCommand, TraceableOrder> getOrderCommandHandler,
            CancellationToken cancellationToken)
        {
            using (_logger.LoggingScope(basketGuid))
            {
                _logger.LogInformation("{msg} {data}",
                  ExpansionConsts.Common.App.Controllers.Workflow.GetOrderRoute, basketGuid);

                var order = await getOrderCommandHandler.Handle(
                    new LoadTraceableOrderCommand()
                    {
                        BasketGuid = basketGuid,
                        WithItems = true,
                    }, cancellationToken);

                return _mapper.Map<TraceableOrderView>(order);
            }
        }



        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.Workflow.ThrowUnhandledExceptionRoute)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        [WorkflowExceptionHandler]
        public IActionResult ThrowUnhandled()
        {
            throw new Exception("Test unhandled exception");
        }

        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.Workflow.ThrowBusinessExceptionRoute)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [WorkflowExceptionHandler]
        public IActionResult ThrowBusiness()
        {
            throw new WorkflowException(WorkflowErrors.BusinessExceptionTest, "Тестовая ошибка!");
        }


        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.Workflow.HealthCheckRoute)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}