using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Blog
{
    
    public static class Blog
    {
        public const string ConnectionString = @"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
        public static int CurrentPost;
        public static int CurrentPostUserId;

        public static void ViewAccountPosts()
        {
            List<PostPoco> posts;
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var database = new Database(conn);
                posts = database.Query<PostPoco>("SELECT * FROM posts WHERE user_id=@i;",
                    new NpgsqlParameter("i", Account.Id));
            }

            if (posts.Count > 0)
            {
                int choosedPost = Post.ChoosePostInterface(posts);
                if (choosedPost >= 0)
                ChoosePost(posts[choosedPost].PostId);
            }
            else
            {
                Console.WriteLine("You don't have any posts yet! create your first post by typing 'post-create' !\n");
            }
        }

        public static void CreateCommentInterface()
        {
            Console.Write("Your comment(Type 'done' when you are done!): ");
            string comment = string.Empty;
            bool creatingComment = true;
            while (creatingComment)
            {
                string line = Console.ReadLine()?.Trim();

                if (line?.ToLowerInvariant() == "done")
                {
                    if (!string.IsNullOrEmpty(comment.Trim()))
                    {
                        creatingComment = false;
                    }
                    else
                    {
                        Console.WriteLine("The comment needs to be at least 1 char long!");

                    }
                }
                else
                {
                    comment += line;
                }
            }

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var database = new Database(conn);
                var parametars = new[]
                {
                    new NpgsqlParameter("a", Account.Name),
                    new NpgsqlParameter("c", comment),
                    new NpgsqlParameter("p", CurrentPost),
                    new NpgsqlParameter("i", Account.Id)
                };
                database.ExecuteNonQuery("INSERT INTO comments (author_name,content,post_id,user_id) VALUES (@a,@c,@p,@i);", parametars);
            }



            Console.WriteLine("You successfully commented!\n");
        }

        private static string EditComment(int commentId)
        {
            Console.Write("Edit comment(Type 'done' when you are done):");

            string newComment = string.Empty;

            bool creatingComment = true;

            while (creatingComment)
            {
                string line = Console.ReadLine();
                if (line?.ToLowerInvariant().Trim() == "done")
                {
                    creatingComment = false;
                }
                else
                {
                    newComment += line;
                }
            }

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);

                var parametars = new[]
                {
                    new NpgsqlParameter("c", newComment),
                    new NpgsqlParameter("i", commentId)
                };

                database.ExecuteNonQuery("UPDATE comments SET content=@c WHERE comment_id@i;", parametars);
            }

            Console.WriteLine("Great you edited this comment!\n");
            return newComment;
        }


        public static void ChoosePost(int id)
        {
            Post.ViewPost(id);
            PostInterface(CurrentPostUserId);
        }

        private static void DeleteProcess(int postId)
        {
            Console.WriteLine("Once you've deleted a post it can't never be restored!");
            Console.Write("Are you sure you want to delete this post? (Y/N): ");
            string option = Console.ReadLine()?.ToLowerInvariant().Trim();
            if (option == "y")
            {
                Console.WriteLine("This requires your password!");
                Console.Write("Your password: ");
                string password = Console.ReadLine();
                if (password == Account.Password)
                {
                    DeletePost(postId);
                }
                else
                {
                    Console.WriteLine("Password incorrect returning to the post!");
                }
            }
        }

        private static void DeletePost(int postId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);

                var parametar = new Dictionary<string, object> {{"i", postId}};

                database.ExecuteNonQuery("DELETE FROM comments WHERE post_id=@i;", parametar);
                database.ExecuteNonQuery("DELETE FROM posts_tags WHERE post_id=@i;", parametar);
                database.ExecuteNonQuery("DELETE FROM posts WHERE post_id=@i;", parametar);
            }

            Console.WriteLine("You sucesfully deleted this post!");
        }

        private static void PostInterface(int userId)
        {
            if (Account.Id == userId)
            {
                Console.WriteLine("DISCLAIMER: You can edit this post!");
                Console.WriteLine("Type: edit help to see how to edit!");
            }

            Console.WriteLine("Type 'all comments' to see all comments on this post! (If any)");
            Console.WriteLine("Type 'my comments' to see your comments on this post! (If any)");
            Console.WriteLine("Type 'comment post' to comment on this post!");
            Console.WriteLine("Type 'return' to return to the blog!");
            Console.WriteLine("Type 'refresh' to view again this post!\n");

            while (true)
            {
                Console.Write("Blog -->#Post: ");
                string line = Console.ReadLine()?.Trim().ToLowerInvariant().Replace(" ", string.Empty);

                if (!string.IsNullOrEmpty(line))
                {

                    if (line == "return")
                    {
                        break;
                    }

                    switch (line)
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
                            EditPostContentInterface();
                            break;
                        default:
                            Console.WriteLine("Your command was invalid!");
                            break;
                    }
                }
            }
        }

        private static void EditPostContentInterface()
        {
            Console.Write("New content: ");
            string content = String.Empty;

            bool creatingContent = true;

            while (creatingContent)
            {
                string input = Console.ReadLine();

                if (input.Trim().ToLowerInvariant() != "done")
                {
                    content += input;
                }
                else
                {
                    creatingContent = false;
                }
            }

            Post.EditPostContent(CurrentPost, content);
        }

        private static void ShowUserComments()
        {
            List<CommentPoco> comments;

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                comments = database.Query<CommentPoco>("SELECT * FROM comments WHERE author_name=@n;",
                    new NpgsqlParameter("n", Account.Name));
            }

            if (comments.Count >= 1)
            {
                for (int i = 0; i < comments.Count; i++)
                {
                    string commentContent = comments[i].Content;
                    string shortContent = commentContent.Length > 7
                        ? commentContent.Substring(0, 7) + "..."
                        : commentContent;
                    Console.WriteLine($"{i + 1} | {shortContent}");
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
                        if (selectedComment > comments.Count - 1)
                        {
                            Console.WriteLine($"Please select comments from 1 to {comments.Count - 1}\n");
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
                                Console.WriteLine(comments[selectedComment].Content + "\n");
                            }
                            else if (input.ToLowerInvariant().Trim() == "edit")
                            {
                                string editedComment = EditComment(comments[selectedComment].CommentId);
                                comments[selectedComment].Content = editedComment;
                            }
                            else if (input.ToLowerInvariant().Trim() == "return")
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
            else
            {
                Console.WriteLine("You dont have any comments on this post!");
            }
        }


        private static void AllComments(int postId)
        {
            List<CommentPoco> comments;

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                comments = database.Query<CommentPoco>("SELECT * FROM comments WHERE post_id=@i;", new NpgsqlParameter("i", postId));

            }

            if (comments.Count < 1)
            {
                Console.WriteLine("They aren't any comments on this post!");
            }

            while (true)
            {
                for (int i = 0; i < comments.Count; i++)
                {
                    string shortContent = comments[i].Content.Length > 7
                        ? comments[i].Content.Substring(0, 7) + "..."
                        : comments[i].Content;
                    Console.WriteLine($"{i + 1} | {shortContent} | {comments[i].AuthorName}");
                }

                Console.WriteLine("Choose a comment to view or edit!\n");

                Console.Write("You choosed: ");

                string line = Console.ReadLine() ?? " ";

                if (line.ToLowerInvariant().Trim() == "return")
                {
                    break;
                }

                int selectedIndex;

                try
                {
                    selectedIndex = int.Parse(line) - 1;
                    if (selectedIndex < 1 || selectedIndex > comments.Count - 1)
                    {
                        Console.WriteLine($"Please choose between 1 and {comments.Count}\n");
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
                    Console.WriteLine($"\n{comments[selectedIndex].Content}\n\nAuthor:{comments[selectedIndex].AuthorName}\n");
                } else if (option.ToLowerInvariant().Trim() == "edit")
                {
                    if (comments[selectedIndex].AuthorName == Account.Name)
                    {
                        EditComment(comments[selectedIndex].CommentId);
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
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                var posts = database.Query<PostPoco>("SELECT * FROM posts ORDER BY post_id LIMIT 10;");

                for (int i = posts.Count - 1; i >= 0; i--)
                {
                    Console.WriteLine($"{Math.Abs(i - posts.Count)}| {posts[i].Title}");
                }

                if (posts.Count > 0)
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
                            id = posts.Count - int.Parse(line.Trim());
                            if (id < 1 || id > posts.Count - 1)
                            {
                                Console.WriteLine($"Invalid number. Number must be in range 1 and {posts.Count}");
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("You need to type number!");
                        }

                        Blog.ChoosePost(posts[id].PostId);

                    }
                }
                else
                {
                    Console.WriteLine("They aren't any posts yet!\n");
                }

                conn.Dispose();
            }
        }
    }
}
