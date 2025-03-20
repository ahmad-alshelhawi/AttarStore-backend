using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Models
{

    public class InventoryMapper
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Location { get; set; }
    }

    public class CreateInventoryMapper
    {
        [Required]
        public string Name { get; set; }
        public string Location { get; set; }
    }

    public class UpdateInventoryMapper
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Location { get; set; }
    }
}

