using Collecting.Interfaces.Enums;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Types.Delivery;
using ECom.Types.Orders;
using Entities.Models.Collecting;
using Entities.Models.Expansion;
using Expansion.Interfaces.Enums;
using Utils;

namespace ECom.Expansion.TestHelpers
{


    public class TestDataGenerator
    {

        Dictionary<int, CatalogItem> catalogItems = new Dictionary<int, CatalogItem>();



        public TestDataGenerator()
        {
            catalogItems.Add((int)Items.CocaCola, Get_Catalog_CocaCola_117());
            catalogItems.Add((int)Items.Banan, Get_Catalog_Banan_32485());
            catalogItems.Add((int)Items.Farba, Get_Catalog_Farba_366());
            catalogItems.Add((int)Items.ChickenFillet, Get_Catalog_File_461800());
            catalogItems.Add((int)Items.Rise, Get_Catalog_Rise_136823());
            catalogItems.Add((int)Items.Cigarettes_255349, Get_Catalog_Siga_255349());
            catalogItems.Add((int)Items.Vine, Get_Catalog_Vine_1285());
            catalogItems.Add((int)Items.Viski_2712, Get_Catalog_Viski_2712());
            catalogItems.Add((int)Items.Viski_37400, Get_Catalog_Viski_37400());
            catalogItems.Add((int)Items.Water_440815, Get_Catalog_Water_440815());
            catalogItems.Add((int)Items.Water_599434, Get_Catalog_Water_599434());
            //catalogItems.Add(, Get_Catalog_());

        }


        public FozzyCollectableOrderInfo AddItemToEComBasket(FozzyCollectableOrderInfo basket,
            FozzyCollectableItemInfo item)
        {
            basket.CollectableItems.Add(item);
            return basket;
        }

        //public ICollectableOrderInfo SetPickedQty(ICollectableOrderInfo basket,
        //    int lagerId, decimal pickedQty)
        //{
        //    basket.CollectableItems.Single(x => x.LagerId == lagerId).q
        //    return basket;
        //}

        public FozzyCollectableOrderInfo AddItemToEComBasket(FozzyCollectableOrderInfo basket,
            Guid ecomLineId, Items lagerId, decimal qty, decimal sum = 100m, string title = "Товар",
            string replacementLagers = "")
        {
            basket.CollectableItems.Add(
                CreateEComItem(ecomLineId, lagerId, qty, sum, title, replacementLagers));
            return basket;
        }


        public FozzyCollectableItemInfo CreateEComItem(
            Guid ecomLineId, Items lagerId, decimal qty, decimal sum = 100m, string title = "Товар",
            string replacementLagers = "")
        {
            return new FozzyCollectableItemInfo()
            {
                Id = ecomLineId,
                LagerId = (int)lagerId,
                Modified = DateTime.Now,
                Qty = qty,
                //Title = title,
                SumOut = sum,
                //Attributes = new HashSet<BasketItemAttributes>(),
                //GoodsData = new GoodsData(),
                ReplacementLagers = replacementLagers.ToArrayOfInts(),
                
            };
        }

        public FozzyCollectableOrderInfo CreateEmptyEComBasket(
          Guid basketGuid,
          OrderPaymentType orderPaymentType = OrderPaymentType.CourierCash,
          BasketType basketType = BasketType.ClickAndCollect,
          DeliveryType deliveryType = DeliveryType.DeliveryHome,
          string orderId = "12434575",
          int filialId = 51
            )
        {
            return new FozzyCollectableOrderInfo()
            {
                BasketGuid = basketGuid,
                FilialId = filialId,
                //OrderCollectingType = OrderCollectingType.FozzyShop,

                OrderPaymentType = orderPaymentType,
                BasketType = basketType,
                DeliveryType = deliveryType,

                PickupDate = DateTime.Now.AddHours(2),
                TimeSlotFrom = DateTime.Now.AddHours(2),
                TimeSlotTo = DateTime.Now.AddHours(3),

                CustomerPhoneAlt = "+380000000",
                CustomerPhone = "+3801111111",
                CustomerName = "Иванов Иван Иванович",
                Created = DateTime.Now,
                OrderBarCode = "barcode",
                OrderNumber = orderId,

                ClientAddress = "Киев, улица, дом, квартира",
                ContragentFullName = "ContragentFullName",
                ContragentOKPO = "000001",
                CourierName = "Водитель 1",
                MerchantId = (int)Merchant.Fozzy,
                MerchantStateId = null,
                Owner = "0123456789012",
                Notes = "Comments",
                SumOut = 100.3m
               
            };
        }


        public FozzyCollectableOrderInfo CreateEComBasketWithTwoLines(
          Guid basketGuid,
          Guid ecomLine1Id,
          Guid ecomLine2Id,
          Items lager1Id = Items.CocaCola,
          Items lager2Id = Items.Vine,
          OrderPaymentType orderPaymentType = OrderPaymentType.CourierCash,
          BasketType basketType = BasketType.ClickAndCollect,
          DeliveryType deliveryType = DeliveryType.DeliveryHome,
          string orderId = "12434575",
          int filialId = 51
      )
        {
            var basket = CreateEmptyEComBasket(basketGuid, orderPaymentType, basketType, deliveryType, orderId, filialId);
            AddItemToEComBasket(basket, ecomLine1Id, lager1Id, 4, 30.1m, "Товар 1");
            AddItemToEComBasket(basket, ecomLine2Id, lager2Id, 7, 80.15m, "Товар 2");
            return basket;
        }

        public CatalogItem Get_Catalog_CocaCola_117()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = (int)Items.CocaCola,
                IsActivityEnable = true,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Coca-cola",
                NameForSite = "Coca-cola Site",
                Price = 20.1m,
                PriceOld = 17.5m,
                PriceOpt = 15.1m,
                Unit = "2л",

            };
        }

        public CatalogItem Get_Catalog_Vine_1285()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Бьющийся",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 1285,
                IsActivityEnable = true,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Вино Teliany Valley МУКУЗАНІчервоне сух.",
                NameForSite = "Вино Teliany Valley Site",
                Price = 84.1m,
                PriceOld = 80.5m,
                PriceOpt = 81.1m,
                Unit = "1б",
            };
        }

        public CatalogItem Get_Catalog_Farba_366()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 366,
                IsActivityEnable = true,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Крем-фарба Palette №2",
                NameForSite = "Крем-фарба Palette №2 Site",
                Price = 21m,
                PriceOld = 15m,
                PriceOpt = 19m,
                Unit = "шт",
                SortingCategory = "Канцелярские товары"
            };
        }

        
        public CatalogItem Get_Catalog_Viski_37400()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 37400,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Віскі БАЛЛАНТАЙН Finest",
                NameForSite = "Віскі Ballantine's Finest Site",
                Price = 226.02m,
                PriceOld = 0,
                PriceOpt = 226.02m,
                Unit = "1л",
                SortingCategory = "Алкоголь (Виски)"
            };
        }

         
        public CatalogItem Get_Catalog_Viski_2712()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 2712,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Віскі Jameson",
                NameForSite = "Віскі Jameson Site",
                Price = 300m,
                PriceOld = 0,
                PriceOpt = 293.22m,
                Unit = "1л",
                SortingCategory = "Алкоголь (Виски)"
            };
        }

         
        public CatalogItem Get_Catalog_Water_599434()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 599434,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Вода питна «Природне джерело» з артезіанської свердловини",
                NameForSite = "Вода питна «Природне джерело» з артезіанської свердловини Site",
                Price = 18.35m,
                PriceOld = 0,
                PriceOpt = 17.08m,
                Unit = "6л",
                SortingCategory = "Питевые воды"
            };
        }


         
        public CatalogItem Get_Catalog_Water_440815()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 440815,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Вода мінеральна «Карпатська Джерельна» негазована",
                NameForSite = "Вода мінеральна «Карпатська Джерельна» негазована Site",
                Price = 12.63m,
                PriceOld = 0,
                PriceOpt = 11.75m,
                Unit = "6л",
                SortingCategory = "Питевые воды"
            };
        }

     
        public CatalogItem Get_Catalog_Siga_255349()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 255349,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Цигарки Sobranie Blue",
                NameForSite = "Цигарки Sobranie Blue Site",
                Price = 16.78m,
                PriceOld = 0,
                PriceOpt = 15.63m,
                Unit = "1п",
                SortingCategory = "Сигареты"
            };
        }

         
        public CatalogItem Get_Catalog_Rise_136823()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = 136823,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = false,
                Name = "Рис Хуторок пропарений",
                NameForSite = "Рис Хуторок пропарений Site",
                Price = 11m,
                PriceOld = 0,
                PriceOpt = 10.62m,
                Unit = "1кг",
                SortingCategory = "Крупы"
            };
        }

         
        public CatalogItem Get_Catalog_Banan_32485()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Сухой",
                FreezeStatusId = (int)FozzyFreezeStatus.Dry,
                Id = (int)Items.Banan,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = true,
                Name = "Банан",
                NameForSite = "Банан Site",
                Price = 150m,
                PriceOld = 0,
                PriceOpt = 120m,
                Unit = "1кг",
                SortingCategory = "Фрукты на развес"
            };
        }

      
        public CatalogItem Get_Catalog_File_461800()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Охлаждённый",
                FreezeStatusId = (int)FozzyFreezeStatus.Cooled,
                Id = 461800,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = true,
                Name = "Куряче філе",
                NameForSite = "Куряче філе Site",
                Price = 55m,
                PriceOld = 0,
                PriceOpt = 45m,
                Unit = "1кг",
                SortingCategory = "Мясо птицы"
            };
        }

        public CatalogItem Get_Catalog_NotFound()
        {
            return new CatalogItem()
            {
                FreezeStatus = "Нет такого товара",
                FreezeStatusId = (int)FozzyFreezeStatus.Cooled,
                Id = 0,
                IsActivityEnable = false,
                IsCollectable = true,
                IsWeighted = true,
                Name = "Нет такого товара",
                NameForSite = "Нет такого товара Site",
                Price = 55m,
                PriceOld = 0,
                PriceOpt = 45m,
                Unit = "1кг",
                SortingCategory = "Нет такого товара"
            };
        }


        public IEnumerable<CatalogItem> Get_Catalog_All_Items()
        {
            return catalogItems.Values.Cast<CatalogItem>();
        }

        public Picker GetValidPicker()
        {
            return new Picker()
            {
                Id = 86505,
                Inn = "2921408973",
                Name = "Picker Name"
            };
        
        }

        public TraceableOrderItem GetTraceableOrderItemByLagerId(TraceableOrder order, Items lagerId)
        {
            return order.Items.Single(x => x.LagerId == (int)lagerId);
        }

        //public TraceableOrderItem GetTraceableOrderItemByEComItemId(TraceableOrder order, Guid ecomItemId)
        //{
        //    return order.Items.Single(x => x.EcomItemId == ecomItemId);
        //}

        public TraceableOrderItem GetTraceableOrderItemByItemId(TraceableOrder order, Guid itemId)
        {
            return order.Items.Single(x => x.Id == itemId);
        }

        public FzShopOrderLines? GetFozzyOrderItemByLagerId(OrderData order, Items lagerId)
        {
            return order.orderLines.SingleOrDefault(x => x.lagerId == (int)lagerId);
        }

        public Task<CatalogInfo> GetValidCatalogSingleItemInfo(int lagerId, int filialId, Merchant merchant,
            CancellationToken cancellationToken, DeliveryType ecomDeliveryType = DeliveryType.Unknown, Guid? basketGuid = null)
        {
            if(catalogItems.TryGetValue(lagerId, out var catalogItem))
            {
                return Task.FromResult(new CatalogInfo()
                {
                    BasketGuid = basketGuid,
                    DeliveryType = ecomDeliveryType,
                    FilialId = filialId,
                    LagerId = lagerId,
                    Merchant = merchant,
                    Items = new List<CatalogItem>(
                        new CatalogItem[] {
                           catalogItem
                            }
                        ),
                });
            }
           
            throw new NotSupportedException(lagerId.ToString());
        }

        public Picker? Get_Null_PickerInfoByBasketId(Guid basketId)
        {
            return null;
        }

        public Task<Picker> Get_Valid_PickerInfoByInn(string inn, CancellationToken cancellationToken)
        {
            return Task.FromResult(GetValidPicker());
        }

        public Picker? Get_Null_PickerInfoByBasketIdAndLagerId(Guid basketId, int lagerId)
        {
            return null;
        }

        public int? Get_Site_OrderOrigin(int merchantId, int filialId, BasketType basketType)
        {
            return (int)FozzyOrderOrigin.Site;
        }

        public int? Get_Null_OrderOriginByBasketId(Guid basketId)
        {
            return null;
        }

        public void DefaultRowNumCalculator(IEnumerable<TraceableOrderItem> items)
        {
            var rowId = 1;
            foreach (var item in items)
            {
                item.RowNum = rowId;
                rowId++;
            }
        }

        public void NoCheckFilledCalculator(IEnumerable<TraceableOrderItem> items)
        {

        }


        public string GetLogisticType(IEnumerable<CatalogInfo> catalogInfos)
        {
            var set = new HashSet<string>();
            foreach (var catalogInfo in catalogInfos)
            {
                set.Add(catalogInfo?.GetSingleItem()?.FreezeStatus ?? "неизвестно");
            }
            return set.ToVerLineString();

        }
    }
}
