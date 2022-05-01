using ECom.Types.Delivery;
using ECom.Types.DTO;
using ECom.Types.Orders;
using Expansion.Interfaces.Dto;
using Expansion.Interfaces.Dto.Requests;
using Expansion.Interfaces.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace HealthCheck.Controllers.Order.Requests
{
    public class CreateTestOrderAndCollectRequest : IExamplesProvider<CreateTestOrderAndCollectRequest>
    {
        public CreateOrderAndCollectUseCases UseCases { get; set; } = CreateOrderAndCollectUseCases.CreateOrderCollectWithReplacements;

        public string Owner
        {
            get => CreateBasketInfo.LoyalityInfo.Owner;
            set => CreateBasketInfo.LoyalityInfo.Owner = value;
        }

        public string PickerInn { get; set; } = "2921408973";

        public DateTime PickupDate { get; set; } = DateTime.Now;

        public OrderPaymentType PaymentType { get; set; } = OrderPaymentType.CourierCash;

        public DeliveryType DeliveryType { get; set; } = DeliveryType.DeliveryHome;

        public BasketType BasketType { get; set; } = BasketType.ClickAndCollect;

        public DeliveryAddressType AddressType { get; set; } = DeliveryAddressType.House;

        public Filials Filial { get; set; } = Filials.Filial_51;

        public Merchant Merchant { get; set; } = Merchant.Fozzy;

        public RequestOrigin Origin { get; set; } = RequestOrigin.Site;

        public Business Business { get; set; } = Business.Fozzy;

        public ItemsCombinations ItemsInCase { get; set; } = ItemsCombinations.TwoItems;

        public CreateBasketInfo CreateBasketInfo { get; set; } = new CreateBasketInfo();

        /// <summary>
        /// Тип эмуляции сборки
        /// </summary>
        public CollectingEmulationType CollectingType { get; set; } = CollectingEmulationType.CollectAll;


        /// <summary>
        /// Первая сборка перед заменами
        /// </summary>
        public CollectingEmulationType CollectingWithReplacementType { get; set; } = CollectingEmulationType.CollectPartialAll;

        /// <summary>
        /// Тип эмуляции генерации замен
        /// </summary>
        public ReplacementsEmulationType ReplacementsType { get; set; } = ReplacementsEmulationType.HasReplacements;


        /// <summary>
        /// Тип эмуляции подтверждения замен оператором
        /// </summary>
        public AcceptReplacementsEmulationType AcceptReplacementsType { get; set; } = AcceptReplacementsEmulationType.SetFizedForAll;

        public CreateTestOrderAndCollectRequest GetExamples()
        {
            return new CreateTestOrderAndCollectRequest();
        }
    }
}
