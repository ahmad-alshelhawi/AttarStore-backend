

using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;

namespace AttarStore.Api.profiler
{
    public class AdminProfiler : Profile
    {
        public AdminProfiler()
        {
            this.CreateMap<Admin, AdminMapperCreate>().ReverseMap();
            this.CreateMap<Admin, AdminMapperView>().ReverseMap();
            this.CreateMap<Admin, AdminView>().ReverseMap();
            this.CreateMap<Admin, AdminMapperUpdate>().ReverseMap();

        }
    }
}
