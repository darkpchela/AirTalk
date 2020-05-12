using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirTalk.Models.InsideModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AirTalk.Models.ViewModels
{
    public class MainIndexViewModel
    {
        public IEnumerable<Theme> allThemes { get; set; }
        public IEnumerable<UserModel> allUsers { get; set; }
        public UserModel currentUser { get; set; }
        public Theme currentTheme { get; set; }

        public MainIndexViewModel(MainDbContext db)
        {
            allThemes = db.themes.ToList();
            allUsers = db.users.ToList();
        }
    }
}
