

using AttarStore.Entities;
using AttarStore.Models;
using AutoMapper;

namespace AttarStore.Api.profiler
{
    public class PermissionProfiler : Profile
    {
        public PermissionProfiler()
        {
            this.CreateMap<PermissionUser, PermissionUserMapper>().ReverseMap();
            this.CreateMap<Permission, PermissionsMapper>().ReverseMap();

        }
    }
}
