using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Tutorial.Identity.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is Required")]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote(action: "IsEmailAvailable", controller: "Accounts")]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
