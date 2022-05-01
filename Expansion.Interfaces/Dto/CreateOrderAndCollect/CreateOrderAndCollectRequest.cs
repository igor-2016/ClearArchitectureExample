using ECom.Types.Delivery;
using ECom.Types.DTO;
using ECom.Types.Orders;
using Expansion.Interfaces.Dto.Requests;
using Expansion.Interfaces.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Expansion.Interfaces.Dto
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreateOrderAndCollectUseCases
    {
        None = 0,

        CreateNewEComOrderAndTimeSlot = 1,

        CheckForNewExpansionOrderFromEComOrder = 2, 

        EmulateFozzyCollectingService = 4,

        CheckNewTraceableOrder = 8,

        CheckNewFozzyOrderInFozzyCollectingService = 16,

        EmulateAcceptFromFzClient = 32,

        StartCollecting = 64,

        CheckStartedFozzyOrderInFozzyCollectingService = 128,

        DoneCollecting = 256,

        CheckCollectedFozzyOrderInFozzyCollectingService = 512,

        CheckCollectedEcomOrder = 1024,

        OfferReplacements = 2048,

        AcceptReplacements = 4096,

        StartCollectingAgain = 8192,

        CancelCollecting = 16384,

        ReadyToCheckCollecting = 32768,

        CheckReadyToCheckCollectingFozzyOrderInFozzyCollectingService = 65536,

        CreateOrderCollectWithReplacements = CreateNewEComOrderAndTimeSlot | CheckForNewExpansionOrderFromEComOrder 
            | CheckNewTraceableOrder
            | CheckNewFozzyOrderInFozzyCollectingService | StartCollecting
            | CheckStartedFozzyOrderInFozzyCollectingService | DoneCollecting | CheckCollectedFozzyOrderInFozzyCollectingService
            | OfferReplacements | AcceptReplacements | StartCollectingAgain 
            | CheckCollectedEcomOrder
            | EmulateFozzyCollectingService
            | EmulateAcceptFromFzClient
            | ReadyToCheckCollecting
            | CheckReadyToCheckCollectingFozzyOrderInFozzyCollectingService,

        CreateOrderCollectWithReplacementsWithoutEmulation = CreateNewEComOrderAndTimeSlot | CheckForNewExpansionOrderFromEComOrder
            | CheckNewTraceableOrder
            | CheckNewFozzyOrderInFozzyCollectingService | StartCollecting
            | CheckStartedFozzyOrderInFozzyCollectingService | DoneCollecting | CheckCollectedFozzyOrderInFozzyCollectingService
            | OfferReplacements | AcceptReplacements | StartCollectingAgain
            | CheckCollectedEcomOrder,
            //| EmulateFozzyCollectingService
            //| EmulateAcceptFromFzClient,

        CreateOrderAndCollect = CreateNewEComOrderAndTimeSlot | CheckForNewExpansionOrderFromEComOrder | CheckNewTraceableOrder
            | CheckNewFozzyOrderInFozzyCollectingService | StartCollecting
            | CheckStartedFozzyOrderInFozzyCollectingService | DoneCollecting | CheckCollectedFozzyOrderInFozzyCollectingService
            | CheckCollectedEcomOrder
            | EmulateFozzyCollectingService 
            | EmulateAcceptFromFzClient
            | ReadyToCheckCollecting 
            | CheckReadyToCheckCollectingFozzyOrderInFozzyCollectingService,


        CreateOrderAndCollectWitoutEmulation = CreateNewEComOrderAndTimeSlot | CheckForNewExpansionOrderFromEComOrder | CheckNewTraceableOrder
            | CheckNewFozzyOrderInFozzyCollectingService | StartCollecting
            | CheckStartedFozzyOrderInFozzyCollectingService | DoneCollecting | CheckCollectedFozzyOrderInFozzyCollectingService
            | CheckCollectedEcomOrder,
            //| EmulateFozzyCollectingService
            //| EmulateAcceptFromFzClient,


        CreateOrderStartAndCancel = CreateNewEComOrderAndTimeSlot | CheckForNewExpansionOrderFromEComOrder | CheckNewTraceableOrder
            | CheckNewFozzyOrderInFozzyCollectingService | StartCollecting
            | CheckStartedFozzyOrderInFozzyCollectingService //| DoneCollecting unrem to check refuse error 
            | EmulateFozzyCollectingService
            | CancelCollecting,

        All = CreateNewEComOrderAndTimeSlot | CheckForNewExpansionOrderFromEComOrder 
            | CheckNewTraceableOrder
            | CheckNewFozzyOrderInFozzyCollectingService | StartCollecting
            | CheckStartedFozzyOrderInFozzyCollectingService | DoneCollecting | CheckCollectedFozzyOrderInFozzyCollectingService
            | OfferReplacements | AcceptReplacements | StartCollectingAgain 
            | CheckCollectedEcomOrder
            | EmulateFozzyCollectingService
            | EmulateAcceptFromFzClient
            | ReadyToCheckCollecting
            | CheckReadyToCheckCollectingFozzyOrderInFozzyCollectingService,
    }


    public class CreateOrderAndCollectRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CreateOrderAndCollectUseCases UseCases { get; set; } = CreateOrderAndCollectUseCases.CreateOrderCollectWithReplacements;

        public string Owner
        {
            get => CreateBasketInfo.LoyalityInfo.Owner;
            set => CreateBasketInfo.LoyalityInfo.Owner = value;   
        }

        public string PickerInn { get; set; } = "2921408973";

        public DateTime PickupDate { get; set; } = DateTime.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderPaymentType PaymentType { get; set; } = OrderPaymentType.Online;

        public DeliveryType DeliveryType { get; set; } = DeliveryType.DeliveryHome;

        public BasketType BasketType { get; set; } = BasketType.ClickAndCollect;

        public DeliveryAddressType AddressType { get; set; } = DeliveryAddressType.House;

        public Filials Filial { get; set; } = Filials.Filial_51;

        public Merchant Merchant { get; set; } = Merchant.Fozzy;

        public RequestOrigin Origin { get; set; } = RequestOrigin.Site;

        public Business Business { get; set; } = Business.Fozzy;

        public ItemsCombinations ItemsInCase { get; set; } = ItemsCombinations.OneItem;

        public CreateBasketInfo CreateBasketInfo { get; set; } = new CreateBasketInfo();

        public CollectingEmulationType CollectingType { get; set; } = CollectingEmulationType.CollectAll;

        public CollectingEmulationType CollectingWithReplacementType { get; set; } = CollectingEmulationType.CollectPartialAll;

        public ReplacementsEmulationType ReplacementsType { get; set; } = ReplacementsEmulationType.HasReplacements;
        
        public AcceptReplacementsEmulationType AcceptReplacementsType { get; set; } = AcceptReplacementsEmulationType.SetFizedForAll;
    }
}
