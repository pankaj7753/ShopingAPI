namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class CartIds
    {
        public int count { get; set; }
        public string id { get; set; }
    }
    public class UserAddressViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Pincode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Landmark { get; set; }
        public string PaymentMethod { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ProductAddToCart[] CartProducts { get; set; }
    }
}
