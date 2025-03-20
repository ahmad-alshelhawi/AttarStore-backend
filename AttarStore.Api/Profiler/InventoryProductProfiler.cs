using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;

namespace AttarStore.Api.Profiler
{
    public class InventoryProductProfiler : Profile
    {
        public InventoryProductProfiler()
        {
            this.CreateMap<InventoryProduct, InventoryProductMapper>().ReverseMap();
            this.CreateMap<InventoryProduct, CreateInventoryProductMapper>().ReverseMap();
            this.CreateMap<InventoryProduct, UpdateInventoryProductMappper>().ReverseMap();

        }
    }
}
