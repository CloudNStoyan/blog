using System;
using Npgsql;
using Xunit;

namespace Blog.Tests
{
    public class Tests
    {
        private const string ConnectionString = Blog.ConnectionString;

        [Fact]
        public void LoginTest()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);

                var expectedUser = new UserPoco {UserId = 19, Name = "defaultUser", Password = "defaultPassword"};
                var returnedUser = service.Login(expectedUser.Name, expectedUser.Password);
                Assert.Matches(expectedUser.Name, returnedUser.Name);
                Assert.Matches(expectedUser.Password, returnedUser.Password);
                Assert.Equal(expectedUser.UserId, returnedUser.UserId);
            }
        }

        [Fact]
        public void RegisterTest()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);

                Assert.False(service.UserExist("353Adam"), "User 353Adam exist");
                Assert.False(service.UserExist("353Eva"), "User 353Eva exist");
                Assert.False(service.UserExist("353Ervin"), "User 353Ervin exist");
                Assert.False(service.UserExist("353Daham"), "User 353Daham exist");
                var adam = service.RegisterUser("353Adam", "password2");
                var eva = service.RegisterUser("353Eva", "password2");
                var ervin = service.RegisterUser("353Ervin", "password2");
                var daham = service.RegisterUser("353Daham", "password2");
                Assert.True(service.UserExist(adam.Name), "User 353Adam doesn't exist");
                Assert.True(service.UserExist(eva.Name), "User 353Eva doesn't exist");
                Assert.True(service.UserExist(ervin.Name), "User 353Ervin doesn't exist");
                Assert.True(service.UserExist(daham.Name), "User 353Daham doesn't exist");

                database.Delete(adam);
                database.Delete(eva);
                database.Delete(ervin);
                database.Delete(daham);
            }
        }


    }
}
