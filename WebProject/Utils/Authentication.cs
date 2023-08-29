using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebProject.Models;

namespace WebProject.Utils
{
    public class Authentication
    {
        public static Guid CreateSession(string username)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/sessions.txt");

            var session = new Session(username);

            CRUD.AddInstance(session, filePath);

            var cookie = new HttpCookie("session", session.Id.ToString()) { 
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.MaxValue,
            };
            HttpContext.Current.Response.SetCookie(cookie);

            return session.Id;
        }

        public static void DeleteSession() {
            HttpContext.Current.Response.Cookies.Remove("session");
        }

        public static bool ValidateUser(string username)
        {

            var sessionId = HttpContext.Current.Request.Cookies.Get("session");
            if (sessionId == null || sessionId.Value == null) return false;

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/sessions.txt");

            var session = new Session(CRUD.RetrieveInstance(sessionId.Value.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            var user = new User(CRUD.RetrieveInstance(session.UserUsername, 1, filePath).ToArray());

            return session.UserUsername == username || user.Role == Role.ADMIN;
        }

        public static bool ValidateUser(Guid userId)
        {

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            var user = new User(CRUD.RetrieveInstance(userId.ToString(), 0, filePath).ToArray());

            return ValidateUser(user.Username);
        }

        public static User GetUserFromSession()
        {
            var sessionId = HttpContext.Current.Request.Cookies.Get("session");
            if (sessionId == null || sessionId.Value == null) return null;

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/sessions.txt");

            var session = new Session(CRUD.RetrieveInstance(sessionId.Value.ToString(), 0, filePath).ToArray());

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/users.txt");

            var user = new User(CRUD.RetrieveInstance(session.UserUsername.ToString(), 1, filePath).ToArray());

            return user;
        }
    }
}