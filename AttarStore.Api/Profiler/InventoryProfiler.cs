using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;

namespace AttarStore.Api.Profiler
{
    public class InventoryProfiler : Profile
    {
        public InventoryProfiler()
        {
            this.CreateMap<Inventory, InventoryMapper>().ReverseMap();
            this.CreateMap<Inventory, CreateInventoryMapper>().ReverseMap();
            this.CreateMap<Inventory, UpdateInventoryMapper>().ReverseMap();

        }
    }
}
