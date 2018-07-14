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
            buildAllCommands.AppendLine("* This command gives you information on how to post a new post!");
            buildAllCommands.AppendLine("# help-comment");
            buildAllCommands.AppendLine("* This command gives you information on how to comment on a post!");
            buildAllCommands.AppendLine("# help-view");
            buildAllCommands.AppendLine("* This command gives you information on how to view material on the blog!");
            buildAllCommands.AppendLine("# help-account");
            buildAllCommands.AppendLine("* This command gives you information on what profile commands you have!");
            buildAllCommands.AppendLine("<--All Commands-->");
            Console.WriteLine(buildAllCommands.ToString().Trim());
        }

        public static void ShowPostCommands()
        {
            var buildPostCommands = new StringBuilder();
            buildPostCommands.AppendLine("<--Post commands-->");
            buildPostCommands.AppendLine("# post-create");
            buildPostCommands.AppendLine("* This command walks you through a procces for making a new post!");
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

        public static void ShowAccountCommands()
        {
            var buildAccountCommands = new StringBuilder();
            buildAccountCommands.AppendLine("<--Account commands-->");
            buildAccountCommands.AppendLine("# my-posts");
            buildAccountCommands.AppendLine("* This command let you choose which of your posts you want to view!");
            buildAccountCommands.AppendLine("<--Account commands-->");
            Console.WriteLine(buildAccountCommands.ToString().Trim());
        }

        public static void ShowEditCommands()
        {
            var buildEditCommands = new StringBuilder();
            buildEditCommands.AppendLine("<--Edit commands-->");
            buildEditCommands.AppendLine("# edit-title");
            buildEditCommands.AppendLine("* This command is giving you the opportunity to edit the title of the post!");
            buildEditCommands.AppendLine("# edit-content");
            buildEditCommands.AppendLine("* This command is giving you the opportunity to edit the content of the post!");
            buildEditCommands.AppendLine("<--Edit commands-->");
            Console.WriteLine(buildEditCommands.ToString().Trim());
        }

        public const string ConnectionPath = @"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
        public static int CurrentPost;
        public static int CurrentPostUserId;

        public static void ViewAccountPosts()
        {
            var postIds = new List<int>();
            var postTitles = new List<string>();

            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM posts WHERE user_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("i", Account.Id);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            postIds.Add(int.Parse(rdr["post_id"].ToString()));
                            postTitles.Add(rdr["title"].ToString());
                        }
                    }
                }

                for (int i = 0; i < postIds.Count; i++)
                {
                    Console.WriteLine($"{i + 1}| {postTitles[i]}");
                }

                Console.WriteLine("Choose which post to view by entering his number! type return to return to main menu!");
                while (true)
                {
                    Console.Write("Post number: ");
                    string line = Console.ReadLine();
                    if (line.ToLowerInvariant().Trim() == "done")
                    {
                        break;
                    }

                    int id = 0;

                    try
                    {
                        id = int.Parse(line.Trim()) - 1;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("You need to type number!");
                    }

                    ChoosePost(postIds[id]);

                }
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
            CreatePost(title,content,tags);
            Console.WriteLine("Done! if you want to view your post type view-latest");

        }

        public static void CreatePost(string title,string content,string[] tags)
        {
            var ids = new List<int>();

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

        public static void CreateCommentInterface()
        {
            Console.Write("Your comment(Type 'done' when you are done!): ");
            var buildComment = new StringBuilder();
            while (true)
            {
                string line = Console.ReadLine();
                if (line.ToLowerInvariant().Trim() == "done")
                {
                    break;
                }

                buildComment.AppendLine(line);
            }

            string comment = buildComment.ToString().Trim();

            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO comments (author_name,content,post_id) VALUES (@a,@c,@p)", conn))
                {
                    cmd.Parameters.AddWithValue("a", Account.Name);
                    cmd.Parameters.AddWithValue("c", comment);
                    cmd.Parameters.AddWithValue("p", CurrentPost);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                conn.Dispose();
            }

            Console.WriteLine("You successfully commented!");
        }

        public static void EditComment()
        {
            Console.Write("Edit comment(Type 'done' when you are done):");

            var buildNewComment = new StringBuilder();
            while (true)
            {
                string line = Console.ReadLine();
                if (line.ToLowerInvariant().Trim() == "done")
                {
                    break;
                }

                buildNewComment.AppendLine(line);
            }

            string newComment = buildNewComment.ToString();

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
                    cmd.Parameters.AddWithValue("c", newComment);
                    cmd.Parameters.AddWithValue("i", commentId);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                conn.Dispose();
            }
        }

        public static void ViewPost(int id)
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
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
                            userId = int.Parse($"{rdr["user_id"]}");
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
                            tagIds.Add(int.Parse(rdr["tag_id"].ToString()));
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
                CurrentPost = id;
                CurrentPostUserId = userId;


                conn.Dispose();
            }
        }

        public static void ChoosePost(int id)
        {
            ViewPost(id);
            PostInterface(CurrentPostUserId);
        }

        public static void PostInterface(int userId)
        {
            if (Account.Id == userId)
            {
                var buildDisclaimer = new StringBuilder();
                buildDisclaimer.AppendLine("\nDISCLAIMER: You can comment and edit this post!");
                buildDisclaimer.AppendLine("Type: edit-help to see how to edit!");
                buildDisclaimer.AppendLine("Type: comment-post to see how to comment!");
                buildDisclaimer.AppendLine("Type: refresh to view again ths post!");
                Console.WriteLine(buildDisclaimer);
            }
            else
            {
                Console.WriteLine("\nDISCLAIMER: You can only comment to this post!\n");
            }

            while (true)
            {
                Console.Write("Blog -->#Post: ");
                string line = Console.ReadLine();
                if (line.ToLowerInvariant().Trim() == "return")
                {
                    break;
                }

                switch (line)
                {
                    case "refresh":
                        ViewPost(CurrentPost);
                        break;
                    case "comment-post":
                        CreateCommentInterface();
                        break;
                    case "comment-select-mine":
                        ShowUserComments();
                        break;
                    case "edit-help":
                        ShowEditCommands();
                        break;
                    case "edit-title":
                        Console.Write("New title: ");
                        EditPostTitle(CurrentPost, Console.ReadLine());
                        break;
                    case "edit-content":
                        Console.Write("New content: ");
                        var buildContent = new StringBuilder();

                        while (true)
                        {
                            string input = Console.ReadLine();
                            if (input.ToLowerInvariant().Trim() == "done")
                            {
                                break;
                            }

                            buildContent.AppendLine(input);
                        }

                        EditPostContent(CurrentPost, buildContent.ToString());
                        break;
                }
            }
        }

        public static void ShowUserComments()
        {
            var commentIds = new List<int>();
            var commentContents = new List<string>();

            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM comments WHERE author_name=@n", conn))
                {
                    cmd.Parameters.AddWithValue("n", Account.Name);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            commentIds.Add(int.Parse($"{rdr["comment_id"]}"));
                            commentContents.Add($"{rdr["content"]}");
                        }
                    }
                }

                conn.Dispose();
            }

            for (int i = 0; i < commentIds.Count; i++)
            {
                var shortContent = new StringBuilder();
                for (int j = 0; j < commentContents[i].Length || j < 7; j++)
                {
                    shortContent.Append(commentContents[i][j]);
                }
                Console.WriteLine($"{i + 1} | {shortContent}...");
            }



            while (true)
            {
                Console.Write("Select comment: ");
                try
                {
                    string line = Console.ReadLine();
                    if (line.ToLowerInvariant().Trim() == "return")
                    {
                        break;
                    }

                    int selectedComment = int.Parse(line) - 1;
                    if (selectedComment > commentContents.Count - 1)
                    {
                        Console.WriteLine($"Please select comments from 1 to {commentContents.Count - 1}");
                        continue;
                    }

                    Console.WriteLine(commentContents[selectedComment]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Please type number!");
                }
            }
        }

        public static void EditPostContent(int postId, string content)
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"UPDATE posts SET content=@c WHERE post_id=@p",conn))
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
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"UPDATE posts SET title=@t WHERE post_id=@p",conn))
                {
                    cmd.Parameters.AddWithValue("t", title);
                    cmd.Parameters.AddWithValue("p", postId);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("You succesfully edited this post title!");

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
