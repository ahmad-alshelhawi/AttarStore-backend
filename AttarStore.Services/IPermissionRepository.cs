
using AttarStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Services
{

    public interface IPermissionRepository
    {
        Task<PermissionUser[]> GetPermissionsById(string name);
        Task<bool> checkPermission(string permission, string clientId);
    }

}
