using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace Blog
{
    public static class Post
    {
        public static void EditPostContent(PostPoco post, string content)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();
                
                var database = new Database(conn);

                post.Content = content;

                database.Update(post);
            }
        }

        public static void EditPostTitle(PostPoco post, string title)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);

                post.Title = title;

                database.Update(post);

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

                var postPoco = new PostPoco {Title = title, Content = content,UserId = Account.Id};

                int postId = database.Insert(postPoco);

                foreach (string tagName in tags)
                {
                    var tag = database.QueryOne<TagPoco>("SELECT * FROM tags WHERE name=@n;", new NpgsqlParameter("n", tagName));

                    if (tag == null)
                    {
                        tag = new TagPoco {Name = tagName};
                        tag.TagId = database.Insert(tag);
                    }

                    int tagId = tag.TagId;

                    var postsTagsPoco = new PostsTagsPoco {PostId = postId, TagId = tagId};

                    database.Insert(postsTagsPoco);
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

                var choosedPost = ChoosePostInterface(posts);
                if (choosedPost != null)
                {
                    Blog.ChoosePost(choosedPost);
                }
            }
        }

        public static void ViewPost(PostPoco post)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);

                string sql = 
                    "SELECT tgs.tag_id AS tag_id,tgs.name AS name FROM posts_tags AS pt INNER JOIN tags AS tgs ON pt.post_id=@i AND pt.tag_id = tgs.tag_id;";

                var parametar = new Dictionary<string,object>
                {
                    {"i", post.PostId}
                };

                var tags = database.Query<TagPoco>(sql,parametar);

                Console.WriteLine("|------------------------------------------------------------------------------------------|");
                Console.WriteLine($"Title: {post.Title.Trim()}");
                Console.WriteLine("|------------------------------------------------------------------------------------------|");
                Console.WriteLine($"Content: {post.Content.Trim()}");
                Console.WriteLine("|------------------------------------------------------------------------------------------|");
                Console.WriteLine($"Tags: {string.Join(", ", tags.Select(t => t.Name)).Trim()}");
                Console.WriteLine("|------------------------------------------------------------------------------------------|");

                Blog.CurrentPost = post;
            }
        }


        public static PostPoco ChoosePostInterface(List<PostPoco> posts)
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


                            return posts[id];
                        }

                        Console.WriteLine("You must type number or 'return' !");
                    }
                }
                else
                {
                    Console.WriteLine("You must type number or 'return' !");
                }
            }

            return null;
        }
    }
}
