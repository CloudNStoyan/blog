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
            var buildAllCommands = new StringBuilder();
            buildAllCommands.AppendLine("<--All Help Commands-->");
            buildAllCommands.AppendLine("# help-post");
            buildAllCommands.AppendLine("* This command gives you information on how to post a new post!");
            buildAllCommands.AppendLine("# help-comment");
            buildAllCommands.AppendLine("* This command gives you information on how to comment on a post!");
            buildAllCommands.AppendLine("# help-view");
            buildAllCommands.AppendLine("* This command gives you information on how to view material on the blog!");
            buildAllCommands.AppendLine("# help-account");
            buildAllCommands.AppendLine("* This command gives you information on what profile commands you have!");
            buildAllCommands.AppendLine("<--All Help Commands-->");
            Console.WriteLine($"{buildAllCommands.ToString().Trim()}\n");
        }

        public static void ShowPosts()
        {
            var buildPostCommands = new StringBuilder();
            buildPostCommands.AppendLine("<--Post commands-->");
            buildPostCommands.AppendLine("# post-create");
            buildPostCommands.AppendLine("* This command walks you through a procces for making a new post!");
            buildPostCommands.AppendLine("# post-all");
            buildPostCommands.AppendLine("* This command will show you a list of all posts and a way to choose which one to view!");
            buildPostCommands.AppendLine("<--Post commands-->");
            Console.WriteLine($"{buildPostCommands.ToString().Trim()}\n");
        }

        public static void ShowComments()
        {
            var buildCommentCommands = new StringBuilder();
            buildCommentCommands.AppendLine("<--Comment commands-->");
            buildCommentCommands.AppendLine("# comment-create");
            buildCommentCommands.AppendLine("* This command walks you through a procces for commenting on a existing post!");
            buildCommentCommands.AppendLine("# comment-edit");
            buildCommentCommands.AppendLine("* This command walks you through a procces for editing an existing comment!");
            buildCommentCommands.AppendLine("<--Comment commands-->");
            Console.WriteLine($"{buildCommentCommands.ToString().Trim()}\n");
        }

        public static void ShowAccounts()
        {
            var buildAccountCommands = new StringBuilder();
            buildAccountCommands.AppendLine("<--Account commands-->");
            buildAccountCommands.AppendLine("# my-posts");
            buildAccountCommands.AppendLine("* This command let you choose which of your posts you want to view!");
            buildAccountCommands.AppendLine("# settings");
            buildAccountCommands.AppendLine("* This command let you edit your profile settings!");
            buildAccountCommands.AppendLine("<--Account commands-->");
            Console.WriteLine($"{buildAccountCommands.ToString().Trim()}\n");
        }

        public static void ShowPostEdits()
        {
            var buildEditCommands = new StringBuilder();
            buildEditCommands.AppendLine("<--Edit commands-->");
            buildEditCommands.AppendLine("# edit-title");
            buildEditCommands.AppendLine("* This command is giving you the opportunity to edit the title of the post!");
            buildEditCommands.AppendLine("# edit-content");
            buildEditCommands.AppendLine("* This command is giving you the opportunity to edit the content of the post!");
            buildEditCommands.AppendLine("<--Edit commands-->");
            Console.WriteLine($"{buildEditCommands.ToString().Trim()}\n");
        }

        public static void ShowView()
        {
            var buildViewCommands = new StringBuilder();
            buildViewCommands.AppendLine("<--View commands-->");
            buildViewCommands.AppendLine("# my-posts");
            buildViewCommands.AppendLine("* This command is letting you choose from your posts to view!");
            buildViewCommands.AppendLine("# all-posts");
            buildViewCommands.AppendLine("* This command is letting you choose from all posts to view!");
            buildViewCommands.AppendLine("# latests");
            buildViewCommands.AppendLine("* This command is letting you choose from the new posts!");
            buildViewCommands.AppendLine("<--View commands-->");
            Console.WriteLine($"{buildViewCommands.ToString().Trim()}\n");
        }
    }
}
