using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class Shop:CommanProperty
    {

        public string ShopName { get; set; }
        public string ShopCode { get; set; }
        public string NoticeByShop { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public Location Location { get; set; }
        public string GSTNumber { get; set; }
        public string LocationId { get; set; }
        public string UserId { get; set; }
        public bool IsPriceVisible { get; set; }
        public string tableName { get; set; }
        public string CurrencyType { get; set; }
        public string CategoryIds { get; set; }
        public string SubCategoryIds { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? DeliveryCost { get; set; }
        public string ShopImageUrl { get; set; }

    }
}
