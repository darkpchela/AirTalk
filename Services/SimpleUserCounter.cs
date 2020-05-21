using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirTalk.Models.DBModels;

namespace AirTalk.Services
{
    public class UserCounterService
    {
        readonly MainDbContext db;
        event EventHandler UserSignedIn;
        event EventHandler UserSignedOut;
        public int onlineCount { get; private set; }
        public int totalCount { get; private set; }

        public UserCounterService(MainDbContext mainDb)
        {
            db = mainDb;
            totalCount = db.users.Count();
        }
        private void OnUserSignedIn(EventArgs e)
        {
            UserSignedIn?.Invoke(this, e);
        }
        private void OnUserSignedOut(EventArgs e)
        {
            UserSignedOut?.Invoke(this, e);
        }
    }
}
