using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class ProductPrice:CommanProperty
    {
        public decimal MRPPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
