using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    class Startup
    {
        static void Main()
        {
            Console.WriteLine("#==========================================================#");
            Console.WriteLine("#===========================BLOG===========================#");
            Console.WriteLine("#==========================================================#");
            while (true)
            {
                Console.Write("Do you have an existing account! (Y/N) ");
                string line = Console.ReadLine();
                if (line.ToLowerInvariant().Trim() == "n")
                {
                    Account.Create();
                    break;
                } else if (line.ToLowerInvariant().Trim() == "y")
                {
                    break;
                }
            }

            while (!Account.Logged)
            {
                Account.AskForLogin();
            }

            while (true)
            {
                Console.Write("Blog --> ");
                string input = Console.ReadLine();
                if (input.ToLowerInvariant().Trim() == "exit")
                {
                    break;
                }

                switch (input)
                {
                    case "help":
                        Commands.ShowAll();
                        break;
                    case "help-post":
                        Commands.ShowPosts();
                        break;
                    case "help-comment":
                        Commands.ShowComments();
                        break;
                    case "help-account":
                        Commands.ShowAccounts();
                        break;
                    case "help-view":
                        Commands.ShowView();
                        break;
                    case "post-create":
                        Post.BeginCreatingPost();
                        break;
                    case "my-posts":
                        Blog.ViewAccountPosts();
                        break;
                    case "all-posts":
                        Post.ViewAllPosts();
                        break;
                    case "latests":
                        Blog.ShowLatests();
                        break;
                    case "settings":
                        Account.AccountOptions();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                }
            }
        }
    }
}
