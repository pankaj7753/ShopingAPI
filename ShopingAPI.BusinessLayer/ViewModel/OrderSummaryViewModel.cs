using System;

namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class OrderSummaryViewModel
    {
        public string OrderSummaryId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal DeliveryAmount { get; set; }
        public string PromoCode { get; set; }
        public int ItemCount { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentStatus { get; set; }
        public string UserAddressDetails { get; set; }
        public string ShopAddressDetails { get; set; }
        public string OrdersDetails { get; set; }
        public string ProductUrl { get; set; }
        public string UserId { get; set; }
        public string CurrencyType { get; set; }
        public string ShopId { get; set; }
        public int OrderStatusId { get; set; }
        public string StatusName { get; set; }
    }
}
