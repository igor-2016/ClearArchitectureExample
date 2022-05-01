using ECom.Expansion.IntegrationTests.Clients.Workflow;
using ECom.Expansion.IntegrationTests.DataAccess.MsSql;
using Workflow.Ecom;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Services
{
    public class EComWorkflowServiceTest
    {
        //private static EComWorkflowService _eComWorkflowServiceService;

        public static EComWorkflowService GetEComWorkflowService(bool useTestDb)
        {
            //if (_eComWorkflowServiceService == null)
            //{
                var eComWorkflowServiceService = new EComWorkflowService(DbContextOptionsTest.GetSqlServerOptionsContext(useTestDb),
                    WorkflowTest.MockEComWorkflowServiceClient(),
                    TransformServiceTest.GetTransformService());
            //}
            return eComWorkflowServiceService;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GeEComWorkflowServiceTest(bool useTestDb)
        {
            var eComWorkflowServiceService = GetEComWorkflowService(useTestDb);
            Assert.IsType<EComWorkflowService>(eComWorkflowServiceService);
        }
    }
}
