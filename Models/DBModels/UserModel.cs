using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AirTalk.Models.DBModels
{
    public class UserModel
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string login { get; set; }
        public string email     { get; set; }
        public string password  { get; set; }
        public UserAccountRigths rigths { get; set; }
        public bool online  { get; set; }
        public string icon { get; set; }
    }
}
