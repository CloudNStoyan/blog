using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog
{
    public static class CommandPrinter
    {
        private const int CommandRange = -15;
        private const int ExplanationRange = -84;

        private static void PrintCommands(Dictionary<string, string> commandDic)
        {
            Console.WriteLine("|".PadRight(17, '-') + '|' + "|".PadLeft(87, '-'));
            Console.WriteLine("| " + "Command".PadRight(15, ' ') + "|".PadRight(39, ' ') + "Explanation" + "|".PadLeft(38, ' '));
            Console.WriteLine("|".PadRight(17, '-') + '|' + "|".PadLeft(87, '-'));
            foreach (var command in commandDic)
            {
                Console.WriteLine($"|{command.Key,CommandRange} | {command.Value,ExplanationRange} |");
            }
            Console.WriteLine("|".PadRight(17, '-') + '|' + "|".PadLeft(87, '-'));
        }

        public static void ShowAll()
        {
            var showAllDic = new Dictionary<string, string>
            {
                { "help post", "This command gives you information on how to post a new post!" },
                { "help comment", "This command gives you information on how to comment on a post!" },
                { "help view", "This command gives you information on how to view material on the blog!" },
                { "help account", "This command gives you information on what profile commandDic you have!" }
            };
            PrintCommands(showAllDic);
        }

        public static void ShowPosts()
        {
            var showPostsDic = new Dictionary<string, string>
            {
                { "post create", "This command walk you through a process for making a new post!" },
                { "all posts", "This command will show you list of all posts and a way to choose which one to view!" }
            };
            PrintCommands(showPostsDic);
        }

        public static void ShowComments()
        {
            var showCommentsDic = new Dictionary<string, string>
            {
                { "comment create", "This command walks you through a process for commenting on an existing post!" },
                { "comment edit", "This command walks you through a process for editing an existing comment!" }
            };
            PrintCommands(showCommentsDic);
        }

        public static void ShowAccounts()
        {
            var showAccountsDic = new Dictionary<string, string>
            {
                { "my posts", "This command let you choose which of your posts you want to view!" },
                { "settings", "This command let you edit your profile settings!" }
            };
            PrintCommands(showAccountsDic);

        }

        public static void ShowPostEdits()
        {
            var showPostsDic = new Dictionary<string, string>
            {
                { "edit title", "This command is giving you the opportunity to edit the title of the post!" },
                { "edit content", "This command is giving you the opportunity to edit the content of the post!" }
            };
            PrintCommands(showPostsDic);
        }

        public static void ShowView()
        {
            var showViewDic = new Dictionary<string, string>
            {
                {"my posts", "This command is letting you to choose from your posts to view!"},
                {"all posts", "This command is letting you to choose from all posts to view!"},
                {"new posts", "This command is letting youto choose from the new posts!"}
            };
            PrintCommands(showViewDic);
        }
    }
}
