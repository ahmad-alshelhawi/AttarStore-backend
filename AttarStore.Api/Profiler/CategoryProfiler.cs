using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;

namespace AttarStore.Api.Profiler
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            this.CreateMap<Category, CategoryMapper>().ReverseMap();
            this.CreateMap<Category, CategoryCreateMapper>().ReverseMap();
            this.CreateMap<Category, CategoryUpdateMapper>().ReverseMap();
            this.CreateMap<Category, CategoryMapperView>().ReverseMap();

        }
    }
}
