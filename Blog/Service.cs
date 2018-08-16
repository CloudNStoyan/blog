using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    public class Service
    {
        private Database Database { get; }

        public Service(Database database)
        {
            this.Database = database;
        }

        public void RegisterUser(string name, string password)
        {
            var userPoco = new UserPoco {Name = name, Password = password};
            this.Database.Insert(userPoco);
        }

        public UserPoco Login(string name, string password)
        {
            var parametars = new Dictionary<string, object>
            {
                {"n", name},
                {"p", password}
            };

            var user = this.Database.QueryOne<UserPoco>("SELECT * FROM users WHERE name=@n AND password=@p;", parametars);

            return user;
        }

        public void CreateComment(string authorName, string content, int postId, int userId)
        {
            var commentPoco = new CommentPoco
            {
                AuthorName = authorName,
                Content = content,
                PostId = postId,
                UserId = userId
            };

            this.Database.Update(commentPoco);
        }

        public void Rename(int id,string password,string newName)
        {
            Console.WriteLine($"Id:{id}|Password:{password}|Name:{newName}");
            var user = new UserPoco {Name = newName,Password = password, UserId = id};
            this.Database.Update(user);
        }

        public void ChangePassword(int id, string newPassword, string name)
        {
            var user = new UserPoco { Name = name, Password = newPassword, UserId = id };
            this.Database.Update(user);
        }

        public List<PostPoco> GetAllPosts()
        {
            return this.Database.Query<PostPoco>("SELECT * FROM posts;");
        }

        public List<PostPoco> GetAllUsersPosts(UserPoco user)
        {
            return this.Database.Query<PostPoco>("SELECT * FROM posts WHERE user_id=@i",
                new NpgsqlParameter("i", user.UserId));
        }

        public bool UserExist(string name)
        {
            var parametar = new NpgsqlParameter("n", name);
            return this.Database.Query<UserPoco>("SELECT * FROM users WHERE name=@n;", parametar).Count > 0;
        }
    }
}
