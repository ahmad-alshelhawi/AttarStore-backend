using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;


namespace AttarStore.Api.profiler
{
    public class ClientProfiler : Profile
    {
        public ClientProfiler()
        {
            this.CreateMap<Client, ClientMapperCreate>().ReverseMap();
            this.CreateMap<Client, ClientMapperView>().ReverseMap();
            this.CreateMap<Client, ClientView>().ReverseMap();
            this.CreateMap<Client, ClientMapperUpdate>().ReverseMap();


        }
    }
}
