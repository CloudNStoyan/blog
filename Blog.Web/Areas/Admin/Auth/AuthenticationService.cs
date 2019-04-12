using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<UserPoco> Login(LoginDataModel loginModel)
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

            var account = await this.Database.QueryOne<UserPoco>(
                "SELECT * FROM users u WHERE u.username=@username AND u.password=@password;", 
                parametars
            );

            return account;
        }

        /// <summary>
        /// Gets user with this <param name="userId">id</param> from the database and returns it
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>UserPoco filled with the data of the user.</returns>
        public async Task<UserPoco> GetUserById(int userId)
        {
            return await this.Database.QueryOne<UserPoco>("SELECT * FROM users u WHERE u.user_id = @userId;", 
                new NpgsqlParameter("userId", userId));
        }

        /// <summary>
        /// Gets session with this <param name="sessionKey">session key</param> from the database and returns it
        /// </summary>
        /// <param name="sessionKey">The session key</param>
        /// <returns></returns>
        public async Task<LoginSessionsPoco> GetSessionBySessionKey(string sessionKey)
        {
            var session = await this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions ls WHERE ls.session_key = @sessionKey;", 
                    new NpgsqlParameter("sessionKey", sessionKey));

            return session;
        }

        /// <summary>
        /// Gets session with this <param name="sessionId">session id</param> from the database and returns it
        /// </summary>
        private async Task<LoginSessionsPoco> GetSessionById(int sessionId)
        {
            var session = await this.Database.QueryOne<LoginSessionsPoco>(
                    "SELECT * FROM login_sessions ls WHERE ls.login_sessions_id=@sessionId;", 
                    new NpgsqlParameter("sessionId", sessionId));

            return session;
        }

        /// <summary>
        /// Returns the session key of the session.
        /// </summary>
        public async Task<string> CreateNewSession(int userId, bool rememberMe)
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

            await this.Database.Insert(loginSession);

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
        /// Logs out the session.
        /// </summary>
        public async void Logout(RequestSession requestSession)
        {
            var sessiom = await this.GetSessionById(requestSession.SessionId);

            var now = DateTime.Now;

            sessiom.LoggedOut = true;
            sessiom.LoggedOutTime = now;

            this.Database.Update(sessiom);
        }
    }
}
