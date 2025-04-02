using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Models
{
    public class ProductMapper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string? ImageUrl { get; set; } // Optionally include Image URL in the view

        public CategoryMapperView Category { get; set; }

        public ManufacturerCategoryMapperView Manufacturer { get; set; }

    }

    public class ProductCreateMapper
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
    }

    public class ProductUpdateMapper
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
    }
    public class ProductCategoryMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

    }
}

public class ProductimageMapperView
{
    public string Name { get; set; }
    public string? ImageUrl { get; set; } // Optionally include Image URL in the view
}