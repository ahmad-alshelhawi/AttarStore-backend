using AttarStore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Models
{

    public class CategoryMapper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ProductCategoryMapperView> Products { get; set; }
    }

    public class CategoryCreateMapper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class CategoryUpdateMapper
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
    public class CategoryMapperView
    {
        public string Name { get; set; }
    }
}

