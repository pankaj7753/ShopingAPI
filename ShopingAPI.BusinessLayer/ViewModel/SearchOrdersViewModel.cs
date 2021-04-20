using System;


namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class SearchOrdersViewModel
    {
        public string InvoiceNo { get; set; }
        public string Status { get; set; }
        public DateTime? OrderStartDate { get; set; }
        public DateTime? OrderEndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
