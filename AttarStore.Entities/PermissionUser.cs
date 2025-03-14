using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Entities
{
    public class PermissionUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public User User { get; set; }
        public Permission Permission { get; set; }
    }
}
