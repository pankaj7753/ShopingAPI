using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class Location:CommanProperty
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName{ get; set; }
        public ICollection<Shop> Shops { get; set; }

    }
}
