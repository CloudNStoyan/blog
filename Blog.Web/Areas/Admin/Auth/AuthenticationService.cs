using System;
using System.Security.Cryptography;
using System.Text;
using Blog.Web.DAL;     
using Npgsql;

namespace Blog.Web.Areas.Admin.Auth
{
    /// <summary>
    /// Handles all the authentication calls with the database.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AuthenticationService
    {
        private Database Database { get; }

        public AuthenticationService(Database database)
        {
            this.Database = database;
        }

        /// <summary>
        /// Signs in the user using login account.
        /// </summary>
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
                new NpgsqlParameter("username", loginModel.Username),
                new NpgsqlParameter("password", result)
            };

            var account = this.Database.QueryOne<UserPoco>(
                "SELECT * FROM users WHERE username=@username AND password=@password;", 
                parametars
            );

            return account;
        }

        /// <summary>
        /// Gets user with this <param name="userId">id</param> from the database and returns it
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>UserPoco filled with the data of the user.</returns>
        public UserPoco GetUserById(int userId)
        {
            return this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE user_id=@userId;", 
                new NpgsqlParameter("userId", userId));
        }

        /// <summary>
        /// Gets session with this <param name="sessionKey">session key</param> from the database and returns it
        /// </summary>
        /// <param name="sessionKey">The session key</param>
        /// <returns></returns>
        public LoginSessionsPoco GetSessionBySessionKey(string sessionKey)
        {
            var session = this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions WHERE session_key=@sessionKey;", 
                    new NpgsqlParameter("sessionKey", sessionKey));

            return session;
        }

        /// <summary>
        /// Gets session with this <param name="sessionId">session id</param> from the database and returns it
        /// </summary>
        private LoginSessionsPoco GetSessionById(int sessionId)
        {
            var session = this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions ls WHERE ls.login_sessions_id=@sessionId;", 
                    new NpgsqlParameter("sessionId", sessionId));

            return session;
        }

        /// <summary>
        /// Returns the session key of the session.
        /// </summary>
        public string CreateNewSession(int userId, bool rememberMe)
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
        /// Log outs the session.
        /// </summary>
        public void Logout(RequestSession requestSession)
        {
            var sessiom = this.GetSessionById(requestSession.SessionId);

            var now = DateTime.Now;

            sessiom.LoggedOut = true;
            sessiom.LoggedOutTime = now;

            this.Database.Update(sessiom);
        }
    }
}
