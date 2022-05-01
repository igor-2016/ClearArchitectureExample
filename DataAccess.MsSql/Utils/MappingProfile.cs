using AutoMapper;
using DataAccess.Interfaces.Dto;
using Entities.Models.Expansion;

namespace DataAccess.MsSql.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<string, byte[]>().ConvertUsing(str => string.IsNullOrEmpty(str) ? Convert.FromBase64String("") : Convert.FromBase64String(str));
            CreateMap<byte[], string>().ConvertUsing(bytes => bytes == null ?  Convert.ToBase64String(new byte[0]) : Convert.ToBase64String(bytes));
            CreateMap<EntityDto, Entity<Guid>>()
                .ReverseMap();

            CreateMap<RowVersionEntityDto, VersionedEntity<Guid>>()
                .IncludeBase<EntityDto, Entity<Guid>>()
                .ReverseMap();

            CreateMap<TraceableOrderDto, TraceableOrder>()
                .IncludeBase<RowVersionEntityDto, VersionedEntity<Guid>>()
                .ReverseMap();

            CreateMap<TraceableOrderDto, TraceableOrderDto>();


            CreateMap<TraceableOrderItemDto, TraceableOrderItem>()
                .IncludeBase<EntityDto, Entity<Guid>>()
                .ReverseMap();

            CreateMap<TraceableOrderItemDto, TraceableOrderItemDto>();
        }
    }
}
