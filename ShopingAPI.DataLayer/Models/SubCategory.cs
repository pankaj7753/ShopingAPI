using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class SubCategory:CommanProperty
    {
        [Required]
        public string SubCategoryName { get; set; }
        public Category Category { get; set; }
        public string CategoryId { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
