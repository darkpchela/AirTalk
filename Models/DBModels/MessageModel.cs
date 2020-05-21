using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirTalk.Models.DBModels
{
    public class Message
    {
        public int id { get; set; }
        public string text   { get; set; }
        public DateTime time { get; set; }
        public int userSenderId { get; set; }
        public int themeId { get; set; }

    }
}
