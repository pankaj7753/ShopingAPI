using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class UserForRegisterViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Password between 4 to 8 character.")]
        public string Password { get; set; }
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile must be 10 digits only.")]
        public string Mobile { get; set; }

    }
}
