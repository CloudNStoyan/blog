﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Blog
{
   public static class Account
   {
       public static string Name;
       public static int PostsCount;
       public static int Id;
       public static bool Logged;

       public static void Login(string name, string password)
       {
           using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
           {
               conn.Open();
               var cmd = new NpgsqlCommand("SELECT * FROM users WHERE name=@n AND password=@p",conn);
               cmd.Parameters.AddWithValue("n", name);
               cmd.Parameters.AddWithValue("p", password);
               var rdr = cmd.ExecuteReader();
               var buildOutput = new StringBuilder();
               while (rdr.Read())
               {
                   buildOutput.Append($"{rdr["name"]}:{rdr["password"]}:{rdr["user_id"]}");
               }

               string[] userInfo = buildOutput.ToString().Split(':');

               if (buildOutput.Length > 1)
               {
                   Name = userInfo[0];
                   Id = int.Parse(userInfo[2]);
                   Logged = true;
               }
               else
               {
                   Console.WriteLine("Invalid login information!");
               }


               cmd.Dispose();
               conn.Dispose();
           }
       }

       public static void Create(string name, string password)
       {
           using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
           {
               conn.Open();
               var cmd = new NpgsqlCommand("INSERT INTO users (name,password) VALUES (@n,@p)",conn);
               cmd.Parameters.AddWithValue("n", name);
               cmd.Parameters.AddWithValue("p", password);
               cmd.ExecuteNonQuery();
               cmd.Dispose();
               conn.Dispose();
           }
       }
   }
}
