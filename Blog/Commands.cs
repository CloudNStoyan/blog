using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog
{
    public class Commands
    {
        public static void ShowAll()
        {
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine("| Command        |                                      Explanation                                     |");
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "help-post", "This command gives you information on how to post a new post!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "help-comment", "This command gives you information on how to comment on a post!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "help-view", "This command gives you information on how to view material on the blog!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "help-account", "This command gives you information on what profile commands you have!"));
            Console.WriteLine("|-------------------------------------------------------------------------------------------------------|");
        }

        public static void ShowPosts()
        {
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine("| Command        |                                      Explanation                                     |");
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "post-create", "This command walks you through a procces for making a new post!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "all-posts", "This command will show you list of all posts and a way to choose which one to view!"));
            Console.WriteLine("|-------------------------------------------------------------------------------------------------------|");
        }

        public static void ShowComments()
        {
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine("| Command        |                                      Explanation                                     |");
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "comment-create", "This command walks you through a procces for commenting on a existing post!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "comment-edit", "This command walks you through a procces for editing an existing comment!"));
            Console.WriteLine("|-------------------------------------------------------------------------------------------------------|");
        }

        public static void ShowAccounts()
        {
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine("| Command        |                                      Explanation                                     |");
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "my-posts", "This command let you choose which of your posts you want to view!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "settings", "This command let you edit your profile settings!"));
            Console.WriteLine("|-------------------------------------------------------------------------------------------------------|");
        }

        public static void ShowPostEdits()
        {
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine("| Command        |                                      Explanation                                     |");
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "edit-title", "This command is giving you the opportunity to edit the title of the post!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "edit-content", "This command is giving you the opportunity to edit the content of the post!"));
            Console.WriteLine("|-------------------------------------------------------------------------------------------------------|");
        }

        public static void ShowView()
        {
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine("| Command        |                                      Explanation                                     |");
            Console.WriteLine("|----------------|--------------------------------------------------------------------------------------|");
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "my-posts", "This command is letting you choose from your posts to view!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "all-posts", "This command is letting you choose from all posts to view!"));
            Console.WriteLine(String.Format("|{0,-15} | {1,-84} |", "new-posts", "This command is letting you choose from the new posts!"));
            Console.WriteLine("|-------------------------------------------------------------------------------------------------------|");
        }
    }
}
