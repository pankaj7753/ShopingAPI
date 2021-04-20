using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class Product:CommanProperty
    {
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public string CategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public string SubCategoryId { get; set; }

        public string BrandId { get; set; }
        public Brand Brands { get; set; }


    }
}
