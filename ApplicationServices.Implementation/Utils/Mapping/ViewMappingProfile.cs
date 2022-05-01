using ApplicationServices.Interfaces.Enums;
using ApplicationServices.Interfaces.Models;
using AutoMapper;
using Collecting.Interfaces.Clients.Responses;
using Collecting.Interfaces.Enums;
using ECom.Entities.Models;
using ECom.Types.Orders;
using Entities.Models.Collecting;
using Entities.Models.Expansion;

namespace ApplicationServices.Implementation.Utils
{
    public class ViewMappingProfile : Profile
    {
        public ViewMappingProfile()
        {
            CreateMap<Entity<Guid>, EntityView<Guid>>();
            CreateMap<Entity<int>, EntityView<int>>();
            CreateMap<VersionedEntity<Guid>, VersionedEntityView<Guid>>();

         
            CreateMap<CatalogItemPrices, CatalogItemPricesView>()
              ;

            CreateMap<CatalogItem, CatalogItemView>()
                .ForMember(v => v.Collectable, opt => opt.MapFrom(e => e.IsCollectable))
                 .ForMember(v => v.FreezeStatus, opt => opt.MapFrom(e => (FozzyFreezeStatus)(e.FreezeStatusId ?? 0)))
                 .ForMember(v => v.LagerName, opt => opt.MapFrom(e => e.Name))
                 .ForMember(v => v.Id, opt => opt.MapFrom(e => e.Id))
                 .ForMember(v => v.LagerUnit, opt => opt.MapFrom(e => e.Unit))
                 .ForMember(v => v.Prices, opt => opt.MapFrom(e => e.GetPrices()))
                ;

            CreateMap<Driver, DriverView>()
            .IncludeBase<Entity<int>, EntityView<int>>()
            ;


            CreateMap<CustomParams, FozzyCustomParams>()
                .ReverseMap()
                ;

            CreateMap<OrderData, FozzyOrderData>()
                .ReverseMap()
                ;

            CreateMap<FzShopOrder, FozzyOrder>()
                .ReverseMap()
                ;

            CreateMap<FzShopOrderLines, FozzyOrderLines>()
                .ReverseMap()
                ;


            //CreateMap<EComPicker, PickerView>()
            //    .IncludeBase<Entity<int>, EntityView<int>>()
            //    

            CreateMap<Picker, PickerView>()
                .IncludeBase<Entity<int>, EntityView<int>>()
                ;
            CreateMap<ItemParams, ItemParamsView>()

                ;

            CreateMap<TraceableOrderItem, TraceableOrderItemView>()
                .ForMember(view => view.CatalogItem, opt => opt.MapFrom(entity => entity.GetCatalogItem()))
                .ForMember(view => view.Picker, opt => opt.MapFrom(entity => entity.GetPicker())) 
                .ForMember(view => view.ReplacementLagers, opt => opt.MapFrom(entity => entity.GetReplacementLagers()))
                .ForMember(view => view.Params, opt => opt.MapFrom(entity => entity.GetCustomParams()))
                .IncludeBase<Entity<Guid>, EntityView<Guid>>()
             ;

           

            CreateMap<TraceableOrder, TraceableOrderView>()
                .ForMember(view => view.CollectingState, opt => opt.MapFrom(entity => (OrderCollectingState)entity.CollectingState))
                .ForMember(view => view.DeliveryId, opt => opt.MapFrom(entity => (FozzyDeliveryType)entity.DeliveryId))
                .ForMember(view => view.Driver, opt => opt.MapFrom(entity => entity.GetDriver()))
                .ForMember(view => view.MerchantId, opt => opt.MapFrom(entity => (Merchant)entity.MerchantId))
                .ForMember(view => view.OrderFrom, opt => opt.MapFrom(entity => (FozzyOrderOrigin)entity.OrderFrom))
                .ForMember(view => view.OrderStatus, opt => opt.MapFrom(entity => entity.OrderStatus))
                .ForMember(view => view.PaymentId, opt => opt.MapFrom(entity => (FozzyPaymentType)entity.PaymentId))
                .ForMember(view => view.Picker, opt => opt.MapFrom(entity => entity.GetPicker())) 
                                                                               
                .IncludeBase<VersionedEntity<Guid>, VersionedEntityView<Guid>>()
                ;
        }
    }
}
