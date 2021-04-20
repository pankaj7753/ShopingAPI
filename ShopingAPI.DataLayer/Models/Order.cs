using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class Order
    {
        public string Id { get; set; }

        public string OrderSummaryId { get; set; }
        public OrderSummary OrderSummary { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal MRPPrice { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal SellingPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public int Quantity { get; set; }
        public string ProductPriceId { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public UserAddress UserAddress { get; set; }
    }
}
