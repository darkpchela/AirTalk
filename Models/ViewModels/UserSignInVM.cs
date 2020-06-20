using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AirTalk.Models.ViewModels
{
    public class UserSignInVM
    {
        [Required]
        public string loginOrEmail { get; set; }
        [Required]
        [UIHint("Password")]
        public string password     { get; set; }
    }
}
