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
using AirTalk.Services;
using System.Text.Json;
namespace AirTalk.Controllers
{
    public class AccountController : Controller
    {
        private readonly MainDbContext db;
        TerminalResultBuilder resultBuilder;
        public AccountController(MainDbContext db, TerminalResultBuilder resultBuilder)
        {
            this.resultBuilder = resultBuilder;
            this.db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(UserRegistrationVM registrationVM)
        {
            if (db.users.Any(u => u.login == registrationVM.login))
                ModelState.AddModelError(nameof(registrationVM.login), "This login is already in use");

            if (registrationVM.email != null && db.users.Any(u => u.email == registrationVM.email))
                ModelState.AddModelError(nameof(registrationVM.email), "This email is already in use");

            if (registrationVM.password != registrationVM.confirmPassword)
                ModelState.AddModelError(nameof(registrationVM.password), "Passwords are not same");

            if (ModelState.IsValid)
            {
                UserModel user = registrationVM.GetDbModel();
                db.users.Add(user);
                db.SaveChanges();
                await Authenticate(user.login);
                HttpContext.Session.SetString("login", user.login);
                var session = HttpContext.Session.SessionInfo();
                resultBuilder.AddJSFuncModel("updateUserInfo", session);
                resultBuilder.AddJSFuncInline("reloadHubConnection");
                return Json(resultBuilder.Build());
            }
            else
            {
                resultBuilder.AddAspView(this, "Registration", registrationVM);
                return Json(resultBuilder.Build());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserSignInVM user)
        {
            UserModel checker = null;
            try
            {
                checker = await db.users.FirstAsync(u => u.login == user.loginOrEmail || u.email == user.loginOrEmail);
                if (user.password != checker.password)
                {
                    ModelState.AddModelError(nameof(user.password), "Incorrect password");
                }
            }
            catch
            { 
                ModelState.AddModelError(nameof(user.loginOrEmail), "Account is not exists");
            }

            if (ModelState.IsValid)
            {
                await Authenticate(user.loginOrEmail);
                HttpContext.Session.SetString("login", checker.login);
                var session = HttpContext.Session.SessionInfo();
                resultBuilder.AddJSFuncModel("updateUserInfo", session);
                resultBuilder.AddJSFuncInline("reloadHubConnection");
                return Json(resultBuilder.Build());
            }
            resultBuilder.AddAspView(this,"SignIn", user);
            return Json(resultBuilder.Build());

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.SetString("login", "unsigned");
            var session = HttpContext.Session.SessionInfo();
            resultBuilder.AddJSFuncModel("updateUserInfo", session);
            resultBuilder.AddJSFuncInline("reloadHubConnection");
            return Json(resultBuilder.Build());
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
            if (db.users.Any(u=>u.login==login))
                return Json(false);
            else
                return Json(true);
        }
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckEmail(string email)
        {
            if (db.users.Any(u => u.email == email))
                return Json(false);
            else
                return Json(true);
        }
    }
}