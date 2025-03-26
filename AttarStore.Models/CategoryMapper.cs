using AttarStore.Entities;
using System;
using System.Collections.Generic;

namespace AttarStore.Models
{
    // Mapper used for displaying categories with the list of products
    public class CategoryMapper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }  // Image URL
        //public int? ParentId { get; set; } // Parent ID for subcategories
        public List<ProductCategoryMapperView> Products { get; set; } = new List<ProductCategoryMapperView>();
    }

    // Mapper used for creating categories
    public class CategoryCreateMapper
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        //public int? ParentId { get; set; }  // Parent ID for creating nested categories
    }

    // Mapper used for updating categories
    public class CategoryUpdateMapper
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        //public int? ParentId { get; set; }  // Parent ID for updating nested categories
    }

    // A simplified view of the category for displaying purposes
    public class CategoryMapperView
    {
        public string Name { get; set; }
    }

    // DTO used to display products within a category

}
