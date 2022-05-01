namespace Expansion.Interfaces.Dto.Requests
{

    public class CreateBasketInfo
    {
        public ContactInfo ContactInfo { get; set; } = new ContactInfo();

        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();

        public CompanyInfo CompanyInfo { get; set; } = new CompanyInfo();


        public LoyalityInfo LoyalityInfo { get; set; } = new LoyalityInfo();

        public LocationInfo LocationInfo { get; set; } = new LocationInfo();

        public OtherInfo OtherInfo { get; set; } = new OtherInfo();

    }


    public class ContactInfo
    {
        public string ContactName { get; set; } = "Пётр";
        public string ContactPhone { get; set; } = "+380501141212";

        public string PhoneNumber { get; set; } = "+380501111111";
    }

    public class CustomerInfo
    {
        public string CustomerEmail { get; set; } = "pp.petrov@temabit.com";
        public string CustomerName { get; set; } = "Пётр Петрович Петров";
        public string CustomerPhone { get; set; } = "+380971111111";

        public string FeedbackPhone { get; set; } = "+380972222222";

    }

    public class CompanyInfo
    {
        public string ContragentFullName { get; set; } = "ЗАО Рога и Копыта";
        public string ContragentOKPO { get; set; } = "123456789";
    }

    public class LoyalityInfo
    {

        public string Owner { get; set; } = "0296018319764";
        public string UserId { get; set; } = "";
        public string AccessToken { get; set; } = "MjMxNDQ5ZDc0NTlkYjljOTY5NTE5YjBlOGYyMGU2MzE";
        public string Url { get; set; } = "https://my.silpo.iir.fozzy.lan/account/purchases/my-orders";
    }

    public class LocationInfo
    {
        public string Latitude { get; set; } = "50.2";
        public string Longitude { get; set; } = "60.4";
        public bool Accurate { get; set; } = false;

        public string Comments { get; set; } = "Координати потребують уточнення. ";
    }

    public class OtherInfo
    {
        public string Notes { get; set; } = "постучать в дверь три раза";
        public string? ExternalOrderId { get; set; } = Guid.NewGuid().ToString();
    }
}
