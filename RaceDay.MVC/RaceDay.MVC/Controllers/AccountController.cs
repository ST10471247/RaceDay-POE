using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RaceDay.MVC.Models.ViewModels;
using RaceDay.MVC.Services;
using System.IdentityModel.Tokens.Jwt;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync("auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                string token = result.token;

                // Save JWT in a secure cookie
                Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to false if testing without HTTPS
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToAction("Index", "Events");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync("auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                // Auto-login after successful registration
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Registration failed. Email may already exist.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Index", "Home");
        }
    }
}