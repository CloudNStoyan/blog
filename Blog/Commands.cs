using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog
{
    public class Commands
    {
        private const int commandRange = -15;
        private const int explanationRange = -84;

        public static void PrintCommand(string command, string explanation)
        {
            Console.WriteLine($"|{command,commandRange} | {explanation,explanationRange} |");
        }

        public static void PrintCommands(Dictionary<string, string> commands)
        {
            Console.WriteLine("|".PadRight(17, '-') + '|' + "|".PadLeft(87, '-'));
            Console.WriteLine("| " + "Command".PadRight(15, ' ') + "|".PadRight(39, ' ') + "Explanation" + "|".PadLeft(38, ' '));
            Console.WriteLine("|".PadRight(17, '-') + '|' + "|".PadLeft(87, '-'));
            foreach (var command in commands)
            {
                Console.WriteLine($"|{command.Key,commandRange} | {command.Value,explanationRange} |");
            }
            Console.WriteLine("|".PadRight(17, '-') + '|' + "|".PadLeft(87, '-'));
        }

        public static void PrintLine()
        {
            Console.WriteLine("|".PadRight(17, '-') + '|' + "|".PadLeft(87, '-'));
        }

        public static void PrintQuote()
        {
            Console.WriteLine("| " + "Command".PadRight(15, ' ') + "|".PadRight(39, ' ') + "Explanation" + "|".PadLeft(38, ' '));
        }

        public static void ShowAll()
        {
            var showAllDic = new SortedDictionary<string, string>();
            showAllDic.Add("help-post", "This command gives you information on how to post a new post!");
            showAllDic.Add("help-comment", "This command gives you information on how to comment on a post!");
            showAllDic.Add("help-view", "This command gives you information on how to view material on the blog!");
            showAllDic.Add("help-account", "This command gives you information on what profile commands you have!");
            //PrintCommands(showAllDic);
        }

        public static void ShowPosts()
        {
            var showPostsDic = new SortedDictionary<string,string>();
            PrintCommand("post-create","This command walk you through a process for making a new post!");
            PrintCommand("all-posts","This command will show you list of all posts and a way to choose which one to view!");
            PrintLine();
        }

        public static void ShowComments()
        {
            PrintLine();
            PrintQuote();
            PrintLine();
            PrintCommand("comment-create","This command walks you through a process for commenting on an existing post!");
            PrintCommand("comment-edit","This command walks you through a process for editing an existing comment!");
            PrintLine();
        }

        public static void ShowAccounts()
        {
            PrintLine();
            PrintQuote();
            PrintLine();
            PrintCommand("my-posts","This command let you choose which of your posts you want to view!");
            PrintCommand("settings","This command let you edit your profile settings!");
            PrintLine();
        }

        public static void ShowPostEdits()
        {
            PrintLine();
            PrintQuote();
            PrintLine();
            PrintCommand("edit-title","This command is giving you the opportunity to edit the title of the post!");
            PrintCommand("edit-content","This command is giving you the opportunity to edit the content of the post!");
            PrintLine();
        }

        public static void ShowView()
        {
            PrintLine();
            PrintQuote();
            PrintLine();
            PrintCommand("my-posts","This command is letting you to choose from your posts to view!");
            PrintCommand("all-posts","This command is letting you to choose from all posts to view!");
            PrintCommand("new-posts","This command is letting youto choose from the new posts!");
            PrintLine();
        }
    }
}
