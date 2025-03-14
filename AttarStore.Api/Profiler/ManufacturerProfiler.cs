using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;

namespace AttarStore.Api.Profiler
{
    public class ManufacturerProfiler : Profile
    {
        public ManufacturerProfiler()
        {
            this.CreateMap<Manufacturer, ManufacturerMapper>().ReverseMap();
            this.CreateMap<Manufacturer, ManufacturerCreateMapper>().ReverseMap();
            this.CreateMap<Manufacturer, ManufacturerUpdateMapper>().ReverseMap();
            this.CreateMap<Manufacturer, ManufacturerCategoryMapperView>().ReverseMap();

        }
    }
}
