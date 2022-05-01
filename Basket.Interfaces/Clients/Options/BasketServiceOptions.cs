using Utils.Sys.Options;

namespace Basket.Interfaces.Clients.Options
{
    /*
      "GetBasketMethodFormat": "/ecom/{0}",
    "CreateBasketMethod": "/ecom/create",
    "ChangeOwnerMethodFormat": "/ecom/{0}/changeOwner",
    "ChangeFilialMethodFormat": "/ecom/{0}/changeFilialDelivery",
    "AddItemMethodFormat": "/ecom/{0}/addItem",
    "CloseBasketMethodFormat": "/ecom/{0}/propsClose"
    "GetCollactableOrderMethodFormat": "/ecom/workflow/order/{0}/getCollactableOrderInfo"
     */
    public class BasketServiceOptions : HttpClientOptions
    {
        public string GetBasketMethodFormat { get; set; }

        public string CreateBasketMethod { get; set; }

        public string ChangeOwnerMethodFormat { get; set; }

        public string ChangeFilialMethodFormat { get; set; }

        public string AddItemMethodFormat { get; set; }

        public string CloseBasketMethodFormat { get; set; }

        public string GetTimeSlotMethodFormat { get; set; }

        public string GetCollactableOrderMethodFormat { get; set; }

        public string UpdateCollectingMethodFormat { get; set; }    
    }
}
