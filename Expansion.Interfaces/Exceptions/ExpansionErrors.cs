using System.Net;
using Utils.Attributes;

namespace Expansion.Interfaces
{
    public enum ExpansionErrors
    {

        /// <summary>
        /// Необработанная ошибка
        /// </summary>
        [ResponseError(Message = "Необработанная ошибка", HttpStatus = HttpStatusCode.InternalServerError)]
        UnhandledException = 11000,

        [ResponseError(Message = "Ошибка обработки Fozzy ордера", HttpStatus = HttpStatusCode.InternalServerError)]
        FozzyOrderError = 11001,

        [ResponseError(Message = "Ошибка обработки Fozzy ордера после сборки", HttpStatus = HttpStatusCode.InternalServerError)]
        FozzyOrderErrorAfterDoneCollecting = 11002,


        [ResponseError(Message = "Ошибка обработки Fozzy ордера после предложения замен", HttpStatus = HttpStatusCode.InternalServerError)]
        FozzyOrderErrorAfterOfferReplacements = 11003,

        [ResponseError(Message = "Не дождались отправки замен оператору", HttpStatus = HttpStatusCode.InternalServerError)]
        WaitForSendOfferReplacementsToOperator = 11004,

        [ResponseError(Message = "Не дождались получения подтверждённых замен от оператора", HttpStatus = HttpStatusCode.InternalServerError)]
        WaitForSendAcceptReplacementsTExpansion = 11005,

        [ResponseError(Message = "Ошибка обработки Fozzy ордера на старте после замен", HttpStatus = HttpStatusCode.InternalServerError)]
        FozzyOrderErrorStartAfterReplacements = 11006,

        [ResponseError(Message = "Не дождались создания нового ордера для сборки", HttpStatus = HttpStatusCode.InternalServerError)]
        WaitForCreateNewCollectingOrderError = 11007,

        [ResponseError(Message = "Не дождались старта сборки ордера", HttpStatus = HttpStatusCode.InternalServerError)]
        WaitForStartCollectingOrderError = 11008,

        [ResponseError(Message = "Не дождались повторного старта сборки ордера", HttpStatus = HttpStatusCode.InternalServerError)]
        WaitForStartAgainCollectingOrderError = 11009,

        [ResponseError(Message = "Не дождались окончания сборки ордера", HttpStatus = HttpStatusCode.InternalServerError)]
        WaitForDoneCollectingOrderError = 11010,

        [ResponseError(Message = "Не дождались установки готовности проверки сборки ордера", HttpStatus = HttpStatusCode.InternalServerError)]
        WaitForReadyToCheckCollectingOrderError = 11011,
    }
}
