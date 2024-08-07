using Microsoft.AspNetCore.Identity;

namespace Tutorial.Identity.Data
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
