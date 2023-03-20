using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable 
namespace BiographyWebApp.Models
{
    public enum UserRole
    {
        User,
        Admin,
    }

    public partial class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        //either Admin or User
        public UserRole Role { get; set; }
        public string Password { get; set; }

        public bool IsEmailVerified { get; set; }

        public ActivationCode? ActivationCode { get; set; }
        public ResetPasswordCode? ResetPasswordCode { get; set; }
    }
}
