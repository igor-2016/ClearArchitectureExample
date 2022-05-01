using Entities.Models.Collecting;

namespace Entities.Models.Expansion
{
    public class TraceableOrder : VersionedEntity<Guid>, IHasBasket
    {
        public TraceableOrder()
        {
            Items = new List<TraceableOrderItem>();
        }

        public Guid BasketId { get; set; }

        public string OrderNumber { get; set; } 

        public int? ExternalOrderId { get; set; } // in fz db int !!! у Чернова


        public DateTime? ChequePrintDateTime { get; set; }


        public string ClientFullName { get; set; }


        public string ClientMobilePhone { get; set; }


        public string ClientMobilePhoneAlt1 { get; set; }

        // max
        public string ContainerBarcodes { get; set; }


        public string ContragentFullName { get; set; }


        public string ContragentOKPO { get; set; }

        
        public DateTime DateModified { get; set; }


        public string DeliveryAddress { get; set; }


        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Fozzy DeliveryType
        /// </summary>
        public int DeliveryId { get; set; } 


        public TimeSpan DeliveryTimeFrom { get; set; }


        public TimeSpan DeliveryTimeTo { get; set; }


        public int? DriverId { get; set; } // not in db


        public string DriverName { get; set; }


        public int FilialId { get; set; }


        public int? GlobalUserId { get; set; }

        public string PickerName { get; set; }


        public string UserInn { get; set; } // added not in fz db


        public string LastContainerBarcode { get; set; }

        
        public string LogisticsType { get; set; }


        public string MegaContainerBarcodes { get; set; } // not in fz db


        public string OrderBarcode { get; set; }


        public DateTime OrderCreated { get; set; }

        
        public int OrderFrom { get; set; } 


       


        public int OrderStatus { get; set; }


        public int PaymentId { get; set; }


        public int? PlacesCount { get; set; }


        public int Priority { get; set; }


        public string Remark { get; set; }


        public string RroNumber { get; set; }


        public decimal? SumPaymentFromInternet { get; set; }


        public decimal? SumPaymentFromKassa { get; set; }

       
        public int CollectingState { get; set; }
        
        public DateTime? CollectStartTime { get; set; }

        public DateTime? CollectEndTime { get; set; }


        public int MerchantId { get; set; }

        public IList<TraceableOrderItem> Items { get; set; }


        /// <summary>
        /// Сформировать сущность Сборщика
        /// </summary>
        /// <returns></returns>
        public Picker GetPicker()
        {
            return new Picker()
            {
                Id = GlobalUserId ?? 0,
                Inn = UserInn,
                Name = PickerName
            };
        }

        public EComPicker GetEComPicker()
        {
            return new EComPicker()
            {
                Id = GlobalUserId ?? 0,
                Name = PickerName
            };
        }

        /// <summary>
        /// Сформировать сущность Водителя
        /// </summary>
        /// <returns></returns>
        public Driver GetDriver()
        {
            return new Driver()
            {
                Id = DriverId ?? 0,
                //Inn = ,
                Name = DriverName
            };
        }
    }
}
