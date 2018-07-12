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
            Console.WriteLine("Please log in or register before using my blog!");
            Console.WriteLine("If you have account login using this format: Username Password");
            Console.WriteLine("Otherwise create account using this format: new Username Password");
            Console.WriteLine("post Some random Title:Thiis a very informative post:guides,lol,stuff,memes");


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
                        Blog.ShowAllCommands();
                        break;
                    case "help-post":
                        Blog.ShowPostCommands();
                        break;
                    case "help-comment":
                        Blog.ShowCommentCommands();
                        break;
                }

                /*string[] inputCommand = input.Split(' ');

                if (Account.Logged)
                {
                    switch (inputCommand[0])
                    {
                        case "help":
                            Blog.ShowAllCommands();
                            break;
                        case "latests":
                            Blog.ShowLatests();
                            break;
                        case "profile":
                            Blog.ShowAccountInformation();
                            break;
                        case "comment:":
                            Blog.CreateComment(input);
                            break;
                        case "post":
                            Blog.CreatePost(input);
                            break;
                        case "view":
                            Blog.ViewPost(input);
                            break;
                        case "edit-post-title:":
                            Blog.EditPostTitle(input);
                            break;
                        case "edit-post-content:":
                            Blog.EditPostContent(input);
                            break;
                        case "edit-comment:":
                            Blog.EditComment(input);
                            break;
                    }
                }
                else
                {
                    if (inputCommand.Length < 2)
                    {
                        Console.WriteLine("You need to log in first!");
                        continue;
                    }

                    if (inputCommand[0] == "new")
                    {
                        Account.Create(inputCommand[1], inputCommand[2]);
                        Account.Login(inputCommand[1], inputCommand[2]);
                        continue;
                    }

                    Account.Login(inputCommand[0], inputCommand[1]);
                    if (Account.Logged)
                    {
                        Console.WriteLine("You sucesfully logged! type help to check for your commands!");
                    }
                    else
                    {
                        Console.WriteLine($"There is no account with {inputCommand[0]} {inputCommand[1]} details!");
                    }
                }
                */
            }
        }
    }
}
