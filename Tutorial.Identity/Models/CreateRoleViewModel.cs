using System.ComponentModel.DataAnnotations;

namespace Tutorial.Identity.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public required string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
