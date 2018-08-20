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

        public UserPoco RegisterUser(string name, string password)
        {
            var userPoco = new UserPoco {Name = name, Password = password};
            int userPocoId = this.Database.Insert(userPoco);
            userPoco.UserId = userPocoId;

            return userPoco;
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

        public void UpdateComment(CommentPoco poco)
        {
            this.Database.Update(poco);
        }

        public void DeletePost(PostPoco poco)
        {
            var parametar = new Dictionary<string, object>() { { "i", poco.PostId } };

            var comments = this.Database.Query<CommentPoco>("SELECT * FROM comments WHERE post_id=@i;", parametar);
            var postsTags = this.Database.Query<PostsTagsPoco>("SELECT * FROM posts_tags WHERE post_id=@i;", parametar);

            foreach (var commentPoco in comments)
            {
                this.Database.Delete(commentPoco);
            }

            foreach (var postsTagsPoco in postsTags)
            {
                this.Database.Delete(postsTagsPoco);
            }

            this.Database.Delete(poco);
        }

        public List<CommentPoco> GetUserComments(UserPoco poco)
        {
           return this.Database.Query<CommentPoco>("SELECT * FROM comments WHERE user_id=@i;", new NpgsqlParameter("i", poco.UserId));
        }

        public List<CommentPoco> GetPostsAllCommentars(PostPoco poco)
        {
            return this.Database.Query<CommentPoco>("SELECT * FROM comments WHERE post_id=@i;", new NpgsqlParameter("i", poco.PostId));
        }

        public List<PostPoco> GetNewestsPosts()
        {
            return this.Database.Query<PostPoco>("SELECT * FROM posts ORDER BY post_id LIMIT 10;");
        }

        public void UpdatePostContent(PostPoco post,string content)
        {
            post.Content = content;
            this.Database.Update(post);
        }

        public void UpdatePostTitle(PostPoco post, string title)
        {
            post.Title = title;
            this.Database.Update(post);
        }

        public List<TagPoco> GetPostTags(PostPoco poco)
        {
            string sql =
                "SELECT tgs.tag_id AS tag_id,tgs.name AS name FROM posts_tags AS pt INNER JOIN tags AS tgs ON pt.post_id=@i AND pt.tag_id = tgs.tag_id;";

            var parametar = new Dictionary<string, object>
            {
                {"i", poco.PostId}
            };

            return this.Database.Query<TagPoco>(sql, parametar);
        }

        public int CreatePost(string title, string content, string[] tags)
        {
            var postPoco = new PostPoco { Title = title, Content = content, UserId = Account.Id };

            int postId = this.Database.Insert(postPoco);

            foreach (string tagName in tags)
            {
                var tag = this.Database.QueryOne<TagPoco>("SELECT * FROM tags WHERE name=@n;", new NpgsqlParameter("n", tagName));

                if (tag == null)
                {
                    tag = new TagPoco { Name = tagName };
                    tag.TagId = this.Database.Insert(tag);
                }

                int tagId = tag.TagId;

                var postsTagsPoco = new PostsTagsPoco { PostId = postId, TagId = tagId };

                this.Database.Insert(postsTagsPoco);
            }

            return postId;
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
