using Newtonsoft.Json;
using System.Globalization;
using System.Xml.Serialization;
using Utils;

namespace Entities.Models.Collecting
{
    [XmlType("FzShopOrderLines")]
    public class FzShopOrderLines
    {
        [XmlElement(IsNullable = true)]
        public string? CustomParams { get; set; }

        [XmlElement(IsNullable = true)]
        public string? IsActivityEnable { get; set; }

        [XmlElement(IsNullable = true)]
        public string? containerBarcode { get; set; }

        public string dateModified { get; set; }

        public int freezeStatus { get; set; }

        [XmlElement("globalUserId", IsNullable = true)]
        public string? globalUserId { get; set; }

        [XmlElement("lagerId")]
        public int lagerId { get; set; }

        [XmlElement("lagerName", IsNullable = true)]
        public string? lagerName { get; set; }

        [XmlElement("lagerUnit", IsNullable = true)]
        public string? lagerUnit { get; set; }
        
        public int orderId { get; set; }
        
        public string orderQuantity { get; set; }

        [XmlElement("pickerQuantity", IsNullable = true)]
        public string? pickerQuantity { get; set; }

        [XmlElement(IsNullable = true)]
        public string? priceOut { get; set; }

        [XmlElement(IsNullable = true)]
        public string? replacementLagers { get; set; }
        
        public int rowNum { get; set; }


        [JsonIgnore]
        [XmlIgnore]
        protected CustomParams Params
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
                
        private CustomParams _params;

        public FzShopOrderLines()
        {

        }
        public FzShopOrderLines(Guid basketId, Guid id, bool? isWeighted)
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

        protected virtual void SetCustomParams(CustomParams customParams)
        {
            CustomParams = customParams.JsonSerialize();
            _params = customParams;
        }
        protected virtual CustomParams GetCustomParams()
        {
            if (!string.IsNullOrEmpty(CustomParams))
            {
                CustomParams customParams;
                try
                {
                    customParams = CustomParams.JsonDeserialize<CustomParams>();
                }
                catch(Exception ex)
                {
                    return new CustomParams() { error = ex.GetBaseException().Message };
                }

                return customParams == null ? new CustomParams() { } : customParams;
                
            }
            return new CustomParams() { };
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
