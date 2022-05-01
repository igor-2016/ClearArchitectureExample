using Expansion.Interfaces;
using Expansion.Interfaces.Dto;
using MediatR;

namespace HealthCheck.UseCases.Tests.CreateOrderAndCollect
{
    public class CreateOrderAndCollectCommand: IRequest<CreateOrderAndCollectResponse>
    {
        public CreateOrderAndCollectRequest Request { get; set; }
    }

    public class CreateOrderAndCollectCommandHandler : 
        IRequestHandler<CreateOrderAndCollectCommand, CreateOrderAndCollectResponse>
    {
        private readonly IExpansionService _expansionService;
       

        public CreateOrderAndCollectCommandHandler(IExpansionService expansionService)
        {
            _expansionService = expansionService;
        }

        public async Task<CreateOrderAndCollectResponse> Handle(CreateOrderAndCollectCommand request, CancellationToken cancellationToken)
        {
            var result = await _expansionService.DoTestScenarioCreateOrderAndCollectWithReplacements(request.Request, cancellationToken);
            return result;
        }
    }
}