namespace ECom.Expansion.TestHelpers
{
    public class TestConsts
    {
        public static class Workflow
        {
            public const string UrlValid = "https://workflow.ecom.test.fozzy.lan";

            public const string TranformMethodFormat = "api/ecom/order/{0}/state/{1}/transform/{2}";

            public const string GetCurrentStateMethodFormat = "api/ecom/order/{0}/state";
        }

        public static class Expansion
        {
            public const string LocalServiceBaseUrl = "https://localhost:7141/";
            public const string TestServiceBaseUrl = "https://expansion.ecom.test.fozzy.lan/";

        }
    }
}
