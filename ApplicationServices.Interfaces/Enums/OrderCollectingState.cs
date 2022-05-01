
using System.ComponentModel;

namespace ApplicationServices.Interfaces.Enums
{
    public enum OrderCollectingState
    {
        /// <summary>
        /// Неизвестное состояние
        /// </summary>
        [Description("Невідомий")]
        Unknown = 0,

        /// <summary>
        /// Новый заказ
        /// Remarks:
        ///     Заказ на отборку сформирован из ECom - заказ еще никем не принят в обработку
        ///     - состояние заказа RO;
        /// </summary>
        [Description("Новий")]
        New = 1,

        /// <summary>
        /// Заказ кем-то обрабатывается
        /// </summary>
        [Description("Збирається")]
        Collecting = 5,

        /// <summary>
        /// Заказ обработан
        /// </summary>
        [Description("Зібраний")]
        Collected = 9,

        /// <summary>
        /// Заказ еще недоступен для сборки/готовки
        /// </summary>
        [Description("Очікується")]
        Waiting = 10,

        /// <summary>
        /// Заказ на согласовании
        /// </summary>
        [Description("На согласовании")]
        CallCollecting = 11,

        /// <summary>
        /// Продолжение сборки после согласования(й)
        /// </summary>
        [Description("Продолжение сборки")]
        UpCollecting = 12,

        /// <summary>
        /// По заказу завершена сборка и наапечатан чек
        /// </summary>
        [Description("Завершений")]
        Done = 20,

        /// <summary>
        /// Заказ отменен
        /// </summary>
        [Description("Відхилений")]
        Refused = 998
    }
}
