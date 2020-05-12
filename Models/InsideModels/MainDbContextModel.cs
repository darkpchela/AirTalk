using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AirTalk.Models.InsideModels
{
    public class MainDbContext:DbContext
    {
        public DbSet<UserModel>    users    { get; set; }
        public DbSet<Theme>   themes   { get; set; }
        public DbSet<Message> messages { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
