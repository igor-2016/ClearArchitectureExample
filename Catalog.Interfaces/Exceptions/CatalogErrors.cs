using System.Net;
using Utils.Attributes;

namespace Catalog.Interfaces.Exceptions
{
    public enum CatalogErrors
    {
        [ResponseError(Message = "Lager не найден", HttpStatus = HttpStatusCode.NotFound)]
        BarcodeOrLagerIdRequired = 10001,

        [ResponseError(Message = "Нельзя найти ECom код доставки для Fozzy", HttpStatus = HttpStatusCode.BadRequest)]
        CannotMapDeliveryTypeFromFozzyToEcom = 10002,

        /// <summary>
        /// Товар не найден
        /// </summary>
        [ResponseError(Message = "Не найден товар по SKU", HttpStatus = HttpStatusCode.NotFound)]
        SKUNotFound = 10003,


        /// <summary>
        /// Неоднозначные данные результатов поиска артикула - найдено более одного товара
        /// </summary>
        [ResponseError(Message = "найдено несколько товаром с одним SKU", HttpStatus = HttpStatusCode.BadRequest)]
        MultipleSKUFound = 10004,


        [ResponseError(Message = "Ошибка серсиса каталога!", HttpStatus = HttpStatusCode.BadRequest)]
        CatalogServiceError = 10005,
    }
}
