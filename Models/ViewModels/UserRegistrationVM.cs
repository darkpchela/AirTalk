using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using AirTalk.Models.DBModels;

namespace AirTalk.Models.ViewModels
{
    public class UserRegistrationVM
    {
        [Required]
        //[RegularExpression(@"^[a-zA-Z0-9]+$")]
        [Remote(action:"CheckLogin", controller:"Account", ErrorMessage ="This login is already in use")]
        [Display(Name = "Login")]
        public string login { get; set; }

        [EmailAddress]
        [Remote(action:"CheckEmail",controller:"Account", ErrorMessage ="This email is already in use")]
        [Display(Name = "Email")]
        public string email     { get; set; }

        [Required]
        [UIHint("Password")]
        [Display(Name = "Password")]
        //[RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$")]
        public string password  { get; set; }

        [Required]
        [Compare("password")]
        [UIHint("Password")]
        [Display(Name = "Confirm Password")]
        public string confirmPassword { get; set; }
        public UserModel GetDbModel()
        {
            UserModel user = new UserModel {login=this.login,
            email=this.email, password=this.password};
            return user;
        }
    }
}
