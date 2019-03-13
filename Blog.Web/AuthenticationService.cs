using Blog.Web.DAL;
using Blog.Web.Models;
using Npgsql;

namespace Blog.Web
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
            var parametars = new[]
            {
                new NpgsqlParameter("u", loginModel.Username), new NpgsqlParameter("p", loginModel.Password)

            };

            var account = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE username=@u AND password=@p", parametars);

            return account;

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
