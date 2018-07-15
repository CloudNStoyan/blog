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
                    Console.WriteLine("Type 'return' to return to the blog!");
                    Console.Write("Post number: ");
                    string line = Console.ReadLine();
                    if (line.ToLowerInvariant().Trim() == "return")
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

        public static string EditComment(int commentId)
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

                using (var cmd = new NpgsqlCommand("UPDATE comments SET content=@c WHERE comment_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("c", newComment);
                    cmd.Parameters.AddWithValue("i", commentId);
                    cmd.ExecuteNonQuery();
                }

                conn.Dispose();
            }

            Console.WriteLine("Great you edited this comment!");
            return newComment;
        }


        public static void ChoosePost(int id)
        {
            Post.ViewPost(id);
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
                Console.WriteLine("Type 'return' to return to the blog!");
                Console.Write("Blog -->#Post: ");
                string line = Console.ReadLine();
                if (line.ToLowerInvariant().Trim() == "return")
                {
                    break;
                }

                switch (line)
                {
                    case "refresh":
                        Post.ViewPost(CurrentPost);
                        break;
                    case "comment-post":
                        CreateCommentInterface();
                        break;
                    case "comment-select-mine":
                        ShowUserComments();
                        break;
                    case "edit-help":
                        Commands.ShowPostEdits();
                        break;
                    case "edit-title":
                        Console.Write("New title: ");
                        Post.EditPostTitle(CurrentPost, Console.ReadLine());
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

                        Post.EditPostContent(CurrentPost, buildContent.ToString());
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
                Console.WriteLine("Type 'return' to return to the post!!");
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

                    Console.WriteLine("Please choose between viewing or editing the comment!");
                    Console.WriteLine("To view the comment type view");
                    Console.WriteLine("To edit the comment type edit");
                    Console.WriteLine("Return to choosing comment by typing 'return'");
                    while (true)
                    {
                        Console.Write("You choosed: ");
                        string input = Console.ReadLine();
                        if (input.ToLowerInvariant().Trim() == "view")
                        {
                            Console.WriteLine(commentContents[selectedComment]);
                        } else if (input.ToLowerInvariant().Trim() == "edit")
                        {
                            string editedComment = EditComment(commentIds[selectedComment]);
                            commentContents[selectedComment] = editedComment;
                        } else if (input.ToLowerInvariant().Trim() == "return")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Please choose between view,edit and return!");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Please type number!");
                }
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
