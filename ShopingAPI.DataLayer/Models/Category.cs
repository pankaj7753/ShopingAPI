using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class Category:CommanProperty
    {
        [Required]
        public string Name { get; set; }
        [MaxLength(200)]
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
