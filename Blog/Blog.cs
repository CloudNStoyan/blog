using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    
    public static class Blog
    {
        public static void ShowAllCommands()
        {
            var buildAllCommands = new StringBuilder();
            buildAllCommands.AppendLine("<--All Commands-->");
            buildAllCommands.AppendLine("# help-post");
            buildAllCommands.AppendLine("* This command gives you information how to post a new post!");
            buildAllCommands.AppendLine("# help-comment");
            buildAllCommands.AppendLine("* This command gives you information how to comment on a post!");
            buildAllCommands.AppendLine("# help-view");
            buildAllCommands.AppendLine("* This command gives you information how to view material on the blog!");
            buildAllCommands.AppendLine("<--All Commands-->");
            Console.WriteLine(buildAllCommands.ToString().Trim());
        }

        public static void ShowPostCommands()
        {
            var buildPostCommands = new StringBuilder();
            buildPostCommands.AppendLine("<--Post commands-->");
            buildPostCommands.AppendLine("# post-create");
            buildPostCommands.AppendLine("* This command walks you through a procces for making a new post!");
            buildPostCommands.AppendLine("# post-edit");
            buildPostCommands.AppendLine("* This command walks you through a procces for editing an existing post!");
            buildPostCommands.AppendLine("<--Post commands-->");
            Console.WriteLine(buildPostCommands.ToString().Trim());
        }

        public static void ShowCommentCommands()
        {
            var buildCommentCommands = new StringBuilder();
            buildCommentCommands.AppendLine("<--Comment commands-->");
            buildCommentCommands.AppendLine("# comment-create");
            buildCommentCommands.AppendLine("* This command walks you through a procces for commenting on a existing post!");
            buildCommentCommands.AppendLine("# comment-edit");
            buildCommentCommands.AppendLine("* This command walks you through a procces for editing an existing comment!");
            buildCommentCommands.AppendLine("<--Comment commands-->");
            Console.WriteLine(buildCommentCommands.ToString().Trim());
        }

        public static void ShowViewCommands()
        {

        }

        public const string ConnectionPath = @"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
        public static int CurrentPost;

        public static void CreatePost(string input) //string title, string content,string[] tags)
        {
            string title = input.Split(':')[0].Remove(0, 5);
            string content = input.Split(':')[1];
            string[] tags = input.Split(':')[2].Split(',');

            var ids = new List<int>();
            // post Some random Title:Thiis a very informative post:guides,lol,stuff,memes
            using (var conn = new NpgsqlConnection(ConnectionPath))
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
                                    ids.Add(int.Parse(rdr["tag_id"].ToString()));
                                }
                            }
                        }
                    }
                    else
                    {
                        ids.Add(int.Parse(buildId));
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
                            postId = int.Parse(rdr["post_id"].ToString());
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

        public static void CreateComment(string input)
        {
            string[] splitedInput = input.Split(':');
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO comments (author_name,content,post_id) VALUES (@a,@c,@p)", conn))
                {
                    cmd.Parameters.AddWithValue("a", Account.Name);
                    cmd.Parameters.AddWithValue("c", splitedInput[1]);
                    cmd.Parameters.AddWithValue("p", CurrentPost);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                conn.Dispose();
            }
        }

        public static void EditComment(string input)
        {
            string[] splitedInput = input.Split(':');
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                int commentId = 0;
                using (var cmd = new NpgsqlCommand("SELECT * FROM comments WHERE author_name=@n AND post_id=@i",conn))
                {
                    cmd.Parameters.AddWithValue("n", Account.Name);
                    cmd.Parameters.AddWithValue("i", CurrentPost);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            commentId = int.Parse(rdr["comment_id"].ToString());
                            Console.WriteLine(commentId);
                        }
                    }
                }
                using (var cmd = new NpgsqlCommand("UPDATE comments SET content=@c WHERE comment_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("c", splitedInput[1]);
                    cmd.Parameters.AddWithValue("i", commentId);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                conn.Dispose();
            }
        }

        public static void ViewPost(string input)
        {
            int id = int.Parse(input.Split(' ')[1]);
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM posts WHERE post_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("i", id);
                    var rdr = cmd.ExecuteReader();
                    var build = new StringBuilder();

                    while (rdr.Read())
                    {
                        build.AppendLine($"{rdr["title"]}\n{rdr["content"]}");
                    }

                    CurrentPost = id;
                    Console.WriteLine(build);
                }

                conn.Dispose();
            }
        }

        public static void EditPostContent(string input)
        {
            string newContent = input.Split(':')[1];
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"UPDATE posts SET content=@c WHERE post_id=@p",conn))
                {
                    cmd.Parameters.AddWithValue("c", newContent);
                    cmd.Parameters.AddWithValue("p", int.Parse(input.Split(':')[2]));
                    cmd.ExecuteNonQuery();
                }

                conn.Dispose();
            }
        }

        public static void EditPostTitle(string input)
        {
            string newTitle = input.Split(':')[1];
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"UPDATE posts SET title=@t WHERE post_id=@p",conn))
                {
                    cmd.Parameters.AddWithValue("t", newTitle);
                    cmd.Parameters.AddWithValue("p", int.Parse(input.Split(':')[2]));
                    cmd.ExecuteNonQuery();
                }

                conn.Dispose();
            }
        }

        public static void ShowLatests()
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM posts LIMIT 10", conn))
                {
                    var rdr = cmd.ExecuteReader();
                    var buildOutput = new StringBuilder();
                    while (rdr.Read())
                    {
                        buildOutput.AppendLine($"{rdr["title"]}");
                        buildOutput.AppendLine($"{rdr["content"]}");
                    }

                    Console.WriteLine(buildOutput);
                    cmd.Dispose();
                }

                conn.Dispose();
            }
        }

        public static void ShowAccountInformation()
        {
            Console.WriteLine($"You are currently logged as: {Account.Name}");
            Console.WriteLine($"With {Account.PostsCount} posts");
        }
    }
}
