namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class CategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
    }
    public class SubCategoryViewModel
    {
        public string Id { get; set; }
        public string SubCategoryName { get; set; }
    }
}
