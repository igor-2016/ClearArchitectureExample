using ECom.Expansion.TestHelpers;
using Workflow.Ecom.Clients.Options;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Options
{
    public class EComWorkflowServiceOptionsTest
    {
        public static MockOptions<EComWorkflowServiceOptions> Get()
        {
            var options = new MockOptions<EComWorkflowServiceOptions>(
                new EComWorkflowServiceOptions()
                {
                    BaseUrl = TestConsts.Workflow.UrlValid,
                    TranformMethodFormat = TestConsts.Workflow.TranformMethodFormat,
                    GetCurrentStateMethodFormat = TestConsts.Workflow.GetCurrentStateMethodFormat
                });

            return options;
        }

        [Fact]
        public void TestGetEComWorkflowServiceOptions()
        {
            var options = Get();
            Assert.Equal(options.Value.BaseUrl, TestConsts.Workflow.UrlValid);
            Assert.Equal(options.Value.TranformMethodFormat, TestConsts.Workflow.TranformMethodFormat);
            Assert.Equal(options.Value.GetCurrentStateMethodFormat, TestConsts.Workflow.GetCurrentStateMethodFormat);
        }
    }
}
