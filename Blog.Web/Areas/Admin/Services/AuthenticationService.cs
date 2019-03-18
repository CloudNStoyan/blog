﻿using System.Security.Cryptography;
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