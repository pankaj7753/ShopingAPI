using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{

    public class VendorProducts:CommanProperty
    {
        [Column(TypeName = "boolean")] 
        public bool IsOutStock { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Selling { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal MRP { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }
        public int UnitId { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string Description { get; set; }
        public string UnitType { get; set; }
        public string ShopId { get; set; }
        public string CurrencyType { get; set; }
        public int? ItemCount { get; internal set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
