using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class UserRole: IdentityUserRole<string>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class Role : IdentityRole<string>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }

    }
}
