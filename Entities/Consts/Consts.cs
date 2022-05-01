namespace Entities.Consts
{
    public static class Consts
    {
        public static class GeneralErrors
        {
            public const string NotFozzyShopOrder = "Обрабатываем только заказы FozzyShop";
            
            public const string NotUpCollectingStatus = "Невозможно повторно начать отборку этого заказа";
            
            public const string FozzyShopOrderIsInvalid = "Ошибка заказа";

            public const string RequireCallCollectingStatus = "Обрабатываем только после согласования";

            public const string OneOrderPerCollecting = "Обрабатываем только один ордер на одну сборку";

            public const string OrderNotFound = "Не найден ни один ордер!";

            public const string CollectingIsNotStarted = "Сборка не начата!";
        }

       
    }
}
