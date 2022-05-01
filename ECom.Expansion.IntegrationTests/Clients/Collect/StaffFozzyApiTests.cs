using AutoMapper;
using Collecting.Fozzy.Clients;
using Collecting.Fozzy.Clients.Options;
using Collecting.Interfaces.Clients;
using DomainServices.Implementation;
using ECom.Expansion.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ECom.Expansion.IntegrationTests.Clients.Collect
{

    /*
        - есть метод FozzyShopOrderService.svc/GetIMFilials - возвращает перечень филиалов интернет-магазина Гипера:

        {"ConfirmResponse":{"errorCode":"0","errorMessage":""},"Filials":[322,382,510,1292,1614,1674]}

        - есть метод FozzyShopOrderService.svc/GetIMFilialEmployes?filialId=1614 - возвращает перечень сотрудников (структура EmploeeInfo) для заданного филиала

        https://s-kv-center-v20.officekiev.fozzy.lan:8907/FozzyShopOrderTest/FozzyShopOrderService.svc/IsAlive
        "AssemblyVersion":"1.0.12.5"

        https://s-kv-center-v20.officekiev.fozzy.lan:8907/FozzyShopOrderTest/FozzyShopOrderService.svc/GetEmployeeInfo?peopleINN=2921408973
        {"ConfirmResponse":{"errorCode":"0","errorMessage":"Ok"},"EmployeeInfo":{"globalUserId":86505,"peopleFullName":"Ховменець Микола Володимирович","peopleINN":"2921408973","peopleMobilePhone":"+380999036923","staffCode":"00097713"}}

        https://s-kv-center-v20.officekiev.fozzy.lan:8907/FozzyShopOrderTest/FozzyShopOrderService.svc/GetEmployeeInfo?peopleINN=86505
        {"ConfirmResponse":{"errorCode":"1","errorMessage":"Сотрудник с ИНН 86505 не найден в действующем штатном расписании."},"EmployeeInfo":{"globalUserId":null,"peopleFullName":null,"peopleINN":null,"peopleMobilePhone":null,"staffCode":null}}

        https://s-kv-center-v20.officekiev.fozzy.lan:8907/FozzyShopOrderTest/FozzyShopOrderService.svc/GetEmployeeInfo?globalUserId=86505
        {"ConfirmResponse":{"errorCode":"0","errorMessage":"Ok"},"EmployeeInfo":{"globalUserId":86505,"peopleFullName":"Ховменець Микола Володимирович","peopleINN":"2921408973","peopleMobilePhone":"+380999036923","staffCode":"00097713"}}
     */

    public class StaffFozzyApiTests
    {
        TestDataGenerator _gn;

        private const string GetByGlobalIdMethodFormat = "/FozzyShopOrderService.svc/GetEmployeeInfo?globalUserId={0}";

        private const string GetByInnMethodFormat = "/FozzyShopOrderService.svc/GetEmployeeInfo?peopleINN={0}";

        private const string BaseUrl_Valid = "https://s-kv-center-x57.officekiev.fozzy.lan:1449/";

        //IMapper _mapper;
        public StaffFozzyApiTests()
        {
            _gn = new TestDataGenerator();
            //_mapper = new TestAutoMapper().GetMapper();
        }

        [Theory]
        [InlineData("2921408973", 86505, true)]
        [InlineData("111", 1, false)]
        public async Task Can_GetStaff_ByInn(string inn, int globalUserId, bool isValid)
        {
            CancellationToken cancellationToken = CancellationToken.None;   
            var loggerMock = new Mock<ILogger<IFozzyStaffServiceClient>>();
            var options = new MockOptions<FozzyShopStaffServiceOptions>(
                new FozzyShopStaffServiceOptions()
                {
                    BaseUrl = BaseUrl_Valid,
                    GetByGlobalIdMethodFormat = GetByGlobalIdMethodFormat,
                    GetByInnMethodFormat = GetByInnMethodFormat
                });

            //var inn = "2921408973"; //"3630609774";// 
            //var globalUserId = 86505;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
                var fozzyStaffWebServiceClient = new FozzyStaffServiceClient(httpClient, options, loggerMock.Object);
                var result = await fozzyStaffWebServiceClient.GetStaffInfoByInn(inn, cancellationToken);

                if (isValid)
                {
                    Assert.True(result.IsSuccess);
                    Assert.False(result.HasError);
                    Assert.NotNull(result.Staff);
                    Assert.NotNull(result.Staff.EmployeeInfo);
                    Assert.NotNull(result.Staff.ConfirmResponse);

                    Assert.Equal(globalUserId, result.Staff.EmployeeInfo.globalUserId);
                }
                else
                {
                    Assert.False(result.IsSuccess);
                    Assert.True(result.IsNotFound);
                    Assert.Null(result.Staff);
                }

            }
        }

        [Theory]
        [InlineData("2921408973", 86505, true, false)]
        [InlineData("2921408973", 1, false, false)]
        [InlineData("2921408973", -1, false, true)]
        public async Task Can_GetStaffByGlobalUserId(string inn, int globalUserId, bool isValid, bool isError)
        {
            CancellationToken cancellationToken = new CancellationToken();
            var loggerMock = new Mock<ILogger<IFozzyStaffServiceClient>>();
            var options = new MockOptions<FozzyShopStaffServiceOptions>(
                new FozzyShopStaffServiceOptions()
                {
                    BaseUrl = BaseUrl_Valid,
                    GetByGlobalIdMethodFormat = GetByGlobalIdMethodFormat,
                    GetByInnMethodFormat = GetByInnMethodFormat
                });


            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
                var client = new FozzyStaffServiceClient(httpClient, options, loggerMock.Object);
                var result = await client.GetStaffInfoByGlobalUserId(globalUserId, cancellationToken);

                if (isValid)
                {
                    Assert.True(result.IsSuccess);
                    Assert.False(result.HasError);
                    Assert.NotNull(result.Staff);
                    Assert.NotNull(result.Staff.EmployeeInfo);
                    Assert.NotNull(result.Staff.ConfirmResponse);
                    Assert.Equal(inn, result.Staff.EmployeeInfo.peopleINN);
                }
                else
                {
                    if (isError)
                    {
                        Assert.False(result.IsSuccess);
                        Assert.False(result.IsNotFound);
                        Assert.True(result.HasError);
                        Assert.NotNull(result.AnError);
                        // TODO check Collecting error code!!
                    }
                    else
                    {
                        Assert.False(result.IsSuccess);
                        Assert.True(result.IsNotFound);
                        Assert.Null(result.Staff);
                    }
                }
            }

        }
    }
}
