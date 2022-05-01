using Utils.Sys.Options;

namespace Workflow.Ecom.Clients.Options
{
    public class EComWorkflowServiceOptions : HttpClientOptions
    {
        public string TranformMethodFormat { get; set; }

        public string GetCurrentStateMethodFormat { get; set; }
    }
}
