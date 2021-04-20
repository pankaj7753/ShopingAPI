using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class OrderSummary:CommanProperty
    {
        public int ItemCount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? DeliveryCost { get; set; }
        public string PromoCode { get; set; }
        public int OrderStatusId { get; set; }
        public string StatusName { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentStatus { get; set; }
        public string UserId { get; set; }
        public string UserAddressDetails { get; set; }
        public string ShopAddressDetails { get; set; }
        public string OrdersDetails { get; set; }
        public string CurrencyType { get; set; }
        public string ShopId { get; set; }

    }
}
