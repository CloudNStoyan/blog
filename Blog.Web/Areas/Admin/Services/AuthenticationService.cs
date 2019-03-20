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
        private Database Database { get; set; }

        public AuthenticationService(Database database)
        {
            this.Database = database;
        }

        public UserPoco ConfirmAccount(LoginAccountModel loginModel)
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

            var account = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE username=@u AND password=@p", parametars);

            return account;

        }

        public UserPoco GetAccountPoco(int id)
        {
            var parametar = new NpgsqlParameter("i", id);
            return this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE user_id=@i", parametar);
        }

        public LoginSessionsPoco RequestSession(string sessionKey)
        {
            var parametar = new NpgsqlParameter("k", sessionKey);

            var session =
                this.Database.QueryOne<LoginSessionsPoco>("SELECT * FROM login_sessions WHERE session_key=@k ",
                    parametar);

            return session;
        }

        public string MakeSession(int userId)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var builder = new StringBuilder();
            var random = new Random();
            int length = 40;

            for (int i = 0; i < length; i++)
            {
                builder.Append(chars[random.Next(length)]);
            }


            var loginSession = new LoginSessionsPoco()
            {
                LoginTime = DateTime.Now,
                SessionKey = builder.ToString(),
                UserId = userId
            };

            int index = this.Database.Insert(loginSession);

            var parametar = new NpgsqlParameter("i", index);

            var session =
                this.Database.QueryOne<LoginSessionsPoco>("SELECT * FROM login_sessions WHERE login_sessions_id=@i", parametar);

            return session.SessionKey;
        }

        public bool CreateAccount(RegisterModel registerModel)
        {

            var poco = new UserPoco()
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
