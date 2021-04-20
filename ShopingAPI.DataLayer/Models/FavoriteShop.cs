using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class FavoriteShop:CommanProperty
    {
        public string UserId { get; set; }
        public string ShopId { get; set; }
    }
}
