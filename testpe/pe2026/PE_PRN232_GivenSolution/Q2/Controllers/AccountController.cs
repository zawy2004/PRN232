using Microsoft.AspNetCore.Mvc;
using Q2.Models;
using Q2.Services;

namespace Q2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _api;
        public AccountController(ApiClient api) => _api = api;

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var (ok, data) = await _api.LoginAsync(model.Email, model.Password);
            if (!ok || data is null)
            {
                model.Error = "Invalid email or password";
                return View(model);
            }

            HttpContext.Session.SetString("Token", data.Token);
            HttpContext.Session.SetString("Email", data.Email);
            HttpContext.Session.SetString("Role", data.Role);

            return RedirectToAction("Index", "Equipments");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Equipments");
        }
    }
}
