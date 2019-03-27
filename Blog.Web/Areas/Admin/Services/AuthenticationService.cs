using System;
using System.Security.Cryptography;
using System.Text;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web.Areas.Admin.Services
{
    public class AuthenticationService
    {
        private Database Database { get; }

        public AuthenticationService(Database database)
        {
            this.Database = database;
        }

        public UserPoco Login(LoginAccountModel loginModel)
        {
            byte[] passwordBytes = Encoding.ASCII.GetBytes(loginModel.Password);

            byte[] result;

            using (var shaM = new SHA512Managed())
            {
                result = shaM.ComputeHash(passwordBytes);
            }

            var parametars = new[]
            {
                new NpgsqlParameter("u", loginModel.Username), new NpgsqlParameter("p", result)
            };

            var account = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE username=@u AND password=@p;", parametars);

            return account;
        }

        public UserPoco GetUserById(int userId)
        {
            var parametar = new NpgsqlParameter("i", userId);

            return this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE user_id=@i;", parametar);
        }

        public LoginSessionsPoco GetSessionBySessionKey(string sessionKey)
        {
            var key = new NpgsqlParameter("k", sessionKey);

            var session =
                this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions WHERE session_key=@k AND deleted=false;", key);

            return session;
        }

        private LoginSessionsPoco GetSessionById(int sessionId)
        {
            var session = this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions WHERE login_sessions_id=@i AND deleted=false", new NpgsqlParameter("i", sessionId));

            return session;
        }

        public string CreateSession(int userId, bool rememberMe)
        {
            string sessionKey = GetRandomSessionKey();

            var now = DateTime.Now;

            var loginSession = new LoginSessionsPoco
            {
                LoginTime = now,
                SessionKey = sessionKey,
                UserId = userId,
            };

            if (!rememberMe)
            {
                loginSession.ExpirationDate = now.Add(TimeSpan.FromMinutes(30));
            }

            this.Database.Insert(loginSession);

            return loginSession.SessionKey;
        }

        /// <summary>
        /// Generates a random 40 length string.
        /// </summary>
        private static string GetRandomSessionKey()
        {
            var builder = new StringBuilder();

            var random = new Random();

            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            int length = 40;

            for (int i = 0; i < length; i++)
            {
                builder.Append(letters[random.Next(length)]);
            }

            return builder.ToString();
        }

        public void Logout(RequestSession requestSession)
        {
            var sessiom = this.GetSessionById(requestSession.SessionId);

            var now = DateTime.Now;

            sessiom.LoggedOut = true;
            sessiom.LoggedOutTime = now;

            this.Database.Update(sessiom);
        }

        public bool CreateAccount(RegisterModel registerModel)
        {
            var poco = new UserPoco
            {
                AvatarUrl = registerModel.AvatarUrl,
                Name = registerModel.Username,
                Password = registerModel.Password
            };

            int result = this.Database.Insert(poco);

            return result > 0;
        }
    }
}
