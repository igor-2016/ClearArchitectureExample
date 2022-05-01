using Workflow.UseCases.Order;

namespace Workflow.UseCases.Consts
{
    public static class WorkflowConsts
    {
        public static class CommandInfo
        {
            public const string CreateCollectingCommandName = nameof(CreateOrderCommand);
            public const string AcceptReplacementsCommandName = nameof(AcceptReplacementsCommand);
            public const string SendOrderToFozzyShopSiteComandName = nameof(SendOrderToFozzyShopSiteComand);
            public const string SendOrderToFozzyShopCollectingServiceComandName = 
                nameof(SendOrderToFozzyShopCollectingServiceComand);

            public const string CancelOrderCommandName = nameof(CancelOrderCommand);

            //public const string AddItemCommandName = nameof(AddItemCommand);
            //public const string UpdateQtyCommandName = nameof(UpdateQtyCommand);
        }
    }
}