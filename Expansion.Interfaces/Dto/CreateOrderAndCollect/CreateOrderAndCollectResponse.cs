using ApplicationServices.Interfaces.Models;
using ECom.Entities.Models;
using ECom.Entities.Models.Workflow;
using ECom.Types.ServiceBus;
using ECom.Types.TimeSlots;
using ECom.Types.Views;
using Entities.Models.Collecting;
using Entities.Models.Expansion;

namespace Expansion.Interfaces.Dto
{
    public class CreateOrderAndCollectResponse
    {
        
        public IList<CreateOrderAndCollectUseCases> UsesCasesPassed { get; set; } = new List<CreateOrderAndCollectUseCases>();

        public CreateOrderAndCollectUseCases StartedUsesCase { get; set; } = CreateOrderAndCollectUseCases.None;

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool Cancelled { get; set; } = false;

        public bool Success => (!(BusinessErrors?.Any() ?? true) && !(UnhandledErrors?.Any() ?? true));

        public TimeSlotView TimeSlot { get; set; }  

        public CreateOrderAndCollectRequest Request { get; set; }

        public BasketView NewEComOrder { get; set; }

        public WorkflowOrderState? CreateNewEComOrderWorkflowState { get; set; }

        public FozzyCollectableOrderInfo EComToCollectOrder { get; set; }

        public TraceableOrder NewExpansionOrder { get; set; }

        public OrderData NewFozzyOrder { get; set; }

        public OrderData StartedFozzyOrder { get; set; }

        public TraceableOrderView StartedExpansionOrder { get; set; } 
        
        public OrderData CollectedFozzyOrder { get; set; }

        public TraceableOrderView CollectedExpansionOrder { get; set; }

        public BasketView CollectedEComOrder { get; set; }

        public OrderData OfferReplacementsFozzyOrder { get; set; }

        public TraceableOrderView OfferReplacementsExpansionOrder { get; set; }

        public TraceableOrder AcceptingExpansionOrder { get; set; }

        public TraceableOrder AcceptedExpansionOrder { get; set; }


        public TraceableOrder CancelExpansionOrder { get; set; }

        public OrderData AcceptedFozzyOrder { get; set; }

        public OrderData StartedAfterReplacementsFozzyOrder { get; set; }

        public TraceableOrderView StartedAfterReplacementsExpansionOrder { get; set; }

        public TraceableOrderView ReadyToCheckCollectingExpansionOrder { get; set; }

        public OrderData ReadyToCheckCollectingFozzyOrder { get; set; }
        public IList<EComError> BusinessErrors { get; set; } = new List<EComError>();

        public IList<EComError> UnhandledErrors { get; set; } = new List<EComError>();

        
    }
}
