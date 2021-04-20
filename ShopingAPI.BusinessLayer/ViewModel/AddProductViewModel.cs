using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class AddProductViewModel: CommanProperty
    {
        public string ProductName { get; set; }
        public IFormFile[] ImageUrl { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public string CategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public string SubCategoryId { get; set; }
        public string BrandId { get; set; }
        public Brand Brands { get; set; }
    }
}
