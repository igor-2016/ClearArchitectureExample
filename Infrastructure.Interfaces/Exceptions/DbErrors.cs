using System.Net;
using Utils.Attributes;

namespace DataAccess.Interfaces
{
    public enum DbErrors
    {
        /// <summary>
        /// Необработанная ошибка доступа к базе данных
        /// </summary>
        [ResponseError(Message = "Общая ошибка доступа к базе данных", HttpStatus = HttpStatusCode.InternalServerError)]
        UnhandledError = 8000,

        [ResponseError(Message = "Общая ошибка доступа к базе данных (тип: {0})", HttpStatus = HttpStatusCode.InternalServerError)]
        UnhandledErrorWithEntityType = 8001,

        [ResponseError(Message = "Общая ошибка доступа к базе данных (тип: {0}, id: {1})", HttpStatus = HttpStatusCode.InternalServerError)]
        UnhandledErrorWithEntityKey = 8002,

        [ResponseError(Message = "Общая ошибка доступа к базе данных (тип: {0}, spec: {1})", HttpStatus = HttpStatusCode.InternalServerError)]
        UnhandledErrorWithEntityTypeAndSpecification = 8003,


        [ResponseError(Message = "Сущность ({0}) пуста", HttpStatus = HttpStatusCode.BadRequest)]
        EntityIsNull = 8010,


        [ResponseError(Message = "Попытка обновить cущность ({0}, id: {1}) версии '{2}' на версию '{3}'", HttpStatus = HttpStatusCode.InternalServerError)]
        InvalidEntityVersion = 8011,


        [ResponseError(Message = "Сущность ({0}, id: {1}) не найдена", HttpStatus = HttpStatusCode.NotFound)]
        EntityNotFound = 8012,


        /// <summary>
        /// Проверить | !
        /// </summary>
        [ResponseError(Message = "Общая ошибка доступа к базе данных", HttpStatus = HttpStatusCode.InternalServerError)]
        GeneralError = UnhandledError | UnhandledErrorWithEntityType | UnhandledErrorWithEntityKey,
    }
}
