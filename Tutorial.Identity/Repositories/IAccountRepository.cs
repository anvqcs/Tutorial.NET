using Microsoft.AspNetCore.Identity;
using Tutorial.Identity.Models;

namespace Tutorial.Identity.Repositories
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> RegisterAsync(RegisterModel model);
        public Task<string> LoginAsync(LoginModel model);
    }
}
