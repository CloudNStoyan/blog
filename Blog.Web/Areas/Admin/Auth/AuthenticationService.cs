using System;
using System.Security.Cryptography;
using System.Text;
using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web.Areas.Admin.Auth
{
    /// <summary>
    /// Handles all the authentication calls with the database
    /// </summary>
    public class AuthenticationService
    {
        private Database Database { get; }

        public AuthenticationService(Database database)
        {
            this.Database = database;
        }

        /// <summary>
        /// Signs in the user using login account
        /// </summary>
        /// <param name="loginModel">The user account</param>
        /// <returns>Userpoco filled with the account data from the database.</returns>
        public UserPoco Login(LoginDataModel loginModel)
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

        /// <summary>
        /// Gets user with this <param name="userId">id</param> from the database and returns it
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>UserPoco filled with the data of the user.</returns>
        public UserPoco GetUserById(int userId)
        {
            var parametar = new NpgsqlParameter("i", userId);

            return this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE user_id=@i;", parametar);
        }

        /// <summary>
        /// Gets session with this <param name="sessionKey">session key</param> from the database and returns it
        /// </summary>
        /// <param name="sessionKey">The session key</param>
        /// <returns></returns>
        public LoginSessionsPoco GetSessionBySessionKey(string sessionKey)
        {
            var key = new NpgsqlParameter("k", sessionKey);

            var session =
                this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions WHERE session_key=@k AND logged_out=false;", key);

            return session;
        }

        /// <summary>
        /// Gets session with this <param name="sessionId">session id</param> from the database and returns it
        /// </summary>
        /// <param name="sessionId">The session id</param>
        /// <returns>The LoginSessionsPoco from the database</returns>
        private LoginSessionsPoco GetSessionById(int sessionId)
        {
            var session = this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions WHERE login_sessions_id=@i AND logged_out=false", new NpgsqlParameter("i", sessionId));

            return session;
        }

        /// <summary>
        /// Creates session with userId and rememberMe boolean.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="rememberMe">If the session will be pernament or will expire</param>
        /// <returns>The session key</returns>
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

        /// <summary>
        /// Log outs the session
        /// </summary>
        /// <param name="requestSession">The session to be logged out</param>
        public void Logout(RequestSession requestSession)
        {
            var sessiom = this.GetSessionById(requestSession.SessionId);

            var now = DateTime.Now;

            sessiom.LoggedOut = true;
            sessiom.LoggedOutTime = now;

            this.Database.Update(sessiom);
        }

        /// <summary>
        /// Creates account in the databse
        /// </summary>
        /// <param name="registerDataModel">The registration info</param>
        /// <returns>If the account was sucessfully created</returns>
        public bool CreateAccount(RegisterDataModel registerDataModel)
        {
            var poco = new UserPoco
            {
                AvatarUrl = registerDataModel.AvatarUrl,
                Name = registerDataModel.Username,
                Password = registerDataModel.Password
            };

            int result = this.Database.Insert(poco);

            return result > 0;
        }
    }
}
