namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class CartViewModel
    {
            public string ProductId { get; set; }
            public string ProductPriceId { get; set; }
            public string ProductName { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
            public decimal MRPPrice { get; set; }
            public decimal SellingPrice { get; set; }
            public int DiscountPercentage { get; set; }
            public string UnitType { get; set; }
            public int UnitId { get; set; }
            public int Quantity { get; set; }
            public decimal Total { get; set; }
            public string UserId { get; set; }
    }
}
