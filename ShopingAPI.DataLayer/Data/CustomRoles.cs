using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Data
{
    public static class CustomRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Vendor = "Vendor";
        public const string Employee = "VendorEmployee";
        public const string VendorOrEmployee = Vendor+","+Employee;
        public const string AdminOrVendor = Admin + "," + Vendor;
    }
}
