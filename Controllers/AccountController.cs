using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirTalk.Models.InsideModels;
using AirTalk.Models.ViewModels;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AirTalk.Controllers
{
    public class AccountController : Controller
    {
        private readonly MainDbContext db;
        public AccountController(MainDbContext db)
        {
            this.db = db;
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserSignInVM user)
        {
            var checker = await db.users.FirstOrDefaultAsync(u => u.login == user.loginOrEmail || u.email == user.loginOrEmail);

            if (checker == null)
            {
                ModelState.AddModelError(nameof(user.loginOrEmail), "Account is not exists");
            }
            else
            if (user.password != checker.password)
            {
                ModelState.AddModelError(nameof(user.password), "Incorrect password");
            }

            if (ModelState.IsValid)
            {
                //HttpContext.Session.SetString(sessionKey, checker.id.ToString());
                await Authenticate(user.loginOrEmail);
                //HttpContext.Session.SetInt32("id", checker.id);
                //HttpContext.Session.SetString("login", checker.login);
                //HttpContext.Session.SetString("rights",checker.rigths.ToString());
                return RedirectToAction("Index", "Main");
            }             
                return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(string loginOrEmail)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, loginOrEmail)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}