using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirTalk.Models.DBModels;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserSignInVM user)
        {
            UserModel checker = null;
            try
            {
                checker = await db.users.FirstAsync(u => u.login == user.loginOrEmail || u.email == user.loginOrEmail);
            }
            catch
            { }

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
                HttpContext.Session.SetInt32("id", checker.id);
                HttpContext.Session.SetString("login", checker.login);
                //HttpContext.Session.SetString("rights",checker.rigths.ToString());
                return RedirectToAction("Index", "Home");
            }

            return PartialView("SignIn", user);

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
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

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckLogin(string login)
        {
            var checker = db.users.FirstOrDefault(u => u.login == login);
            if (checker == null)
                return Json(true);
            else
                return Json(false);
        }

        public IActionResult CheckEmail(string email)
        {
            var checker = db.users.FirstOrDefault(u => u.email == email);
            if (checker == null)
                return Json(true);
            else
                return Json(false);
        }
    }
}