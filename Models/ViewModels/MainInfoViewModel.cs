using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirTalk.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AirTalk.Models.ViewModels
{
    public class MainInfoViewModel
    {
        private readonly MainDbContext dbContext;
        public IEnumerable<Theme> allThemes { get; set; }
        public IEnumerable<UserModel> allUsers { get; set; }
        public UserModel currentUser { get; set; }
        public Theme currentTheme { get; set; }
        public IEnumerable<UserModel> usersInTheme { get; private set; }
        public IEnumerable<Message> messages { get; set; }
        public Dictionary<int, string> users;

        public MainInfoViewModel(MainDbContext db)
        {
            allThemes = db.themes.ToList();
            allUsers = db.users.ToList();
            messages = db.messages.Where(m => m.themeId == currentTheme.id);
            messages = messages.OrderBy(m => m.time);

            var tempUsersData = db.messages.Join(db.users,
                m => m.userSenderId,
                u => u.id,
                (m, u) => new
                {
                    userId = m.id,
                    userName = u.login
                });

            foreach (var u in tempUsersData)
            {
                users.Add(u.userId, u.userName);
            }
        }

    }
}
