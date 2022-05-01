using Newtonsoft.Json;

namespace Catalog.Interfaces.Dto
{
    public class CatalogGoodsData
    {
        /*
        //
        // Summary:
        //     ШК
        [JsonProperty(PropertyName = "barcode")]
        public string Barcode;

        //
        // Summary:
        //     Ид бренда
        [JsonProperty(PropertyName = "brandid")]
        public int BrandId;

        //
        // Summary:
        //     Ид бренда
        [JsonProperty(PropertyName = "classid")]
        public int ClassId;

        //
        // Summary:
        //     Значение для счетчика ВР
        [JsonProperty(PropertyName = "counter")]
        public decimal Counter;

        //
        // Summary:
        //     Дробность количества
        [JsonProperty(PropertyName = "div")]
        public decimal Div;

        //
        // Summary:
        //     Дополнительный налог (алкоголь, табак)
        [JsonProperty(PropertyName = "extrataxgroup")]
        public int ExtraTaxGroup;
        */
        //
        // Summary:
        //     Ид артикула
        [JsonProperty(PropertyName = "lagerid")]
        public int LagerId;

        /*
        //
        // Summary:
        //     Ид маркетинговой программы
        [JsonProperty(PropertyName = "marketprogramid")]
        public int MarketProgramId;

        //
        // Summary:
        //     Минимальное кол-во для продажи
        [JsonProperty(PropertyName = "minkolvo")]
        public decimal MinKolvo;

        //
        // Summary:
        //     Группа налогообложения
        [JsonProperty(PropertyName = "ndsgroup")]
        public byte NdsGroup;

        //
        // Summary:
        //     Битовая маска параметров
        [JsonProperty(PropertyName = "params")]
        public int Params;

        //
        // Summary:
        //     Битовая маска параметров. Часть 2
        [JsonProperty(PropertyName = "params2")]
        public int Params2;

        //
        // Summary:
        //     Цена. Розница
        [JsonProperty(PropertyName = "price")]
        public decimal Price;

        //
        // Summary:
        //     Розничный индикатив
        [JsonProperty(PropertyName = "pricemin")]
        public decimal PriceMin;

        //
        // Summary:
        //     Оптовая/мелкооптовая цена
        [JsonProperty(PropertyName = "pricediscount")]
        public decimal PriceOpt;

        //
        // Summary:
        //     Оптовый индикатив
        [JsonProperty(PropertyName = "priceminopt")]
        public decimal PriceMinOpt;

        //
        // Summary:
        //     Сорт
        [JsonProperty(PropertyName = "quality")]
        public int Quality;

        //
        // Summary:
        //     Значение расфасовки
        [JsonProperty(PropertyName = "rasf")]
        public decimal Rasf;

        //
        // Summary:
        //     Ид расфасовки
        [JsonProperty(PropertyName = "rasfid")]
        public int RasfId;

        //
        // Summary:
        //     Кол-во для мелкоопта
        [JsonProperty(PropertyName = "wholesalequantity")]
        public decimal WholesaleQuantity;

        //
        // Summary:
        //     Кол-во для добавления в чек
        [JsonProperty(PropertyName = "count")]
        public decimal Quantity;

        //
        // Summary:
        //     Краткое наименование
        [JsonProperty(PropertyName = "shortName")]
        public string ShortName;

        //
        // Summary:
        //     Краткое наименование
        [JsonProperty(PropertyName = "name")]
        public string Name;

        //
        // Summary:
        //     Единица измерения
        [JsonProperty(PropertyName = "unit")]
        public string Unit;

        //
        // Summary:
        //     Фото товара по разрешению/бизнесу
        [JsonProperty(PropertyName = "imageUrls")]
        public Dictionary<string, string> ImageUrls;

        //
        // Summary:
        //     Данные по просканированному ШК со скидкой
        [JsonProperty(PropertyName = "discountMinusPercent")]
        public DiscountMinusPercent DiscountMinusPercent;

        //
        // Summary:
        //     Цена с учетом скидки
        [JsonProperty(PropertyName = "priceOut")]
        public decimal PriceOut;

        //
        // Summary:
        //     Скидка на позицию, %
        [JsonProperty(PropertyName = "discountPercent")]
        public decimal DiscountPercent;

        //
        // Summary:
        //     Остаток на основном складе
        [JsonProperty(PropertyName = "storeKolvo")]
        public decimal StoreKolvo;

        //
        // Summary:
        //     Нетто
        [JsonProperty(PropertyName = "netto")]
        public decimal Netto;

        //
        // Summary:
        //     Брутто
        [JsonProperty(PropertyName = "brutto")]
        public decimal Brutto;

        //
        // Summary:
        //     Признак наличия товара в каталоге сайта
        [JsonProperty(PropertyName = "existsInCatalog")]
        public bool ExistsInCatalog;

        //
        // Summary:
        //     Рсчетный остаток
        [JsonProperty(PropertyName = "calcStoreKolvo")]
        public decimal CalcStoreKolvo;

        //
        // Summary:
        //     Признак еко-упаковки
        [JsonProperty(PropertyName = "ecoPacking")]
        public bool EcoPacking;

        //
        // Summary:
        //     Параметры всякие
        [JsonProperty(PropertyName = "parameters")]
        public IEnumerable<string> Parameters;
        
        //
        // Summary:
        //     Вложенные товары
        [JsonProperty(PropertyName = "items")]
        public IEnumerable<GoodsData> Items;

        
        //
        // Summary:
        //     Тип товара
        [JsonProperty(PropertyName = "typeId")]
        public int TypeId
        {
            get;
            set;
        }

        //
        // Summary:
        //     Дата последнего прихода
        [JsonProperty(PropertyName = "lastIncomeDate")]
        public string LastIncomeDate
        {
            get;
            set;
        }

        //
        // Summary:
        //     Описание
        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "prices")]
        public IEnumerable<ItemPrice> Prices
        {
            get;
            set;
        }

        //
        // Summary:
        //     Признак акциза для скана доп-шк
        [JsonProperty(PropertyName = "isExcise")]
        public bool IsExcise
        {
            get;
            set;
        }

        //
        // Summary:
        //     Slug
        [JsonProperty(PropertyName = "slug")]
        public string Slug
        {
            get;
            set;
        }
        */
    }
}
