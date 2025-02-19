using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContentTypeId { get; set; }
        public List<PermissionUser> Users { get; set; }

    }
}
