using System.ComponentModel.DataAnnotations;

namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class UserForLoginViewModel
    {
        [Required]
        [Display(Name ="Username")]
        public string UserName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Password 4 to 50 char only")]
        public string Password { get; set; }
    }


   
}
