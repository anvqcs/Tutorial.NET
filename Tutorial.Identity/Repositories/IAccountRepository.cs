using Microsoft.AspNetCore.Identity;
using Tutorial.Identity.Models;

namespace Tutorial.Identity.Repositories
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        public Task<SignInResult> SignInAsync(LoginViewModel model);
        public Task SignOutAsync();
    }
}
