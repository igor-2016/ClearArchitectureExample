namespace DomainServices.Interfaces
{
    // TODO move to Application Interfaces!!
    public interface IEntityMappingService
    {
        int? GetDeliveryTypeFromEcomToFozzy(int ecomDeliveryTypeId);
        int? GetPaymentTypeFromEcomToFozzy(int ecomPaymentTypeId);

        int? GetDeliveryTypeFromFozzyToEcom(int fozzyDeliveryTypeId);
        int? GetPaymentTypeFromFozzyToEcom(int fozzyPaymentTypeId);
        
    }
}
