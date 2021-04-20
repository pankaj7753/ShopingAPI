using System.ComponentModel.DataAnnotations;

namespace ShopingAPI.BusinessLayer.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
