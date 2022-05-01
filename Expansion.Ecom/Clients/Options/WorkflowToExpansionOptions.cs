using Utils.Sys.Options;

namespace Expansion.Ecom.Clients.Options
{
    /// <summary>
    /// Опции для подключения к сервису Экспансия через контроллер Workflow
    /// Имена методов смотри в ExpansionConsts.Common.App.Controllers.Workflow
    /// </summary>
    public class WorkflowToExpansionOptions : HttpClientOptions
    {
        public string EnterCollectingMethod { get; set; }
        
        public string AccepCollectingMethodFormat { get; set; }

        public string CancelCollectingMethodFormat { get; set; }

        public string SendOrderToFozzyCollectingServiceMethodFormat { get; set; }

        public string SendOrderToFozzyWebSiteMethodFormat { get; set; }
    }
}
