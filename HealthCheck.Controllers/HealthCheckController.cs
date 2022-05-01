using ApplicationServices.Interfaces;
using ApplicationServices.Interfaces.Responses;
using AutoMapper;
using ECom.Entities.Models;
using ECom.Types.ServiceBus;
using Entities.Consts;
using Expansion.Interfaces.Dto;
using HealthCheck.Controllers.Catalog.Requests;
using HealthCheck.Controllers.Catalog.Responses;
using HealthCheck.Controllers.Filters.Exceptions;
using HealthCheck.Controllers.Order.Requests;
using HealthCheck.UseCases.Catalog.Queries.SimpleCatalogItems;
using HealthCheck.UseCases.Database.Queries.CheckDbConnection;
using HealthCheck.UseCases.Tests.CreateOrderAndCollect;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HealthCheck.Controllers
{

    [ApiController]
    [ApiExplorerSettings(GroupName = ExpansionConsts.Common.App.Groups.Expansion.GroupNameVersionOne)]
    [Route(ExpansionConsts.Common.App.Controllers.HealthCheck.HealthCheckRoute)]
    [SwaggerTag(ExpansionConsts.Common.App.Controllers.HealthCheck.Description)]
    public class HealthcheckController : ControllerBase
    {
        private readonly IMapper _mapper;
        public HealthcheckController(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Вызов каталога товаров. 
        /// Если не указывать ничего, то вернёт один товар из каталога если он есть (LagerId = 117) 
        /// </summary>
        /// <param name="lagerId"></param>
        /// <param name="filialId"></param>
        /// <param name="merchant">пока не используется</param>
        /// <param name="deliveryType">(пока указывать 0!)</param>
        /// <seealso href="https://temabit.atlassian.net/wiki/spaces/ECS/pages/889258075/ECom">Типы доставки ECom</seealso>
        /// <returns></returns>
        [HttpPost]
        [Route(ExpansionConsts.Common.App.Controllers.HealthCheck.CatalogHealthCheckRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(CheckCatalogResponse))]
        [ProducesResponseType(StatusCodes.Status501NotImplemented, Type=typeof(EComError))]
        [ExpansionExceptionHandler]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<CheckCatalogResponse> CatalogPing(
            [FromBody] CheckCatalogRequest request,
            [FromServices] IRequestHandler<CheckSimpleCatalogItemsQuery, DataResult<CatalogInfo>> requestHandler, CancellationToken cancellationToken)
        {
            var requestValid = CheckCatalogRequest.Default;
            if (!request.isValid())
            {
                request = requestValid;
            }

            var result = await requestHandler.Handle(new CheckSimpleCatalogItemsQuery() 
            { 
                EComDeliveryType =  request.EComDeliveryType,
                Filial = request.Filial,
                Merchant = request.Merchant,
                LagerId = request.LagerId,
            }, cancellationToken);

            if (result.IsSuccess)
            {
                return new CheckCatalogResponse() { Request = request, CatalogInfo = result.Data };
            }

            throw result.AnError;
        }

        /// <summary>
        /// Тестовое создание ордера и его сборка
        /// </summary>
        /// <param name="testRequest"></param>
        /// <param name="executeTestCaseHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ExpansionConsts.Common.App.Controllers.HealthCheck.CreateOrderAndCollectRoute)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateOrderAndCollectResponse))]
        [ExpansionExceptionHandler]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<CreateOrderAndCollectResponse> TestCreateOrderAndCollect(
            [FromBody] CreateTestOrderAndCollectRequest testRequest,
            [FromServices] IRequestHandler<CreateOrderAndCollectCommand, CreateOrderAndCollectResponse> executeTestCaseHandler
            , CancellationToken cancellationToken)
        {
            var commandRequest = _mapper.Map<CreateOrderAndCollectRequest>(testRequest);
            return await executeTestCaseHandler.Handle(new CreateOrderAndCollectCommand() { Request = commandRequest }, cancellationToken);   
        }


        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.HealthCheck.Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Ping([FromServices] 
        IRequestHandler<CheckReadOnlyDbConnectionQuery, CheckDataResult<bool>> requestHandler, CancellationToken cancellationToken)
        {
            var result = await requestHandler.Handle(new CheckReadOnlyDbConnectionQuery() { Request = new DbConnectionInfoRequest() { } }, default);
            if (result.IsSuccess)
            {
                return Ok("OK");
            }

            var errorMessage = "Unknown error";
            if (result.HasError && result.AnError != null)
            {
                errorMessage = result.AnError.GetBaseException().Message;
            }
            return new ObjectResult(errorMessage) { StatusCode = StatusCodes.Status503ServiceUnavailable };
        }

        [HttpGet]
        [Route(ExpansionConsts.Common.App.Controllers.HealthCheck.ThrowUnhandledExceptionRoute)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ThrowUnhandled()
        {
            throw new Exception("Test unhandled exception");
        }
    }
}