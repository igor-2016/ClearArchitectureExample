namespace Utils.Consts
{
    public static class CommonConsts
    {
        public static class Project
        {
            public const string ServiceSource = "ECom.Expansion";

            public const string UnknownService = ServiceSource + ": ** Unknown **";
            public const string Entities = ServiceSource + ": Entities";
            public const string BasketService = ServiceSource + ": BasketService";
            public const string CollectingService = ServiceSource + ": CollectingService";
            public const string WorkflowService = ServiceSource + ": WorkflowService";
            public const string DataAccess = ServiceSource + ": DataAccess";
            public const string WebSite = ServiceSource + ": FozzyShopWebSite";
            public const string WebService = ServiceSource + ": FozzyShopWebService";

            public const string CatalogService = ServiceSource + ": CatalogService";
            public const string ExpansionService = ServiceSource + ": ExpansionService";

        }

        public static class Subsystem
        {
            public const string Unknown = "ECom.Expansion";
            public const string ECom = "ECom.Core";
            public const string EComCatalog = "ECom.Catalog";
            public const string EComWorkflow = "ECom.Workflow";
            public const string EComOms = "ECom.OMS";
            public const string CollectFozzy = "Collect.Fozzy";
            public const string WebSitePresta = "WebSite.Presta";
        }
    }
}
