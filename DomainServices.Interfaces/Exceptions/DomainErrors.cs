using System.Net;
using Utils.Attributes;

namespace DomainServices.Interfaces.Exceptions
{

    public enum DomainErrors
    {
        [ResponseError(Message = "Для ECom кода не найден Fozzy код доставки", HttpStatus = HttpStatusCode.BadRequest)]
        NotFoundMapFromEComToFozzyDeliveryType = 9000,

        [ResponseError(Message = "Для ECom кода не найден Fozzy код оплаты", HttpStatus = HttpStatusCode.BadRequest)]
        NotFoundMapFromEComToFozzyPaymentType = 9001,

        [ResponseError(Message = "Не найден товар в каталоге", HttpStatus = HttpStatusCode.BadRequest)]
        NotFoundCatalogItem = 9002,

      
        [ResponseError(Message = "Время начала доставки неверно", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidDeliveryTimeFrom = 9009,


        [ResponseError(Message = "Время окончания доставки неверно", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidDeliveryTimeTo = 9010,

        [ResponseError(Message = "Дата доставки не указана", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidDeliveryDate = 9011,

        [ResponseError(Message = "Дата указана дата изменения ордера", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidDateModified = 9012,


        [ResponseError(Message = "Ожидается цифровой номер ордера", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidOrderIdAsInt = 9014,


        [ResponseError(Message = "Дата создания не указана", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidCreatedOrder = 9015,

        [ResponseError(Message = "Дата указана дата изменения позиции", HttpStatus = HttpStatusCode.BadRequest)]
        InvalidDateModifiedLine = 9016,

        [ResponseError(Message = "Сборщик не найден", HttpStatus = HttpStatusCode.NotFound)]
        NotFoundPicker = 9017,

        [ResponseError(Message = "Сборщик не найден в позиции", HttpStatus = HttpStatusCode.NotFound)]
        NotFoundPickerInLine = 9018,


        [ResponseError(Message = "Для Fozzy кода не найден ECom код доставки", HttpStatus = HttpStatusCode.BadRequest)]
        NotFoundMapFromFozzyToEComDeliveryType = 9019,

        [ResponseError(Message = "Для Fozzy кода не найден ECom код оплаты", HttpStatus = HttpStatusCode.BadRequest)]
        NotFoundMapFromFozzyToEComPaymentType = 9020,
    }

}
