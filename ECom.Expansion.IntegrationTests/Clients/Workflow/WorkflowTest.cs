using ECom.Expansion.IntegrationTests.Options;
using ECom.Expansion.TestHelpers;
using Entities.Models.Collecting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Workflow.Ecom.Clients;
using Workflow.Ecom.Clients.Options;
using Workflow.Interfaces.Clients;
using Workflow.Interfaces.Clients.Responses;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Clients.Workflow
{
    public class WorkflowTest
    {
        public static EComWorkflowServiceClient MockEComWorkflowServiceClient()
        {
            var loggerMock = LoggerHelper.GetLogger<IWorkflowServiceClient>();
            var eComWorkflowServiceOptions = EComWorkflowServiceOptionsTest.Get();

            var httpClient = HttpClientHelper.GetHttpClient(eComWorkflowServiceOptions.Value.BaseUrl);
            var client = new EComWorkflowServiceClient(httpClient, eComWorkflowServiceOptions, loggerMock.Object);
            return client;
        }

        public async Task<EmptyDataResult> SetStatusStart(Guid basketId, EComPicker picker)
        {
            CancellationToken cancellationToken = CancellationToken.None;
            var client = WorkflowTest.MockEComWorkflowServiceClient();
            var result = await client.OnChangeState(basketId, 913, 15, picker, cancellationToken);
            return result;
        }

        [Theory]
        [InlineData("74d24e34-f2bf-4312-81a2-3901bc7fe6fb")]
        public async Task SetStatusStartPositive(Guid basketId)
        {
            var picker = new EComPicker()
            {
                Id = 0,
                Name = "Tester"
            };

            var result = await SetStatusStart(basketId, picker);
            
            Assert.True(result.IsSuccess);
            Assert.False(result.HasError);
        }

        [Theory]
        [InlineData("1c35ce72-e707-4244-9378-8c1af41b2a08")]
        public async Task SetStatusStartNegative(Guid basketId)
        {
            var picker = new EComPicker()
            {
                Id = 0,
                Name = "Tester"
            };

            var result = await SetStatusStart(basketId, picker);

            Assert.False(result.IsSuccess);
            Assert.True(result.HasError);
        }
    }
}
