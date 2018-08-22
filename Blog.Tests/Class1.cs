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

                var expectedUser = new UserPoco {UserId = 19, Name = "defaultuser", Password = "defaultPassword"};
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

                Assert.False(service.UserExist("353adam"), "User 353Adam exist");
                Assert.False(service.UserExist("353eva"), "User 353Eva exist");
                Assert.False(service.UserExist("353ervin"), "User 353Ervin exist");
                Assert.False(service.UserExist("353daham"), "User 353Daham exist");
                var adam = service.RegisterUser("353adam", "password2");
                var eva = service.RegisterUser("353eva", "password2");
                var ervin = service.RegisterUser("353ervin", "password2");
                var daham = service.RegisterUser("353daham", "password2");
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

        [Fact]
        public void CreatePostTest()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);

                var postId = service.CreatePost("Test Title", "Test content", new[] {"new", "tag"});
                var post = database.QueryOne<PostPoco>("SELECT * FROM posts WHERE post_id=@i",
                    new NpgsqlParameter("i", postId));

                Assert.NotNull(post);

                service.DeletePost(post);

                post = database.QueryOne<PostPoco>("SELECT * FROM posts WHERE post_id=@i",
                    new NpgsqlParameter("i", postId));

                Assert.Null(post);
            }
        }

        [Fact]
        public void ChangePasswordTest()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                service.ChangePassword(19,"dwarf", "defaultuser");
                var userPoco = service.Login("defaultuser", "dwarf");
                Assert.NotNull(userPoco);
                service.ChangePassword(19,"defaultPassword","defaultuser");
                userPoco = service.Login("defaultuser", "defaultPassword");
                Assert.NotNull(userPoco);
            }
        }

        [Fact]
        public void RenameTest()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                service.Rename(19,"defaultPassword","undefaultuser");
                var userPoco = service.Login("undefaultuser", "defaultPassword");
                Assert.NotNull(userPoco);
                service.Rename(19,"defaultPassword", "defaultuser");
                userPoco = service.Login("defaultuser", "defaultPassword");
                Assert.NotNull(userPoco);
            }
        }

        [Fact]
        public void AllPostsTest()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                var posts = service.GetAllPosts();
                Assert.True(posts.Count > 1);
            }
        }

        [Fact]
        public void AllusersCommentsTest()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                var comments =
                    service.GetUserComments(new UserPoco
                    {
                        Name = "defaultuser",
                        Password = "defaultPassword",
                        UserId = 19
                    });

                Assert.True(comments.Count > 1);
            }
        }
    }
}
