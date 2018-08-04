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

            using (var conn = new NpgsqlConnection(@"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;"))
            {
                conn.Open();
                var myDatabase = new Database(conn);

                var result = myDatabase.Query<User>("SELECT * FROM users");

                foreach (var user in result)
                {
                    Console.WriteLine(user.name);
                }
            }

            while (true)
            {
                Console.Write("Do you have an existing account! (Y/N) ");
                string line = Console.ReadLine() ?? " ";

                if (String.IsNullOrEmpty(line))
                {
                    Console.WriteLine("Thats not a valid answer use only 'Y' for yes and 'N' for no!");
                    continue;
                }

                if (line.ToLowerInvariant().Trim() == "n")
                {
                    Account.Create();
                    break;
                } else if (line.ToLowerInvariant().Trim() == "y")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Thats not a valid answer use only 'Y' for yes and 'N' for no!");
                }
            }

            while (!Account.Logged)
            {
                Account.AskForLogin();
            }

            while (true)
            {
                Console.Write("Blog --> ");
                string input = Console.ReadLine() ?? " ";

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Thats not valid command!");
                    continue;
                }

                if (input.ToLowerInvariant().Trim() == "exit")
                {
                    break;
                }

                switch (input.ToLowerInvariant().Replace(" ", String.Empty))
                {
                    case "help":
                        CommandPrinter.ShowAll();
                        break;
                    case "helppost":
                        CommandPrinter.ShowPosts();
                        break;
                    case "helpcomment":
                        CommandPrinter.ShowComments();
                        break;
                    case "helpaccount":
                        CommandPrinter.ShowAccounts();
                        break;
                    case "helpview":
                        CommandPrinter.ShowView();
                        break;
                    case "postcreate":
                        Post.BeginCreatingPost();
                        break;
                    case "myposts":
                        Blog.ViewAccountPosts();
                        break;
                    case "allposts":
                        Post.ViewAllPosts();
                        break;
                    case "newposts":
                        Blog.ShowLatests();
                        break;
                    case "settings":
                        Account.AccountOptions();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Thats not valid command!");
                        break;
                }
            }
        }
    }
}
