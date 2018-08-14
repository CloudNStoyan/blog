using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;

namespace Blog
{
    public class Post
    {
        public static void EditPostContent(int postId, string content)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();
                
                var database = new Database(conn);

                var parametars = new Dictionary<string,object>
                {
                    {"c", content},
                    {"i", postId}
                };

                database.ExecuteNonQuery("UPDATE posts SET content=@c WHERE post_id=@i;", parametars);
            }
        }

        public static void EditPostTitle(int postId, string title)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                var parametars = new Dictionary<string, object>
                {
                    {"t", title},
                    {"i", postId}
                };

                database.ExecuteNonQuery("UPDATE posts SET title=@t WHERE post_id=@i;", parametars);

                Console.WriteLine("You succesfully edited this post title!");
            }
        }

        public static void BeginCreatingPost()
        {
            Console.Write("Title: ");
            string title = Console.ReadLine();

            Console.Write("Content(Type done when you are done!): ");
            string content = string.Empty;

            bool creatingContent = true;

            while (creatingContent)
            {
                string line = Console.ReadLine();

                if (line?.ToLowerInvariant().Trim() != "done")
                {
                    content += line;
                }
                else
                {
                    creatingContent = false;
                }
            }

            Console.Write("Tags(Split tags by ',')!: ");

            string[] tags = Console.ReadLine()?.Split(',');
            CreatePost(title, content, tags);

            Console.WriteLine("Done! if you want to view your post type new-posts");
        }

        private static void CreatePost(string title, string content, string[] tags)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                var parametars = new Dictionary<string,object>
                {
                    {"t", title},
                    {"c", content},
                    {"u", Account.Id}
                };

                int postId = database.Execute<int>("INSERT INTO posts (title,content,user_id) VALUES (@t,@c,@u) RETURNING post_id;", parametars);

                foreach (string tagName in tags)
                {
                    var tag = database.QueryOne<TagPoco>("SELECT * FROM tags WHERE name=@n;", new NpgsqlParameter("n", tagName));

                    int id = tag?.TagId ?? database.Execute<int>("INSERT INTO tags (name) VALUES (@n) RETURNING tag_id;", new NpgsqlParameter("n", tagName));

                    parametars = new Dictionary<string,object>
                    {
                        {"p", postId},
                        {"t", id}
                    };

                    database.ExecuteNonQuery("INSERT INTO posts_tags (post_id,tag_id) VALUES (@p,@t);", parametars);
                }
            }

        }

        public static void ViewAllPosts()
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();
                var database = new Database(conn);
                var posts = database.Query<PostPoco>("SELECT * FROM posts;");

                Blog.ChoosePost(ChoosePostInterface(posts));
            }
        }

        public static void ViewPost(int id)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                var post = database.QueryOne<PostPoco>("SELECT * FROM posts WHERE post_id=@i;", new NpgsqlParameter("i", id));
                var postsTagsConnections = database.Query<PostsTagsPoco>("SELECT * FROM posts_tags WHERE post_id=@i;", new NpgsqlParameter("i", post.PostId));

                var tags = new List<TagPoco>();
                foreach (var tagsConnection in postsTagsConnections)
                {
                    tags.Add(database.QueryOne<TagPoco>("SELECT * FROM tags WHERE tag_id=@i", new NpgsqlParameter("i", tagsConnection.TagId)));
                }

                Console.WriteLine("|------------------------------------------------------------------------------------------|");
                Console.WriteLine($"Title: {post.Title.Trim()}");
                Console.WriteLine("|------------------------------------------------------------------------------------------|");
                Console.WriteLine($"Content: {post.Content.Trim()}");
                Console.WriteLine("|------------------------------------------------------------------------------------------|");
                Console.WriteLine($"Tags: {string.Join(", ", tags.Select(t => t.Name)).Trim()}");
                Console.WriteLine("|------------------------------------------------------------------------------------------|");

                Blog.CurrentPost = post.PostId;
                Blog.CurrentPostUserId = post.UserId;
            }
        }


        public static int ChoosePostInterface(List<PostPoco> posts)
        {
            Console.WriteLine("|Number|" + "Title".PadLeft(13,' ').PadRight(27,' ') + "|");

            for (int i = 0; i < posts.Count; i++)
            {
                string postTitle = posts[i].Title.Length > 23 ? posts[i].Title.Substring(0, 23) + "..." : posts[i].Title;
                Console.WriteLine($"|{i + 1,5} | {postTitle.PadRight(26,' ')}|");
            }

            bool postIsNotChoosed = true;

            while (postIsNotChoosed)
            {
                Console.Write("Post number: ");
                string line = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (!string.IsNullOrEmpty(line))
                {
                    if (line == "return")
                    {
                        postIsNotChoosed = false;
                    }
                    else
                    {
                        bool lineIsNumber = int.TryParse(line, out int id);
                        id--;

                        if (lineIsNumber)
                        {
                            if (id < 0 || id > posts.Count - 1)
                            {
                                Console.WriteLine($"Invalid number. Number must be in range of 1 to {posts.Count}");
                            }


                            return posts[id].PostId;
                        }

                        Console.WriteLine("You must type number or 'return' !");
                    }
                }
                else
                {
                    Console.WriteLine("You must type number or 'return' !");
                }
            }

            return -1;
        }
    }
}
