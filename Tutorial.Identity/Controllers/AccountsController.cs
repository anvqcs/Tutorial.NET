using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tutorial.Identity.Models;
using Tutorial.Identity.Repositories;

namespace Tutorial.Identity.Controllers
{
    public class AccountsController : Controller
    {
        private IAccountRepository _repository;

        public AccountsController(IAccountRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _repository.RegisterAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
    }
}
