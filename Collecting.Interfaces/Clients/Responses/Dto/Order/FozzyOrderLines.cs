using Newtonsoft.Json;
using System.Globalization;
using System.Xml.Serialization;
using Utils;

namespace Collecting.Interfaces.Clients.Responses
{
    [XmlType("FzShopOrderLines")]
    public class FozzyOrderLines
    {
        public string CustomParams { get; set; }

        public string IsActivityEnable { get; set; }

        public string containerBarcode { get; set; }

        public string dateModified { get; set; }

        public int freezeStatus { get; set; }

        public string globalUserId { get; set; }

        [XmlElement("lagerId")]
        public int lagerId { get; set; }
        public string lagerName { get; set; }
        public string lagerUnit { get; set; }
        
        public int orderId { get; set; }
        
        public string orderQuantity { get; set; }

        public string pickerQuantity { get; set; }

        public string priceOut { get; set; }

        public string replacementLagers { get; set; }
        
        public int rowNum { get; set; }


        [JsonIgnore]
        [XmlIgnore]
        protected FozzyCustomParams Params
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
                
        private FozzyCustomParams _params;

        public FozzyOrderLines()
        {

        }
        public FozzyOrderLines(Guid basketId, Guid id, bool? isWeighted)
        {
            SetBasketAndLineIdAndWeight(basketId, id, isWeighted);
        }

        public void SetLineId(Guid lineId)
        {
            var parameters = Params;
            parameters.lineId = lineId;
            SetCustomParams(parameters);
        }

        public void SetBasketAndLineIdAndWeight(Guid basketId, Guid lineId, bool? isWeighted)
        {
            var parameters = GetCustomParams();
            parameters.lineId = lineId;
            parameters.basketGuid = basketId;
            parameters.isWeighted = isWeighted;
            SetCustomParams(parameters);
        }

        public Guid? GetLineId()
        {
            return GetCustomParams().lineId;
        }

        public Guid? GetBasketId()
        {
            return GetCustomParams().basketGuid;
        }

        public bool? GetIsWeighted()
        {
            return GetCustomParams().isWeighted;
        }

        public void SetBasketId(Guid value)
        {
            var parameters = GetCustomParams();
            parameters.basketGuid = value;
            SetCustomParams(parameters);
        }

        public void SetIsWeghted(bool? isWeighted)
        {
            var parameters = GetCustomParams();
            parameters.isWeighted = isWeighted;
            SetCustomParams(parameters);
        }

        protected virtual void SetCustomParams(FozzyCustomParams customParams)
        {
            CustomParams = customParams.JsonSerialize();
            _params = customParams;
        }
        protected virtual FozzyCustomParams GetCustomParams()
        {
            if (!string.IsNullOrEmpty(CustomParams))
            {
                FozzyCustomParams customParams;
                try
                {
                    customParams = CustomParams.JsonDeserialize<FozzyCustomParams>();
                }
                catch(Exception ex)
                {
                    return new FozzyCustomParams() { error = ex.GetBaseException().Message };
                }

                return customParams == null ? new FozzyCustomParams() { } : customParams;
                
            }
            return new FozzyCustomParams() { };
        }

        public decimal GetQuantity()
        {
            return string.IsNullOrEmpty(orderQuantity) ? 0m : ToDecimal(orderQuantity);
        }

        private static decimal ToDecimal(string input)
        {
            return decimal.Parse(input.Replace(',', '.'), _formater);
        }

        public decimal? GetPickerQuantity()
        {
            return string.IsNullOrEmpty(pickerQuantity) ? new decimal?() : ToDecimal(pickerQuantity);
        }

        public bool PickerQuantityHasValue()
        {
            return !string.IsNullOrEmpty(pickerQuantity);
        }

        public decimal GetPickerQuantityZero()
        {
            return string.IsNullOrEmpty(pickerQuantity) ? 0m : ToDecimal(pickerQuantity);
        }


        private readonly static IFormatProvider _formater = new CultureInfo("en");
    }
}
