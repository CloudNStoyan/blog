﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.Areas.Admin.Posts;
using Blog.Web.DAL;
using Npgsql;
using NpgsqlTypes;

namespace Blog.Web.Areas.Admin.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserService
    {
        private Database Database { get; }
        private AuthenticationService AuthenticationService { get; }

        public UserService(Database database, AuthenticationService authenticationService)
        {
            this.Database = database;
            this.AuthenticationService = authenticationService;
        }

        public async Task<UserPoco[]> GetAllUsers()
        {
            var users = await this.Database.Query<UserPoco>(
                "SELECT * FROM users WHERE is_deleted = false;"
                );

            return users.ToArray();
        }

        public async Task<FilteredUsersModel> GetUsers(UserFilter filter)
        {
            filter ??= new UserFilter();

            string orderBy = UserFilter.UserFilterOrderByToSqlColumn(filter.OrderBy);

            string sort = UserFilter.UserFilterSortToSql(filter.Sort);

            string offsetSql = string.Empty;

            var postgresParameters = new List<NpgsqlParameter>();

            if (filter.Offset > 0)
            {
                offsetSql = "OFFSET @offset";
                postgresParameters.Add(new NpgsqlParameter("offset", filter.Offset));
            }

            string limitSql = string.Empty;

            if (filter.Limit > 1)
            {
                limitSql = "LIMIT @limit";
                postgresParameters.Add(new NpgsqlParameter("limit", filter.Limit));
            }

            string sql =
                $"SELECT * FROM users ORDER BY {orderBy} {sort} {offsetSql} {limitSql};";

            var filteredUsers = await this.Database.Query<UserPoco>(sql, postgresParameters.ToArray());

            // We clone the parameters because the original ones belong to the filtered query
            int usersCount = await this.Database.Execute<int>(
                $"SELECT COUNT(*)::int FROM users;",
                postgresParameters.Select(x => x.Clone()).ToArray()
            );

            return new FilteredUsersModel
            {
                Users = filteredUsers.ToArray(),
                Filter = filter,
                UsersCount = usersCount
            };
        }

        /// <summary>
        /// Checks to see if a username is already taken
        /// </summary>
        /// <param name="username">The username you wish to create a user with</param>
        /// <returns>Bool if the username is free</returns>
        public async Task<bool> IsUsernameFree(string username)
        {
            var userPoco = await this.Database.QueryOne<UserPoco>(
                "SELECT * FROM users u WHERE u.username=@username;",
                new NpgsqlParameter("username", username));

            return userPoco == null;
        }

        /// <summary>
        /// Gets user with this <param name="userId">id</param> from the database and returns it
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>UserPoco filled with the data of the user.</returns>
        public async Task<UserPoco> GetUserById(int userId)
        {
            return await this.Database.QueryOne<UserPoco>("SELECT * FROM users u WHERE u.user_id = @userId AND is_deleted = false;",
                new NpgsqlParameter("userId", userId));
        }

        private async Task UpdateUser(UserPoco poco)
        {
            await this.Database.Update(poco);
        }

        public async Task DeleteUser(UserPoco poco)
        {
            poco.IsDeleted = true;

            await this.UpdateUser(poco);

            await this.AuthenticationService.LogoutAllSessionsWithUserId(poco.UserId);
        }

        public async Task CreateUser(string username, string password)
        {
            byte[] hashedPassword = HashPassword(password);

            var poco = new UserPoco
            {
                Name = username,
                Password = hashedPassword
            };

            await this.Database.Insert(poco);
        }

        public async Task ChangePassword(UserPoco userPoco, string newPassword)
        {
            userPoco.Password = HashPassword(newPassword);

            await this.UpdateUser(userPoco);
        }

        private static byte[] HashPassword(string password)
        {
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);

            using var shaM = SHA512.Create();

            return shaM.ComputeHash(passwordBytes);
        }
    }
}
