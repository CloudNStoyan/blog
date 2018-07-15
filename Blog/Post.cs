using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    public class Post
    {
        public static void EditPostContent(int postId, string content)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"UPDATE posts SET content=@c WHERE post_id=@p", conn))
                {
                    cmd.Parameters.AddWithValue("c", content);
                    cmd.Parameters.AddWithValue("p", postId);
                    cmd.ExecuteNonQuery();
                }

                conn.Dispose();
            }
        }

        public static void EditPostTitle(int postId, string title)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"UPDATE posts SET title=@t WHERE post_id=@p", conn))
                {
                    cmd.Parameters.AddWithValue("t", title);
                    cmd.Parameters.AddWithValue("p", postId);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("You succesfully edited this post title!");

                conn.Dispose();
            }
        }

        public static void BeginCreatingPost()
        {
            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Content(Type done when you are done!): ");
            var buildContent = new StringBuilder();
            while (true)
            {
                string line = Console.ReadLine();
                if (line.ToLowerInvariant().Trim() == "done")
                {
                    break;
                }

                buildContent.AppendLine(line);
            }

            string content = buildContent.ToString();
            Console.Write("Tags(Split tags by ',')!: ");
            string[] tags = Console.ReadLine().Split(',');
            CreatePost(title, content, tags);
            Console.WriteLine("Done! if you want to view your post type view-latest");

        }

        public static void CreatePost(string title, string content, string[] tags)
        {
            var ids = new List<int>();

            using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO posts (title,content,user_id) VALUES (@t,@c,@u)", conn))
                {
                    cmd.Parameters.AddWithValue("t", title);
                    cmd.Parameters.AddWithValue("c", content);
                    cmd.Parameters.AddWithValue("u", Account.Id);
                    cmd.ExecuteNonQuery();
                }


                var build = new StringBuilder();
                string buildId = String.Empty;

                foreach (string tag in tags)
                {
                    using (var cmd = new NpgsqlCommand("SELECT * FROM tags WHERE name=@n", conn))
                    {
                        cmd.Parameters.AddWithValue("n", tag);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                build.AppendLine($"{rdr["name"]}");
                                buildId = $"{rdr["tag_id"]}";
                            }
                        }
                    }

                    if (build.ToString().Length == 0)
                    {
                        using (var cmd = new NpgsqlCommand("INSERT INTO tags (name) VALUES (@n)", conn))
                        {
                            cmd.Parameters.AddWithValue("n", tag);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new NpgsqlCommand("SELECT * FROM tags WHERE name=@t", conn))
                        {
                            cmd.Parameters.AddWithValue("t", tag);
                            using (var rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    ids.Add(Int32.Parse(rdr["tag_id"].ToString()));
                                }
                            }
                        }
                    }
                    else
                    {
                        ids.Add(Int32.Parse(buildId));
                    }
                }


                int postId = 0;
                using (var cmd = new NpgsqlCommand("SELECT * FROM posts WHERE title=@t", conn))
                {
                    cmd.Parameters.AddWithValue("t", title);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            postId = Int32.Parse(rdr["post_id"].ToString());
                        }
                    }
                }


                foreach (int id in ids)
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO posts_tags (post_id,tag_id) VALUES (@p,@t)", conn))
                    {
                        cmd.Parameters.AddWithValue("p", postId);
                        cmd.Parameters.AddWithValue("t", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                conn.Dispose();
            }

        }


        public static void ViewPost(int id)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
            {
                conn.Open();
                StringBuilder build;

                int userId = 0;

                using (var cmd = new NpgsqlCommand("SELECT * FROM posts WHERE post_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("i", id);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        build = new StringBuilder();

                        while (rdr.Read())
                        {
                            userId = Int32.Parse($"{rdr["user_id"]}");
                            build.AppendLine($"Title: {rdr["title"]}\n");
                            build.AppendLine($"Content: {rdr["content"]}\n");
                        }
                    }
                }

                var tagIds = new List<int>();

                using (var cmd = new NpgsqlCommand("SELECT * FROM posts_tags WHERE post_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("i", id);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            tagIds.Add(Int32.Parse(rdr["tag_id"].ToString()));
                        }
                    }
                }

                foreach (int tagId in tagIds)
                {
                    using (var cmd = new NpgsqlCommand("SELECT * FROM tags WHERE tag_id=@i", conn))
                    {
                        cmd.Parameters.AddWithValue("i", tagId);

                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                build.Append($"#{rdr["name"]} ");
                            }
                        }
                    }
                }

                Console.WriteLine(build);
                Blog.CurrentPost = id;
                Blog.CurrentPostUserId = userId;


                conn.Dispose();
            }
        }
    }
}
