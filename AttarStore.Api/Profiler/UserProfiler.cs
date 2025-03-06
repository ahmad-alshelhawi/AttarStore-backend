using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;


namespace AttarStore.Api.profiler
{
    public class UserProfiler : Profile
    {
        public UserProfiler()
        {
            this.CreateMap<User, UserMapperCreate>().ReverseMap();
            this.CreateMap<User, UserMapperView>().ReverseMap();
            this.CreateMap<User, UserView>().ReverseMap();
            this.CreateMap<User, UserMapperUpdate>().ReverseMap();


        }
    }
}
