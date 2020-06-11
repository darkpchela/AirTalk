using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AirTalk.Services
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize<T>(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
        public static Dictionary<string, string> SessionInfo(this ISession _session)
        {

            Dictionary<string, string> session = new Dictionary<string, string>();
            foreach (var key in _session.Keys)
            {
                session.Add(key, _session.GetString(key));
            }
            return session;
        }
    }
}
