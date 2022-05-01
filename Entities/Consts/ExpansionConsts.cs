using Utils.Consts;

namespace Entities.Consts
{
    public static class ExpansionConsts
    {
        public static class Common
        {
            public static class App
            {
                public const string Name = "ECom.Expansion";
                public const string DefaultRoute = "ecom/expansion/";

                public static class Groups
                {
                    public static class Expansion
                    {
                        public const string GroupNameVersionOne = "ecom_expansion_v1";
                        public const string Description = Name;
                    }
                    public static class Workflow
                    {
                        public const string GroupNameVersionOne = "ecom_workflow_v1";
                        public const string Description = "ECom.Workflow";
                    }

                    public static class FozzyCollecting
                    {
                        public const string GroupNameVersionOne = "fozzy_collecting_v1";
                        public const string Description = "Fozzy.Collecting";
                    }
                }

                public static class Controllers
                {
                    public static class HealthCheck
                    {
                        public const string HealthCheckRoute = "/";
                        public const string Route = "health";
                        public const string Description = "Проверка жизнедеятельности";

                        public const string CatalogHealthCheckRoute = "checkCatalog";

                        public const string CreateOrderAndCollectRoute = "createOrderAndCollect";

                        public const string ThrowUnhandledExceptionRoute = "throwUnhandled";
                    }

                    public static class Collecting
                    {
                        public const string CollectingBaseRoute = "ecom/expansion/collecting/";
                        public const string Description = "Взаимодействие с сервисом сборки";
                        public const string HealthCheckRoute = "health";

                        public const string ChangeOrderRoute = "changeCollecting";
                        public const string ThrowUnhandledExceptionRoute = "throwUnhandled";
                        public const string ThrowBusinessExceptionRoute = "throwBusiness";
                    }
                    public static class Workflow
                    {
                        public const string WorkflowBaseRoute = "ecom/expansion/workflow/";
                        public const string Description = "Взаимодействие с сервисом бизнес логики";
                        
                        public const string HealthCheckRoute = "health";

                        public const string SendOrderToFozzyShopCollectingServiceRoute = 
                            "sendOrderToFozzyShopCollectingService/{basketGuid:Guid}/status/{newStatusId:int}";
                        
                        public const string SendOrderToFozzyShopWebSiteRoute =
                            "sendOrderToFozzyShopWebSite/{basketGuid:Guid}/status/{newStatusId:int}";

                        public const string AcceptCollectingRoute = "acceptCollecting/{basketGuid:Guid}";

                        public const string CancelCollectingRoute = "cancelCollecting/{basketGuid:Guid}";
                        public const string EnterCollectingRoute = "enterCollecting";

                        public const string ThrowUnhandledExceptionRoute = "throwUnhandled";
                        public const string ThrowBusinessExceptionRoute = "throwBusiness";

                        public const string GetOrderRoute = "order/{basketGuid:Guid}";
                    }
                }
            }


            public static class Exception
            {
                public const string Source = "Entities";
            }
        }
        public static class Order
        {
            public static class Exceptions
            {
                public const string OrderHasNoItemsException = "В заказе нет ни одной позиции";
            }
        }
    }
}
