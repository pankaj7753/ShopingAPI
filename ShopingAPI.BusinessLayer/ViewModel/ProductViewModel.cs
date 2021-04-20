using System;

namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class ProductViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string[] ImageUrl { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public decimal MRP { get; set; }
        public decimal Selling { get; set; }
        public decimal Discount { get; set; }
        public string UnitType { get; set; }
        public int UnitId { get; set; }
        public int? ItemCount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsChecked { get; set; }

    }

    public class ProductAutoCompleteViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class VendorProductViewModel: ProductViewModel
    {
        public string Id { get; set; }
        public bool IsOutStock { get; set; }
        public string[] ProductUrl { get;  set; }
        public decimal SubTotalOnSelling { get;  set; }
        public decimal SubTotalOnMRP { get;  set; }
        public string CurrencyType { get; set; }

    }
}
