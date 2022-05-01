using Collecting.Interfaces.Enums;
using DomainServices.Interfaces;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using Entities.Models;

namespace ApplicationServices.Implementation
{
    public class EntityMapper : IEntityMappingService
    {
        private readonly EntityMap<Guid, int, int> _deliveryTypes;
        private readonly EntityMap<Guid, int, int> _paymentTypes;
        public EntityMapper()
        {
            // use db!!!!!
            _deliveryTypes = new EntityMap<Guid, int, int>();
            LoadDeliveryTypeMap(_deliveryTypes);

            _paymentTypes = new EntityMap<Guid, int, int>();
            LoadPaymentTypeMap(_paymentTypes);
        }
        private void LoadPaymentTypeMap(EntityMap<Guid, int, int> paymentTypes)
        {
            // Ecom -> Fozzy

            paymentTypes.LoadFromTo((int)OrderPaymentType.CourierCash, (int)FozzyPaymentType.Payment89);
            paymentTypes.LoadFromTo((int)OrderPaymentType.Online, (int)FozzyPaymentType.Payment161); // проверить?

            // Fozzy -> Ecom
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment89, (int)OrderPaymentType.CourierCash);

            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment29, (int)OrderPaymentType.Unknown); // безнал
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment147, (int)OrderPaymentType.CourierCash); // на Новой Почте
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment161, (int)OrderPaymentType.Online); // надо добавить Liqpay
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment213, (int)OrderPaymentType.Online); // надо добавить Liqpay
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment239, (int)OrderPaymentType.CourierCash);
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment246, (int)OrderPaymentType.Unknown); // безнал
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment273, (int)OrderPaymentType.Unknown); // надо добавить Prests Shop
            paymentTypes.LoadToFrom((int)FozzyPaymentType.Payment336, (int)OrderPaymentType.CourierCash); // на Новой Почте

            /*
             paymentId         paymentName
             29           Оплата по безналичному расчету
             89           Оплата наличными при получении
             147         Оплата наличными при получении в отделении НП
             161         Liqpay
             213         Liqpay
             239         Оплата наличными при получении
             246         Оплата по безналичному расчету
             273         Y.CMS Prestashop
             336         Оплата в отделении Новой Почты

            if (!orderPaymentType.HasValue)
                throw new CollectException(CollectErrors.InvalidFozzyShopEnumConvertion, $"OrderPaymentType 0 -> FozztShopPaymentType");

            switch (orderPaymentType)
            {
                case OrderPaymentType.Unknown:
                    break;
                case OrderPaymentType.Cachdesk:
                    break;
                case OrderPaymentType.Online:
                    break;

                case OrderPaymentType.CourierCash:
                    return FozztShopPaymentType.Payment89;

                case OrderPaymentType.CourierCard:
                    break;
                case OrderPaymentType.MasterPass:
                    break;
                case OrderPaymentType.CreditCashless:
                    break;
                default:
                    break;
            
            */
        }
        private void LoadDeliveryTypeMap(EntityMap<Guid, int, int> deliveryTypes)
        {
            // Ecom -> Fozzy
            deliveryTypes.LoadFromTo((int)DeliveryType.DeliveryHome, (int)FozzyDeliveryType.Delivery27);
            deliveryTypes.LoadFromTo((int)DeliveryType.DeliveryOffice, (int)FozzyDeliveryType.Delivery27);
            //..

            // Fozzy -> Ecom
            deliveryTypes.LoadToFrom((int)FozzyDeliveryType.Delivery27, (int)DeliveryType.DeliveryHome); 
            deliveryTypes.LoadToFrom((int)FozzyDeliveryType.Delivery30, (int)DeliveryType.DeliveryHome); // город
            deliveryTypes.LoadToFrom((int)FozzyDeliveryType.Delivery33, (int)DeliveryType.DeliveryHome); // город
            deliveryTypes.LoadToFrom((int)FozzyDeliveryType.Delivery35, (int)DeliveryType.DeliveryHome);

            deliveryTypes.LoadToFrom((int)FozzyDeliveryType.Delivery22, (int)DeliveryType.SelfPickup); //филиал
            deliveryTypes.LoadToFrom((int)FozzyDeliveryType.Delivery24, (int)DeliveryType.SelfPickup); //филиал
            deliveryTypes.LoadToFrom((int)FozzyDeliveryType.Delivery32, (int)DeliveryType.SelfPickup); //филиал

            /*
            deliveryId           deliveryName
                22           Fozzy-Drive (Самовывоз - ул. Заболотного 37)
                24           Fozzy-Drive (Самовывоз - ул. Балковская, 88)
                27           Доставка курьером
                30           Доставка курьером по г. Одесса
                32           Fozzy-Drive (Самовывоз - ул Маршала Малиновского 2)
                33           Доставка курьером по г. Днепр
                35           Доставка курьером
          
            switch (deliveryType)
            {
                case DeliveryType.Unknown:
                    break;
                case DeliveryType.SelfPickup:
                    break;

                case DeliveryType.DeliveryHome:
                    return FozzyShopDeliveryType.Delivery27;

                case DeliveryType.DeliveryFlat:
                    break;
                case DeliveryType.DeliveryOffice:
                    break;
                case DeliveryType.DeliveryGlovo:
                    break;
                case DeliveryType.DeliveryExpress:
                    break;
                case DeliveryType.DeliveryExpressFood:
                    break;
                case DeliveryType.JustIn:
                    break;
                case DeliveryType.LongDelivery:
                    break;
                default:
                    break;
            }
            */
        }

        public int? GetDeliveryTypeFromEcomToFozzy(int ecomDeliveryTypeId)
        {
            var nullValue = -1;
            var fozzyDeliveryTypeId = _deliveryTypes.GetFrom(ecomDeliveryTypeId, nullValue);
            return fozzyDeliveryTypeId == nullValue ? new int?() : fozzyDeliveryTypeId;
        }

        public int? GetDeliveryTypeFromFozzyToEcom(int fozzyDeliveryTypeId)
        {
            var nullValue = -1;
            var ecomDeliveryTypeId = _deliveryTypes.GetTo(fozzyDeliveryTypeId, nullValue);
            return ecomDeliveryTypeId == nullValue ? new int?() : ecomDeliveryTypeId;
        }

        public int? GetPaymentTypeFromEcomToFozzy(int ecomPaymentTypeId)
        {
            var nullValue = -1;
            var fozzyPaymentTypeId = _paymentTypes.GetFrom(ecomPaymentTypeId, nullValue);
            return fozzyPaymentTypeId == nullValue ? new int?() : fozzyPaymentTypeId;
        }

        public int? GetPaymentTypeFromFozzyToEcom(int fozzyPaymentTypeId)
        {
            var nullValue = -1;
            var  ecomPaymentTypeId = _paymentTypes.GetTo(fozzyPaymentTypeId, nullValue);
            return ecomPaymentTypeId == nullValue ? new int?() : ecomPaymentTypeId;
        }
    }
}
