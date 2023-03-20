using System.ComponentModel.DataAnnotations;

#pragma warning disable
namespace BiographyWebApp.Models
{
    public class UserLoginModel
    {
        [Display(Name = "Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password should contain at least 6 characters.")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberAuthorization { get; set; }

        public string ReturnUrl { get; set; }
    }
}
