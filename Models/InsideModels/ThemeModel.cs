using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirTalk.Models.InsideModels
{
    public class Theme
    {
        public int      id   { get; set; }
        public string   name { get; set; }
        public string   body { get; set; }
        public DateTime creationTime  { get; set; }
        public int      userCreatorId { get; set; }
        IEnumerable<string> tags { get; set; }
    }
}
