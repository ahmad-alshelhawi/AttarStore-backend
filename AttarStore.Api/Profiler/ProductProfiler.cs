using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;

namespace AttarStore.Api.Profiler
{
    public class ProductProfiler : Profile
    {
        public ProductProfiler()
        {
            this.CreateMap<Product, ProductMapper>().ReverseMap();
            this.CreateMap<Product, ProductCreateMapper>().ReverseMap();
            this.CreateMap<Product, ProductUpdateMapper>().ReverseMap();
            this.CreateMap<Product, ProductCategoryMapperView>().ReverseMap();
            this.CreateMap<Product, ProductimageMapperView>().ReverseMap();

        }
    }
}
