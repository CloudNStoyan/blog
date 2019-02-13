using Blog.Web.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
