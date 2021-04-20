namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class SearchProductViewModel
    {
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string ProductId { get; set; }
        public string BrandId { get; set; }
        public int PageNumber { get; set; }
    
        public int PageSize { get; set; }
    }

    public class SearchShopProductViewModel: SearchProductViewModel
    {
        public string ShopId { get; set; }
    }

    public class SearchShops{
        public string ShopId { get; set; }
        public string City { get; set; }
        public string CategoryId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchProductByNameCategoryIdSubCategoryId
    {
        public string ShopId { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
