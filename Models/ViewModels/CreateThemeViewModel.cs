using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AirTalk.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace AirTalk.Models.ViewModels
{
    public class CreateThemeViewModel
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string body { get; set; }
        [Required]
        public DateTime creationTime    { get; set; }
        public IEnumerable<string> tags { get; set; }

        public Theme GetDBModel(int userId)
        {
            Theme theme = new Theme { name = this.name, body = this.body, creationTime = DateTime.UtcNow, userCreatorId = userId };
            return theme;
        }
    }
}
