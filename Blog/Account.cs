using System;
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
       public static string Password;
       public static int PostsCount;
       public static int Id;
       public static bool Logged;


       public static void Login(string name, string password)
       {
           using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
           {
               conn.Open();

               using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE name=@n AND password=@p", conn))
               {
                   cmd.Parameters.AddWithValue("n", name);
                   cmd.Parameters.AddWithValue("p", password);

                   using (var rdr = cmd.ExecuteReader())
                   {
                       while (rdr.Read())
                       {
                           Name = $"{rdr["name"]}";
                           Password = $"{rdr["password"]}";
                           Id = int.Parse($"{rdr["user_id"]}");
                       }
                   }
               }

               conn.Dispose();
           }

           if (Name != "" && Password != "")
           {
               Logged = true;
               Console.WriteLine($"Sucesfully logged as {Name}");
               Console.WriteLine("Type help for more information!\n");
           }
           else
           {
               Console.WriteLine("Invalid login information!\n");
               Name = "";
               Password = "";
               Id = -1;
           }
       }

       public static void AskForLogin()
       {
           Console.Write("Username: ");
           string name = Console.ReadLine();
           Console.Write("Password: ");
           string password = Console.ReadLine();

           Login(name,password);
        }

       public static void Create()
       {
           Console.WriteLine("You are creating new account please fill the fields!");
           string name;
           string password;
           while (true)
           {
               Console.Write("Username: ");
               name = Console.ReadLine();

               Console.Write("Password: ");
               password = Console.ReadLine();
               Console.Write("Confirm Password: ");
               if (Console.ReadLine() == password)
               {
                   break;
               }
               else
               {
                   Console.WriteLine("Your password confirmation was incorrect!");
               }
           }

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


           Login(name,password);
       }
   }
}
