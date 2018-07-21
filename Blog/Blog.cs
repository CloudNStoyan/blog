using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Console = System.Console;

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

                Console.WriteLine("|Number|   Title   |");
                for (int i = 0; i < postIds.Count; i++)
                {
                    Console.WriteLine($"|{i + 1,5} | {postTitles[i],10}|");
                }

                if (postIds.Count > 0)
                {
                    Console.WriteLine("Type 'return' to return to the blog!\n");
                    while (true)
                    {
                        Console.Write("Post number: ");
                        string line = Console.ReadLine() ?? " ";

                        if (String.IsNullOrEmpty(line))
                        {
                            Console.WriteLine("Please type a number!");
                            continue;
                        }

                        if (line.ToLowerInvariant().Trim() == "return")
                        {
                            break;
                        }

                        int id = 0;

                        try
                        {
                            id = int.Parse(line.Trim()) - 1;
                            if (id < 0 || id > postIds.Count - 1)
                            {
                                Console.WriteLine($"Invalid number. Number must be in range of 1 to {postIds.Count}");
                                continue;
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("You need to type number!");
                            continue;
                        }

                        ChoosePost(postIds[id]);

                    }
                }
                else
                {
                    Console.WriteLine("You don't have any posts yet! Create your first by typing 'post-create' !\n");
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
                string line = Console.ReadLine() ?? " ";
                if (String.IsNullOrEmpty(line))
                {
                    Console.WriteLine("Your comment needs to be at least 1 char long!");
                    continue;
                }
                if (line.ToLowerInvariant().Trim() == "done")
                {
                    if (String.IsNullOrEmpty(buildComment.ToString().Trim()))
                    {
                        Console.WriteLine("Your comment needs to be at least 1 char long!");
                        continue;
                    }
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

            Console.WriteLine("You successfully commented!\n");
        }

        public static string EditComment(int commentId)
        {
            Console.Write("Edit comment(Type 'done' when you are done):");

            var buildNewComment = new StringBuilder();
            while (true)
            {
                string line = Console.ReadLine() ?? " ";
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

            Console.WriteLine("Great you edited this comment!\n");
            return newComment;
        }


        public static void ChoosePost(int id)
        {
            Post.ViewPost(id);
            PostInterface(CurrentPostUserId);
        }

        public static void DeleteProcess(int postId)
        {
            Console.WriteLine("Once you've deleted a post it can't never be restored!");
            Console.Write("Are you sure you want to delete this post? (Y/N): ");
            string option = Console.ReadLine() ?? " ";
            if (option.ToLowerInvariant().Trim() == "y")
            {
                Console.WriteLine("This requires your password!");
                Console.Write("Your password: ");
                string password = Console.ReadLine() ?? " ";
                if (password != Account.Password)
                {
                    Console.WriteLine("Password incorrect returning to the post!");
                    return;
                }

                DeletePost(postId);
            } else if (option.ToLowerInvariant().Trim() == "n")
            {
                return;
            }
        }

        public static void DeletePost(int postId)
        {
            DeleteAllCommentsFromPost(postId);
            DeleteAllConnectedTags(postId);

            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("DELETE FROM posts WHERE post_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("i", postId);
                    cmd.ExecuteNonQuery();
                }

                conn.Dispose();
            }

            Console.WriteLine("You sucesfully deleted this post!");
        }

        public static void DeleteAllCommentsFromPost(int postId)
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("DELETE FROM comments WHERE post_id=@i",conn))
                {
                    cmd.Parameters.AddWithValue("i", postId);
                    cmd.ExecuteNonQuery();
                }

                conn.Dispose();
            }
        }

        public static void DeleteAllConnectedTags(int postId)
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("DELETE FROM posts_tags WHERE post_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("i", postId);
                    cmd.ExecuteNonQuery();
                }

                conn.Dispose();
            }
        }
        public static void PostInterface(int userId)
        {
            if (Account.Id == userId)
            {
                Console.WriteLine("DISCLAIMER: You can edit this post!");
                Console.WriteLine("Type: edit-help to see how to edit!");
            }

            Console.WriteLine("Type 'all-comments' to see all comments on this post! (If any)");
            Console.WriteLine("Type 'my-comments' to see your comments on this post! (If any)");
            Console.WriteLine("Type 'comment-post' to comment on this post!");
            Console.WriteLine("Type 'return' to return to the blog!");
            Console.WriteLine("Type 'refresh' to view again this post!\n");

            while (true)
            {
                Console.Write("Blog -->#Post: ");
                string line = Console.ReadLine() ?? " ";

                if (String.IsNullOrEmpty(line))
                {
                    Console.WriteLine("Invalid command!");
                    continue;
                }

                if (line.ToLowerInvariant().Trim() == "return")
                {
                    break;
                }

                switch (line.ToLowerInvariant().Replace(" ", String.Empty))
                {
                    case "clear":
                        Console.Clear();
                        break;
                    case "refresh":
                        Post.ViewPost(CurrentPost);
                        break;
                    case "commentpost":
                        CreateCommentInterface();
                        break;
                    case "mycomments":
                        ShowUserComments();
                        break;
                    case "allcomments":
                        Blog.AllComments(CurrentPost);
                        break;
                    case "delete":
                        Blog.DeleteProcess(CurrentPost);
                        break;
                    case "edithelp":
                        CommandPrinter.ShowPostEdits();
                        break;
                    case "edittitle":
                        Console.Write("New title: ");
                        Post.EditPostTitle(CurrentPost, Console.ReadLine() ?? " ");
                        break;
                    case "editcontent":
                        Console.Write("New content: ");
                        var buildContent = new StringBuilder();

                        while (true)
                        {
                            string input = Console.ReadLine() ?? " ";
                            if (input.ToLowerInvariant().Trim() == "done")
                            {
                                break;
                            }

                            buildContent.AppendLine(input);
                        }

                        Post.EditPostContent(CurrentPost, buildContent.ToString());
                        break;
                    default:
                        Console.WriteLine("Your command was invalid!");
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

            if (commentContents.Count < 1)
            {
                Console.WriteLine("You dont have any comments on this post!");
                return;
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
                Console.WriteLine("Type 'return' to return to the post!!\n");
                Console.Write("Select comment: ");
                try
                {
                    string line = Console.ReadLine() ?? " ";
                    if (line.ToLowerInvariant().Trim() == "return")
                    {
                        break;
                    }

                    int selectedComment = int.Parse(line) - 1;
                    if (selectedComment > commentContents.Count - 1)
                    {
                        Console.WriteLine($"Please select comments from 1 to {commentContents.Count - 1}\n");
                        continue;
                    }

                    Console.WriteLine("\nPlease choose between viewing or editing the comment!");
                    Console.WriteLine("To view the comment type view");
                    Console.WriteLine("To edit the comment type edit");
                    Console.WriteLine("Return to choosing comment by typing 'return'\n");
                    while (true)
                    {
                        Console.Write("You choosed: ");
                        string input = Console.ReadLine() ?? " ";
                        if (input.ToLowerInvariant().Trim() == "view")
                        {
                            Console.WriteLine(commentContents[selectedComment] + "\n");
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
                            Console.WriteLine("Please choose between view,edit and return!\n");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Please type number!\n");
                }
            }
        }


        public static void AllComments(int postId)
        {
            var commentsContent = new List<string>();
            var authorNames = new List<string>();
            var commentIds = new List<int>();

            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM comments WHERE post_id=@i",conn))
                {
                    cmd.Parameters.AddWithValue("i", postId);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            commentIds.Add(int.Parse($"{rdr["comment_id"]}"));
                            commentsContent.Add($"{rdr["content"]}");
                            authorNames.Add($"{rdr["author_name"]}");
                        }
                    }
                }

                conn.Dispose();
            }

            if (commentIds.Count < 1)
            {
                Console.WriteLine("They aren't any comments on this post!");
            }

            while (true)
            {
                for (int i = 0; i < commentsContent.Count; i++)
                {
                    Console.WriteLine($"{i + 1} | {commentsContent[i].Substring(0, 7)}... | {authorNames[i]}");
                }

                Console.WriteLine("Choose a comment to view or edit!\n");

                Console.Write("You choosed: ");

                string line = Console.ReadLine() ?? " ";

                if (line.ToLowerInvariant().Trim() == "return")
                {
                    break;
                }

                int selectedIndex = -1;

                try
                {
                    selectedIndex = int.Parse(line) - 1;
                    if (selectedIndex < 1 || selectedIndex > commentsContent.Count - 1)
                    {
                        Console.WriteLine($"Please choose between 1 and {commentsContent.Count}\n");
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("You need to type number!\n");
                    continue;
                }

                Console.WriteLine("Please choose between view and edit");
                Console.WriteLine("Type 'view' to view the whole comment");
                Console.WriteLine("Type 'edit' to edit this comment (Only if you have the right to do it)\n");

                Console.Write("You want to: ");
                string option = Console.ReadLine() ?? " ";
                if (option.ToLowerInvariant().Trim() == "view")
                {
                    Console.WriteLine($"\n{commentsContent[selectedIndex]}\n\nAuthor:{authorNames[selectedIndex]}\n");
                } else if (option.ToLowerInvariant().Trim() == "edit")
                {
                    if (authorNames[selectedIndex] == Account.Name)
                    {
                        EditComment(commentIds[selectedIndex]);
                    }
                    else
                    {
                        Console.WriteLine("You dont have permission to edit this comment!");
                    }
                }
            }

        }

        public static void ShowLatests()
        {
            var postIds = new List<int>();
            var postTitles = new List<string>();

            using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM posts LIMIT 10", conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            postIds.Add(int.Parse(rdr["post_id"].ToString()));
                            postTitles.Add(rdr["title"].ToString());
                        }
                    }
                }

                for (int i = postIds.Count - 1; i >= 0; i--)
                {
                    Console.WriteLine($"{Math.Abs(i - postIds.Count)}| {postTitles[i]}");
                }

                if (postIds.Count > 0)
                {
                    Console.WriteLine("Type 'return' to return to the blog!\n");
                    while (true)
                    {
                        Console.Write("Post number: ");
                        string line = Console.ReadLine() ?? " ";
                        if (line.ToLowerInvariant().Trim() == "return")
                        {
                            break;
                        }

                        int id = 0;

                        try
                        {
                            id = postIds.Count - int.Parse(line.Trim());
                            if (id < 1 || id > postIds.Count - 1)
                            {
                                Console.WriteLine($"Invalid number. Number must be in range 1 and {postIds.Count}");
                                continue;
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("You need to type number!");
                        }

                        Blog.ChoosePost(postIds[id]);

                    }
                }
                else
                {
                    Console.WriteLine("They aren't any posts yet!\n");
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
