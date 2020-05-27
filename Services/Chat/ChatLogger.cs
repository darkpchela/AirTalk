using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AirTalk
{
    public class ChatLogger
    {
        ILogger<ChatLogger> logger;
        public List<string> allUsers { get; private set; }
        public List<int> allGroupsId { get; private set; }
        public List<UserToGroups> userToGroups { get; private set; }

        public ChatLogger(ILogger<ChatLogger> logger)
        {
            this.logger = logger;
            allUsers = new List<string>();
            allGroupsId = new List<int>();
            userToGroups = new List<UserToGroups>();
        }

        public void Connected(HttpContext context)
        {
            string userName = context.User.Identity.Name;
            if (!allUsers.Contains(userName))
                allUsers.Add(userName);

            int groupId = (int)context.Session.GetInt32("currentThemeId");
            if (!allGroupsId.Contains(groupId))
                allGroupsId.Add(groupId);

            UserToGroups tempUserToGroups = userToGroups.Find(ug => ug.userName == userName);
            if (tempUserToGroups == null)
            {
                userToGroups.Add(new UserToGroups(userName, groupId));
            }
            else if (!tempUserToGroups.usersGroupsId.Contains(groupId))
            {
                tempUserToGroups.usersGroupsId.Add(groupId);
            }
        }

        public void Disconnected(HttpContext context)
        {
            string userName = context.User.Identity.Name;
            int groupId = (int)context.Session.GetInt32("currentThemeId");
            UserToGroups tempUserToGroups = userToGroups.Find(ug => ug.userName == userName);
            tempUserToGroups.usersGroupsId.Remove(groupId);
            if (tempUserToGroups.usersGroupsId.Count==0)
            {
                allUsers.Remove(userName);
                userToGroups.Remove(tempUserToGroups);
            }
        }

        public class Group
        {
            public int id { get; private set; }
            public List<string> usersAtGroup { get; private set; }
            public Group(int id, string userCreator)
            {
                this.id = id;
                usersAtGroup = new List<string>() { userCreator };
            }
        }

        public class UserToGroups
        {
            public string userName { get; private set; }
            public List<int> usersGroupsId { get; private set; }

            public UserToGroups(string userName)
            {
                this.userName = userName;
                this.usersGroupsId = new List<int>();
            }
            public UserToGroups(string userName, int groupId)
            {
                this.userName = userName;
                this.usersGroupsId = new List<int>();
                this.usersGroupsId.Add(groupId);
            }
        }
        public void GetChatLoggerInfo()
        {
            string log = "All users at chats: ";
            foreach (var u in allUsers)
            {
                log += u + " ";
            }
            logger.LogInformation(log);

            log = "All alive chatGroups: ";
            foreach (var g in allGroupsId)
            {
                log += g.ToString() + " ";
            }
            logger.LogInformation(log);

            foreach (var ug in userToGroups)
            {
                log = "User " + ug.userName + " chatting at groups: ";
                foreach (var g in ug.usersGroupsId)
                {
                    log += g.ToString();
                }
                logger.LogInformation(log);
            }
        }
    }
}