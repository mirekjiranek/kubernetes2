using AutoMapper;
using NorthwindStore.BL.DTO;
using NorthwindStore.DAL.Entities;

namespace NorthwindStore.BL.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryBasicDTO>();

            CreateMap<Category, CategoryListDTO>()
                .ForMember(c => c.ImageUrl, m => m.MapFrom(c => $"/image/category/{c.Id}"));

            CreateMap<Category, CategoryDetailDTO>();

            CreateMap<CategoryDetailDTO, Category>()
                .ForMember(c => c.Id, m => m.Ignore())
                .ForMember(c => c.Products, m => m.Ignore());
        }
    }
}