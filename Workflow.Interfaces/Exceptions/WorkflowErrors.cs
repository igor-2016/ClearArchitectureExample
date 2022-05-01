using System.Net;
using Utils.Attributes;

namespace Workflow.Interfaces.Exceptions
{

    public enum WorkflowErrors
    {

 
        [ResponseError(Message = "Для ECom кода не найден Fozzy код доставки", HttpStatus = HttpStatusCode.BadRequest)]
        NotFoundMapFromEComToFozzyDeliveryType = 9000,

        [ResponseError(Message = "Для ECom кода не найден Fozzy код оплаты", HttpStatus = HttpStatusCode.BadRequest)]
        NotFoundMapFromEComToFozzyPaymentType = 9001,


        [ResponseError(Message = "Wrong basketGuid", HttpStatus = HttpStatusCode.BadRequest)]
        WrongBasketGuid = 9002,

        [ResponseError(Message = "Ордер не найден", HttpStatus = HttpStatusCode.NotFound)]
        OrderNotFound = 9003,

        [ResponseError(Message = "Ордер должен быть в статусе согласования (914)", HttpStatus = HttpStatusCode.NotFound)]
        InvalidOrderReplacementStatus = 9004,

        [ResponseError(Message = "Необработанная ошибка при смене статуса ECom.Workflow", HttpStatus = HttpStatusCode.InternalServerError)]
        UnhandledExceptionOnChangeWorkflowStatus = 9005,

        [ResponseError(Message = "Ошибка при смене статуса ECom.Workflow", HttpStatus = HttpStatusCode.InternalServerError)]
        PublicErrorOnChangeWorkflowStatus = 9006,


        [ResponseError(Message = "Не указан ECom id товара", HttpStatus = HttpStatusCode.BadRequest)]
        SkuNotFoundEComItemId = 9007,
        
        [ResponseError(Message = "Нужно указать Lager", HttpStatus = HttpStatusCode.BadRequest)]
        LagerIdRequired = 9008,

        /// <summary>
        /// Товар не найден
        /// </summary>
        [ResponseError(Message = "Не найден товар по SKU", HttpStatus = HttpStatusCode.NotFound)]
        SKUNotFound = 9009,


        /// <summary>
        /// Неоднозначные данные результатов поиска артикула - найдено более одного товара
        /// </summary>
        [ResponseError(Message = "найдено несколько товаром с одним SKU", HttpStatus = HttpStatusCode.BadRequest)]
        MultipleSKUFound = 9010,

        /// <summary>
        /// Неоднозначные данные результатов поиска артикула - найдено более одного товара
        /// </summary>
        [ResponseError(Message = "Неверное количество", HttpStatus = HttpStatusCode.BadRequest)]
        WrongQty = 9011,


        [ResponseError(Message = "Позиция товара не найдена", HttpStatus = HttpStatusCode.NotFound)]
        NotFoundItem = 9012,


        /// <summary>
        /// Невозможно изменить кол-во - итоговое кол-во меньше нуля
        /// </summary>
        [ResponseError(Message = "Result qty less zero", HttpStatus = HttpStatusCode.BadRequest)]
        ResultQtyLessZero = 9014,

        /// <summary>
        /// Для интерфейса контроллера
        /// </summary>
        [ResponseError(Message = "Для проверки интерфейса ошибки", HttpStatus = HttpStatusCode.BadRequest)]
        BusinessExceptionTest = 9015,

        [ResponseError(Message = "Источник обновления позиции в заказе не найден", HttpStatus = HttpStatusCode.NotFound)]
        NotFoundSourceOfChangesItem = 9016,

        [ResponseError(Message = "Изменение не найдено", HttpStatus = HttpStatusCode.NotFound)]
        NotFoundChanges = 9017,

        [ResponseError(Message = "Цель обновления позиции в заказе не найдена", HttpStatus = HttpStatusCode.NotFound)]
        NotFoundTargetItem = 9018,

        [ResponseError(Message = "Ордер отклонён", HttpStatus = HttpStatusCode.BadRequest)]
        OrderRefused = 9019,

        [ResponseError(Message = "Position not found", HttpStatus = HttpStatusCode.NotFound)]
        PositionNotFound = 9020,

        [ResponseError(Message = "Ордер уже был ранее в сборке", HttpStatus = HttpStatusCode.BadRequest)]
        OrderIsAlreadyInCollecting = 9021,

        [ResponseError(Message = "Нет сборки в данный момент", HttpStatus = HttpStatusCode.BadRequest)]
        OrderIsNotInCollecting = 9022,
    }

}
