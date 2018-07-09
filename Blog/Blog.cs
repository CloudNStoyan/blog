using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
    
    public static class Blog
    {
        public static void ShowAllCommands()
        {
            var buildAllCommands = new StringBuilder();
            buildAllCommands.AppendLine("********************************************************");
            buildAllCommands.AppendLine("*          To create new post type: help post          *");
            buildAllCommands.AppendLine("*    To add new comment to post type: help comment     *");
            buildAllCommands.AppendLine("********************************************************");
            Console.WriteLine(buildAllCommands.ToString().Trim());
        }

        public const string ConnectionPath = @"Server=vm5;Port=5437;Database=postgres;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
        public static void CreatePost(string title, string content)
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO posts (title,content,user_id) VALUES (@t,@c,@u)", conn);
                cmd.Parameters.AddWithValue("t", title);
                cmd.Parameters.AddWithValue("c", content);
                cmd.Parameters.AddWithValue("u", Account.Id);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public static void CreateComment( string content, string postId)
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO comments (author_name,content,post_id) VALUES (@a,@c,@p)",conn);
                cmd.Parameters.AddWithValue("a", Account.Name);
                cmd.Parameters.AddWithValue("c", content);
                cmd.Parameters.AddWithValue("p", int.Parse(postId));
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Dispose();
            }
        }

        public static void ShowLatests()
        {
            using (var conn = new NpgsqlConnection(ConnectionPath))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT * FROM posts LIMIT 10",conn);
                var rdr = cmd.ExecuteReader();
                var buildOutput = new StringBuilder();
                while (rdr.Read())
                {
                    buildOutput.AppendLine($"{rdr["title"]}");
                    buildOutput.AppendLine($"{rdr["content"]}");
                }

                Console.WriteLine(buildOutput);
                cmd.Dispose();
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
