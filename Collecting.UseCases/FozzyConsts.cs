namespace Collecting.UseCases
{
    public static class FozzyConsts
    {
        

        public static class CommandInfo
        {
            //public const string SendOrderToFozzyShopServiceCommandName = nameof(FozzyShopSendOrderToFozzyShopCollectingServiceComand);
            //public const string SendOrderToFozzyShopSiteComand  = nameof(FozzyShopSendOrderToFozzyShopSiteComand);
            public const string ReadyToCheckCollectingCommandName = nameof(ReadyToCheckCollectingCommand);
            public const string StartCollectingCommandName = nameof(StartCollectingCommand);
            public const string RestartCollectingCommandName = nameof(RestartCollectingCommand);
            public const string OfferReplacementsCommandName = nameof(OfferReplacementsCommand);
            
            public const string DoneCollectingCommandName = nameof(DoneCollectingCommand);
            public const string DefaultCollectingCommandName = nameof(DefaultCollectingCommand);

            //public const string AggregatedChangeCollectingCommandName = nameof(FozzyShopAggChangeCollectingCommand);


            //public const string AddItemCommandName = nameof(AddItemCommand);
            //public const string UpdateQtyCommandName = nameof(UpdateQtyCommand);
            //public const string SetPickedQtyCommandName = nameof(SetPickedQtyCommand);
            //public const string RemoveItemCommandName = nameof(RemoveItemCommand);
            //public const string AddItemFromReplacementsCommandName = nameof(AddItemFromReplacementsCommand);
            //public const string ChangeItemFromReplacementsCommandName = nameof(ChangeItemFromReplacementsCommand);
            //public const string RemoveItemFromReplacementsCommandName = nameof(RemoveItemFromReplacementsCommand);
            public const string LookupOrderByOrderIdCommandName = nameof(LookupOrderByOrderNumberCommand);


             //public const string UpdateFozzyShopCommandName = nameof(FozzyShopUpdateOrderStatusCommand);

        }
        //public static class EventHandlerInfo
        //{
        //    public const string AddItemFromReplacementsEventHandlerName = nameof(AddItemFromReplacementsEventHandler);

        //}

    }
}
