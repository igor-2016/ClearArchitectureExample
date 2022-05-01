using System.Net;
using Utils.Attributes;

namespace WebSiteService.Interfaces
{
    public enum SiteErrors
    {
        /// <summary>
        /// Необработанная ошибка при обращении к сайту
        /// </summary>
        [ResponseError(Message = "Необработанная ошибка при обращении к сайту", HttpStatus = HttpStatusCode.BadRequest)]
        UnhandledExceptionWebSite = 8000,


        /// <summary>
        /// Ответ не OK
        /// </summary>
        [ResponseError(Message = "Ошибка обновления ордера на сайте", HttpStatus = HttpStatusCode.BadRequest)]
        ErrorWebSite = 8001,


        /// <summary>
        /// Обработанная ошибка сайта
        /// </summary>
        [ResponseError(Message = "Ошибка обновления ордера на сайте", HttpStatus = HttpStatusCode.BadRequest)]
        BusinessErrorWebSite = 8002,

        /// <summary>
        /// Обработанная ошибка сайта
        /// </summary>
        [ResponseError(Message = "Некорректный ответ сайта", HttpStatus = HttpStatusCode.BadRequest)]
        FormatErrorWebSite = 8003,
    }
}
