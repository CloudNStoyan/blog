using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Blog
{
    
    public static class Blog
    {
        public const string ConnectionString = @"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
        public static PostPoco CurrentPost;

        public static void ViewAccountPosts()
        {
            var user = new UserPoco {Name = Account.Name, Password = Account.Password, UserId = Account.Id};
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                var posts = service.GetAllUsersPosts(user);

                if (posts.Count > 0)
                {
                    var choosedPost = Post.ChoosePostInterface(posts);

                    if (choosedPost != null)
                    {
                        ChoosePost(choosedPost);
                    }
                }
                else
                {
                    Console.WriteLine(
                        "You don't have any posts yet! create your first post by typing 'post-create' !\n");
                }
            }
        }

        private static void CreateCommentInterface()
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
                var database = new Database(conn);
                var service = new Service(database);
                service.CreateComment(Account.Name, comment, CurrentPost.PostId, Account.Id);
            }

            Console.WriteLine("You successfully commented!\n");
        }

        private static void EditComment(CommentPoco comment)
        {
            Console.Write("Edit comment(Type 'done' when you are done):");

            string commentContent = string.Empty;

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
                    commentContent += line;
                }
            }

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                comment.Content = commentContent;
                service.UpdateComment(comment);
            }

            Console.WriteLine("Great you edited this comment!\n");
        }


        public static void ChoosePost(PostPoco post)
        {
            Post.ViewPost(post);
            PostInterface(post);
        }

        private static void DeleteProcess(PostPoco post)
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
                    DeletePost(post);
                }
                else
                {
                    Console.WriteLine("Password incorrect returning to the post!");
                }
            }
        }

        private static void DeletePost(PostPoco post)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                service.DeletePost(post);
            }

            Console.WriteLine("You sucesfully deleted this post!");
        }

        private static void PostInterface(PostPoco post)
        {
            if (Account.Id == post.UserId)
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
                            Blog.DeleteProcess(post);
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
                var database = new Database(conn);
                var service = new Service(database);
                comments = service.GetUserComments(Account.AccountToUserPoco());
            }

            if (comments.Count >= 1)
            {
                bool choosingComment = true;

                while (choosingComment)
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
                        choosingComment = false;
                    }
                    else
                    {
                        bool isNumber = int.TryParse(line, out int selectedIndex);
                        selectedIndex--;
                        if (isNumber)
                        {
                            if (selectedIndex >= 0 && selectedIndex <= comments.Count - 1)
                            {
                                Console.WriteLine("Please choose between view and edit");
                                Console.WriteLine("Type 'view' to view the whole comment");
                                Console.WriteLine(
                                    "Type 'edit' to edit this comment (Only if you have the right to do it)\n");

                                Console.Write("You want to: ");
                                string option = Console.ReadLine() ?? " ";
                                if (option.ToLowerInvariant().Trim() == "view")
                                {
                                    Console.WriteLine(
                                        $"\n{comments[selectedIndex].Content}\n\nAuthor:{comments[selectedIndex].AuthorName}\n");
                                }
                                else if (option.ToLowerInvariant().Trim() == "edit")
                                {
                                    if (comments[selectedIndex].UserId == Account.Id)
                                    {
                                        EditComment(comments[selectedIndex]);
                                    }
                                    else
                                    {
                                        Console.WriteLine("You dont have permission to edit this comment!");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Please choose between 1 and {comments.Count}\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("You need to type a number!\n");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("They aren't any comments on this post!");
            }
        }


        private static void AllComments(PostPoco post)
        {
            List<CommentPoco> comments;

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                comments = service.GetPostsAllCommentars(post);
            }

            if (comments.Count >= 1)
            {
                bool choosingComment = true;

                while (choosingComment)
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
                        choosingComment = false;
                    }
                    else
                    {
                        bool isNumber = int.TryParse(line, out int selectedIndex);
                        selectedIndex--;
                        if (isNumber)
                        {
                            if (selectedIndex >= 0 && selectedIndex <= comments.Count - 1)
                            {
                                Console.WriteLine("Please choose between view and edit");
                                Console.WriteLine("Type 'view' to view the whole comment");
                                Console.WriteLine(
                                    "Type 'edit' to edit this comment (Only if you have the right to do it)\n");

                                Console.Write("You want to: ");
                                string option = Console.ReadLine() ?? " ";
                                if (option.ToLowerInvariant().Trim() == "view")
                                {
                                    Console.WriteLine(
                                        $"\n{comments[selectedIndex].Content}\n\nAuthor:{comments[selectedIndex].AuthorName}\n");
                                }
                                else if (option.ToLowerInvariant().Trim() == "edit")
                                {
                                    if (comments[selectedIndex].UserId == Account.Id)
                                    {
                                        EditComment(comments[selectedIndex]);
                                    }
                                    else
                                    {
                                        Console.WriteLine("You dont have permission to edit this comment!");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Please choose between 1 and {comments.Count}\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("You need to type a number!\n");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("They aren't any comments on this post!");
            }
        }

        public static void ShowLatests()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                var service = new Service(database);
                var posts = service.GetNewestsPosts();

                var choosedPost = Post.ChoosePostInterface(posts);
                if (choosedPost != null)
                {
                    ChoosePost(choosedPost);
                }
            }
        }
    }
}
