using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tutorial.Identity.Data;
using Tutorial.Identity.Models;

namespace Tutorial.Identity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(ApplicationDbContext context
                                      , UserManager<ApplicationUser> userManager
                                      , RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #region Roles
        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            List<ApplicationRole> roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the role already exists
                bool roleExists = await _roleManager.RoleExistsAsync(roleModel?.RoleName);
                if (roleExists)
                {
                    ModelState.AddModelError("", "Role Already Exists");
                }
                else
                {
                    // Create the role
                    // We just need to specify a unique role name to create a new role
                    ApplicationRole identityRole = new ApplicationRole
                    {
                        Name = roleModel?.RoleName,
                        Description = roleModel?.Description
                    };
                    // Saves the role in the underlying AspNetRoles table
                    IdentityResult result = await _roleManager.CreateAsync(identityRole);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(ListRoles));
                    }
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(roleModel);
        }
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string roleId)
        {
            ApplicationRole role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return View("Error");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Users = new List<string>(),
                Claims = new List<string>(),
            };
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            model.Claims = roleClaims.Select(x => x.Value).ToList();
            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    // Handle the scenario when the role is not found
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    role.Name = model.RoleName;
                    role.Description = model.Description;
                    // Update other properties if needed
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles"); // Redirect to the roles list
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);

        }
        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Role not found, handle accordingly
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                // Role deletion successful
                return RedirectToAction("ListRoles"); // Redirect to the roles list page
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            // If we reach here, something went wrong, return to the view
            return View("ListRoles", await _roleManager.Roles.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            ViewBag.RollName = role.Name;
            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            //First check whether the Role Exists or not
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult? result;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    //If IsSelected is true and User is not already in this role, then add the user
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    //If IsSelected is false and User is already in this role, then remove the user
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    //Don't do anything simply continue the loop
                    continue;
                }

                //If you add or remove any user, please check the Succeeded of the IdentityResult
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { roleId = roleId });
                }
            }

            return RedirectToAction("EditRole", new { roleId = roleId });
        }
        #endregion
        #region Users
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string UserId)
        {
            //First Fetch the User Details by UserId
            var user = await _userManager.FindByIdAsync(UserId);

            //Check if User Exists in the Database
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await _userManager.GetClaimsAsync(user);

            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            //Store all the information in the EditUserViewModel instance
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            //Pass the Model to the View
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            //First Fetch the User by Id from the database
            var user = await _userManager.FindByIdAsync(model.Id);

            //Check if the User Exists in the database
            if (user == null)
            {
                //If the User does not exists in the database, then return Not Found Error View
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                //If the User Exists, then proceed and update the data
                //Populate the user instance with the data from EditUserViewModel
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                //UpdateAsync Method will update the user data in the AspNetUsers Identity table
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    //Once user data updated redirect to the ListUsers view
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    //In case any error, stay in the same view and show the model validation error
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            //First Fetch the User you want to Delete
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                // Handle the case where the user wasn't found
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }
            else
            {
                //Delete the User Using DeleteAsync Method of UserManager Service
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // Handle a successful delete
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    // Handle failure
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View("ListUsers");
            }
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found.";
                return View("NotFound");
            }
            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;

            var model = new List<UserRolesViewModel>();

            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                UserRolesViewModel userRolesViewModel = new UserRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Description = role.Description,
                    IsSelected = false
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                    userRolesViewModel.IsSelected = true;
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            //fetch the list of roles the specified user belongs to
            var roles = await _userManager.GetRolesAsync(user);

            var results = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!results.Succeeded) 
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            List<string> RolesToBeAssigned = model.Where(x => x.IsSelected).Select(y => y.RoleName).ToList();
            if (RolesToBeAssigned.Any()) 
            {
                results = await _userManager.AddToRolesAsync(user, RolesToBeAssigned);
                if (!results.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot Add Selected Roles to User");
                    return View(model);
                }
            }
            return RedirectToAction("EditUser", new { UserId = UserId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string UserId)
        {
            var user = await _userManager.FindByIdAsync(userId: UserId);
            if (user == null) 
            {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found.";
                return View("NotFound");
            }
            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;

            var model = new UserClaimsViewModel
            {
                UserId = UserId
            };
            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            foreach (var claims in await _context.ClaimsStores.ToListAsync())
            {
                UserClaim userClaim = new UserClaim()
                {
                    ClaimType =  claims.Type,
                    ClaimValue = claims.Value
                };
                if (existingUserClaims.Any(x => x.Type == claims.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Cliams.Add(userClaim);
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) 
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found.";
                return View("NotFound");
            }
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded) 
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }
            var AllSelectedClaims = model.Cliams.Where(x => x.IsSelected)
                                                .Select(y => new Claim(y.ClaimType, y.ClaimValue))
                                                .ToList();
            if (AllSelectedClaims.Any()) 
            {
                result = await _userManager.AddClaimsAsync(user, AllSelectedClaims);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected claims to user");
                    return View(model);
                }
            }
            return RedirectToAction("EditUser", new { UserId = model.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageRoleClaims(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {RoleId} cannot be found";
                return View("NotFound");
            }
            ViewBag.RoleName = role.Name;
            var model = new RoleClaimsViewModel
            {
                RoleId = RoleId,
                Claims = new List<RoleClaim>()
            };

            var existingRoleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in await _context.ClaimsStores.ToListAsync())
            {
                RoleClaim roleClaim = new RoleClaim()
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };
                if (existingRoleClaims.Any(x => x.Type == claim.Type))
                {
                    roleClaim.IsSelected = true;
                }
                model.Claims.Add(roleClaim);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageRoleClaims(RoleClaimsViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.RoleId} cannot be found";
                return View("NotFound");
            }
            var claims = await _roleManager.GetClaimsAsync(role);
            
            foreach (var item in model.Claims.Select((value, i) => (value, i)))
            {
                var value = item.value;
                Claim claim = new Claim(value.ClaimType, value.ClaimValue);
                IdentityResult? result;
                if (value.IsSelected && !claims.Any(x => x.Type == value.ClaimType))
                {
                    result = await _roleManager.AddClaimAsync(role, claim);
                }
                else if (!value.IsSelected && claims.Any(x => x.Type == value.ClaimType))
                {
                    result = await _roleManager.RemoveClaimAsync(role, claim);
                }
                else
                    continue;
                if (result.Succeeded)
                {
                    if (item.i < model.Claims.Count - 1)
                        continue;
                    return RedirectToAction("EditRole", new { roleId = model.RoleId });
                }
                else
                {
                    ModelState.AddModelError("", "Cannot add or removed selected claims to role");
                    return View(model);
                }

            }
            return RedirectToAction("EditRole", new { roleId = model.RoleId });
        }
    }
}
