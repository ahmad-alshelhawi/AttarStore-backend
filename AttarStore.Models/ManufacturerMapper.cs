using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Models
{
    public class ManufacturerMapper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<ProductCategoryMapperView> Products { get; set; }

    }

    public class ManufacturerCreateMapper
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class ManufacturerUpdateMapper
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class ManufacturerCategoryMapperView
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }
}
