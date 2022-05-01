namespace Collecting.Interfaces.Enums
{
    public enum FozzyPaymentType
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Оплата по безналичному расчету
        /// </summary>
        Payment29 = 29,

        /// <summary>
        /// Оплата наличными при получении
        /// </summary>
        Payment89 = 89,

        /// <summary>
        /// Оплата наличными при получении в отделении НП
        /// </summary>
        Payment147 = 147,

        /// <summary>
        /// Liqpay
        /// </summary>
        Payment161 = 161,

        /// <summary>
        /// Liqpay
        /// </summary>
        Payment213 = 213,

        /// <summary>
        /// Оплата наличными при получении
        /// </summary>
        Payment239 = 239,

        /// <summary>
        /// Оплата по безналичному расчету
        /// </summary>
        Payment246 = 246,

        /// <summary>
        /// Y.CMS Prestashop
        /// </summary>
        Payment273 = 273,

        /// <summary>
        /// Оплата в отделении Новой Почты
        /// </summary>
        Payment336 = 336
    }
}
