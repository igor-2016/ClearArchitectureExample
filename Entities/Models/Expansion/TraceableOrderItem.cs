using ECom.Entities.Models;
using Entities.Models.Collecting;
using Newtonsoft.Json;
using System.Xml.Serialization;
using Utils;

namespace Entities.Models.Expansion
{
    public class TraceableOrderItem : Entity<Guid>, IHasBasket
    {

        public TraceableOrderItem()
        {

        }

        public TraceableOrderItem(Guid orderId)
        {
            OrderId = orderId;
        }

        public TraceableOrderItem(Guid id, Guid orderId, Guid basketId, bool? isWeighted) : this(orderId)
        {
            //CustomParams
            SetBasketAndLineIdAndWeight(basketId, id, isWeighted);
            Id = id;
            BasketId = basketId;
            IsWeighted = isWeighted;
            OrderId = orderId;
        }



        public Guid OrderId { get; set; } // FK

        //public Guid? EcomItemId { get; set; } // from collectable info from ecom core

        public Guid BasketId { get; set; }

        public int LagerId { get; set; }


        public string OrderNumber { get; set; }


        public int? ExternalOrderId { get; set; }


        public string CustomParams { get; set; }


        public bool? IsActivityEnable { get; set; }


        public string ContainerBarcode { get; set; }


        public DateTime DateModified { get; set; }


        public int? FreezeStatus { get; set; }


        public int? GlobalUserId { get; set; }

        public string PickerName { get; set; }

        public string UserInn { get; set; }  // added


        public string LagerName { get; set; }


        public string LagerUnit { get; set; }


        public decimal OrderQuantity { get; set; }


        public decimal? PickerQuantity { get; set; }


        public decimal PriceOut { get; set; }


        public int? ReplacementOnLagerId { get; set; }

        public string ReplacementLagers { get; set; }

        public bool? IsWeighted { get; set; }

        public bool? IsFilled { get; set; }

        public bool Collectable { get; set; } = true;

        public int RowNum { get; set; }

        public string SortingCategory { get; set; }


        [JsonIgnore]
        [XmlIgnore]
        //[AutoMapper.AutoMap.Ignore]
        protected ItemParams Params
        {
            get
            {
                if (_params == null)
                {
                    _params = GetCustomParams();
                }
                return _params;
            }
        }

        private ItemParams _params;


        public CatalogItem GetCatalogItem()
        {
            return new CatalogItem()
            {
                FreezeStatusId = FreezeStatus,
                FreezeStatus = FreezeStatus?.ToString() ?? "0",
                Id = LagerId,
                IsActivityEnable = IsActivityEnable,
                IsCollectable = Collectable,
                IsWeighted = IsWeighted ?? true,
                Name = LagerName,
                NameForSite = LagerName,
                Price = PriceOut,
                PriceOld = null,
                PriceOpt = null,
                SortingCategory = SortingCategory,
                Unit = LagerUnit,
            };
        }

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

        public int[] GetReplacementLagers()
        {
            return ReplacementLagers.ToArrayOfInts();
        }

        public void SetLineId(Guid lineId)
        {
            var parameters = Params;
            parameters.LineId = lineId;
            SetCustomParams(parameters);
        }

        public void SetBasketAndLineIdAndWeight(Guid basketId, Guid lineId, bool? isWeighted)
        {
            var parameters = GetCustomParams();
            parameters.LineId = lineId;
            parameters.BasketGuid = basketId;
            parameters.IsWeighted = isWeighted;
            SetCustomParams(parameters);
        }

        public Guid? GetLineId()
        {
            return GetCustomParams().LineId;
        }

        public Guid? GetBasketId()
        {
            return GetCustomParams().BasketGuid;
        }

        public bool? GetIsWeighted()
        {
            return GetCustomParams().IsWeighted;
        }

        public void SetBasketId(Guid value)
        {
            var parameters = GetCustomParams();
            parameters.BasketGuid = value;
            SetCustomParams(parameters);
        }

        public void SetIsWeghted(bool? isWeighted)
        {
            var parameters = GetCustomParams();
            parameters.IsWeighted = isWeighted;
            SetCustomParams(parameters);
        }

        protected virtual void SetCustomParams(ItemParams customParams)
        {
            CustomParams = customParams.JsonSerialize();
            _params = customParams;
        }
        public virtual ItemParams GetCustomParams()
        {
            if (!string.IsNullOrEmpty(CustomParams))
            {
                ItemParams customParams;
                try
                {
                    customParams = CustomParams.JsonDeserialize<ItemParams>();
                }
                catch (Exception ex)
                {
                    return new ItemParams() { Error = ex.GetBaseException().Message };
                }

                return customParams == null ? new ItemParams() { } : customParams;

            }
            return new ItemParams() { };
        }
    }
}
