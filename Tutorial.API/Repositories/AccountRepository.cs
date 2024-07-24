using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tutorial.API.Data;
using Tutorial.API.Helper;
using Tutorial.API.Models;

namespace Tutorial.API.Repositories
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
        public async Task<string> SignInAsync(SignInModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (user == null || !passwordValid) 
            {
                return string.Empty;
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    claims: authClaims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha512)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
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
                if (!await _roleManager.RoleExistsAsync(ApplicationRole.Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(ApplicationRole.Customer));
                }
                await _userManager.AddToRoleAsync(user, ApplicationRole.Customer);
            }
            return result;
        }
    }
}
