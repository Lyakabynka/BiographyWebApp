using System.ComponentModel.DataAnnotations;

namespace BiographyWebApp.Models
{
    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password should contain at least 6 characters.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        public string ResetPasswordCode { get; set; }
    }
}
