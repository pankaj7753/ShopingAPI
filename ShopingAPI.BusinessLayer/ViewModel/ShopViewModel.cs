namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class ShopViewModel
    {
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public decimal DeliveryCost { get; set; }
        public bool IsPriceVisible { get; set; }
        public string ShopImageUrl { get; set; }
        public string GSTNumber { get; set; }
        public string NoticeByShop { get; set; }
    }
}
