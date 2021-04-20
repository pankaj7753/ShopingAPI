using System.Collections.Generic;

namespace ShopingAPI.DataLayer.Models
{
    public class Brand: CommanProperty
    {
        public string BrandName { get; set; }
        public virtual ICollection<Product> Products { get; set; }

    }
}