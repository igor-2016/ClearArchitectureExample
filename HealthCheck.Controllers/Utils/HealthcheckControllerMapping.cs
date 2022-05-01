using AutoMapper;
using Expansion.Interfaces.Dto;
using HealthCheck.Controllers.Order.Requests;

namespace HealthCheck.Controllers
{
    public class HealthcheckControllerMapping : Profile
    {
        public HealthcheckControllerMapping()
        {
            CreateMap<CreateTestOrderAndCollectRequest, CreateOrderAndCollectRequest>();
        }
    }
}
