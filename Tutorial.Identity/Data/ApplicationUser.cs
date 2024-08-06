using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tutorial.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(256)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(256)]
        public string LastName { get; set; } = string.Empty;
    }
}
