using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tutorial.Identity.Data;
using Tutorial.Identity.Models;

namespace Tutorial.Identity.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountRepository(UserManager<ApplicationUser> userManager
                                , SignInManager<ApplicationUser> signInManager
                                , RoleManager<IdentityRole> roleManager
                                , IConfiguration configuration) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
             _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<SignInResult> SignInAsync(LoginViewModel model)
        {
            SignInResult result = null;
            try
            {
                result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded) 
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return result;
        }
    }
}
