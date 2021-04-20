using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class Cart:CommanProperty
    {
        [Column(TypeName = "decimal(10,2)")]
        public decimal MRPPrice { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal SellingPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
