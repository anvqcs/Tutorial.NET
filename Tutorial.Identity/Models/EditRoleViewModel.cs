using System.ComponentModel.DataAnnotations;

namespace Tutorial.Identity.Models
{
    public class EditRoleViewModel
    {
        [Required]
        public required string Id { get; set; }
        [Required(ErrorMessage = "Role Name is Required")]
        public required string RoleName { get; set; }
        public string? Description { get; set; }
        public List<string>? Users { get; set; }
        public List<string>? Claims { get; set; }
    }
}
