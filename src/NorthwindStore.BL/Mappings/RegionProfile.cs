using System;
using AutoMapper;
using NorthwindStore.BL.DTO;
using NorthwindStore.DAL.Entities;

namespace NorthwindStore.BL.Mappings
{
    public class RegionProfile : Profile
    {
        public RegionProfile()
        {
            CreateMap<Region, RegionDTO>();
            CreateMap<RegionDTO, Region>()
                .ForMember(r => r.Id, m => m.Ignore())
                .ForMember(r => r.Territories, m => m.Ignore());
        }
    }
}
