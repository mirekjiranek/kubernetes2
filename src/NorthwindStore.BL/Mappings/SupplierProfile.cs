using AutoMapper;
using NorthwindStore.BL.DTO;
using NorthwindStore.DAL.Entities;

namespace NorthwindStore.BL.Mappings
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<Supplier, SupplierBasicDTO>();

            CreateMap<Supplier, SupplierListDTO>();

            CreateMap<Supplier, SupplierDetailDTO>();
            CreateMap<SupplierDetailDTO, Supplier>()
                .ForMember(s => s.Id, m => m.Ignore())
                .ForMember(s => s.Products, m => m.Ignore());
        }
    }
}