using System.Net;
using Utils.Attributes;

namespace Basket.Interfaces
{
    public enum BasketErrors
    {
      
        /// <summary>
        /// Заказ не FozzyShop
        /// </summary>
        [ResponseError(Message = "The current order is not Fozzy Shop order", HttpStatus = HttpStatusCode.BadRequest)]
        NotFozzyShopOrder = 7000,

        /// <summary>
        /// FozzyShop заказ не найден
        /// </summary>
        [ResponseError(Message = "Fozzy Shop order is invalid", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidFozzyShopOrder = 7001,

        /// <summary>
        /// FozzyShop конвертация справочных данных
        /// </summary>
        [ResponseError(Message = "Fozzy Shop invalid enum convertion", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidFozzyShopEnumConvertion = 7002,

        /// <summary>
        /// FozzyShop статус не обрабатывается
        /// </summary>
        [ResponseError(Message = "Fozzy Shop invalid status", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidFozzyShopInvalidStatus = 7003,

        /// <summary>
        /// FozzyShop поддерживает одиночную сборку
        /// </summary>
        [ResponseError(Message = "Fozzy Shop one order per collecting", HttpStatus = HttpStatusCode.BadRequest)]
        OneCollectingPerOrder = 7004,

        /// <summary>
        /// FozzyShop сборка не начата
        /// </summary>
        [ResponseError(Message = "Fozzy Shop collecting is not started", HttpStatus = HttpStatusCode.BadRequest)]
        CollectingIsNotStarted = 7005,

        /// <summary>
        /// FozzyShop не найдено соответсвие между global user id и ИНН
        /// </summary>
        [ResponseError(Message = "Fozzy Shop global user id and INN mapping not found", HttpStatus = HttpStatusCode.NotFound)]
        GlobalUserIdInnMappingNotFound = 7006,


        /// <summary>
        /// Ошибочный номер заказа Fozzy Shop
        /// </summary>
        [ResponseError(Message = "Wrong orderId", HttpStatus = HttpStatusCode.BadRequest)]
        WrongOrderId = 7007,


        [ResponseError(Message = "Таймслот не найден", HttpStatus = HttpStatusCode.NotFound)]
        NotFoundTimeSlot = 7008,

        [ResponseError(Message = "Ошибка при попытке создать ордер", HttpStatus = HttpStatusCode.BadRequest)]
        CreateBasketOrderError = 7009,
    }
}
