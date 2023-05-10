using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MagicVilla_Web.Controllers
{
    public class AuthController : Controller
    {

        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(loginRequestDTO);
            if(response != null && response.isSuccess)
            {
                LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, model.User.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, model.User.Role));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
                HttpContext.Session.SetString(SD.SessionToken, model.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Custom Error", response.ErrorMessage.FirstOrDefault());
                return View(loginRequestDTO);
            }
            //return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationDTO registrationDTO)
        {
            APIResponse result = await _authService.RegisterAsync<APIResponse>(registrationDTO);
            if(result != null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken, "");
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

    }
}
