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

           if (!String.IsNullOrWhiteSpace(Name) && !String.IsNullOrWhiteSpace(Password))
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

       public static void AccountOptions()
       {
           Console.WriteLine("Account Options:");
           Console.Write("Do you want to rename your account? (Y/N): ");
           string renameInput = Console.ReadLine();
           if (renameInput.ToLowerInvariant().Trim() == "y")
           {
               Console.Write("New name: ");
               string newName = Console.ReadLine();
               Rename(Name, newName);
               Console.WriteLine("Do you want to change another thing? (Y/N): ");
               while (true)
               {
                   string option = Console.ReadLine();
                   if (option.ToLowerInvariant().Trim() == "n")
                   {
                       return;
                   }
                   else if (option.ToLowerInvariant().Trim() == "y")
                   {
                       Console.WriteLine("Ok!");
                       break;
                   }
                   else
                   {
                       Console.WriteLine("Choose between Y for yes and N for no!");
                   }
               }
           }

           Console.Write("Do you want to change your account password? (Y/N): ");
           string passwordChangeInput = Console.ReadLine();

           if (passwordChangeInput.ToLowerInvariant().Trim() == "y")
           {
               while (true)
               {
                   Console.Write("Current password: ");
                   string currentPassword = Console.ReadLine();
                   if (currentPassword == Password)
                   {
                       Console.Write("New password: ");
                       string newPassword = Console.ReadLine();
                       Console.Write("Confirm new password: ");
                       string confirmedNewPassword = Console.ReadLine();
                       if (newPassword == confirmedNewPassword)
                       {
                            ChangePassword(Id,newPassword);
                           break;
                       }
                   }
                   else
                   {
                       Console.WriteLine("Invalid password!");
                    }
               }
           }
       }

       public static void Rename(string oldName, string newName)
       {
           using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
           {
               conn.Open();

               using (var cmd = new NpgsqlCommand("UPDATE users SET name=@n WHERE user_id=@i", conn))
               {
                   cmd.Parameters.AddWithValue("n", newName);
                   cmd.Parameters.AddWithValue("i", Id);
                   cmd.ExecuteNonQuery();
               }

               conn.Dispose();
           }

           RenameComments(oldName, newName);

           Console.WriteLine("You succsefully renamed your account!\n");
       }

       public static void RenameComments(string oldName, string newName)
       {

           var commentIds = new List<int>();

           using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
           {
               conn.Open();
               using (var cmd = new NpgsqlCommand("SELECT * FROM comments WHERE author_name=@n", conn))
               {
                   cmd.Parameters.AddWithValue("n", oldName);
                   using (var rdr = cmd.ExecuteReader())
                   {
                       while (rdr.Read())
                       {
                            commentIds.Add(int.Parse($"{rdr["comment_id"]}"));
                       }
                   }
               }

               foreach (int commentId in commentIds)
               {
                   using (var cmd = new NpgsqlCommand("UPDATE comments SET author_name=@n WHERE comment_id=@i", conn))
                   {
                       cmd.Parameters.AddWithValue("n", newName);
                       cmd.Parameters.AddWithValue("i", commentId);
                       cmd.ExecuteNonQuery();
                   }
               }

               conn.Dispose();
           }
        }

       public static void ChangePassword(int userId, string newPassword)
       {
           using (var conn = new NpgsqlConnection(Blog.ConnectionPath))
           {
               conn.Open();
               using (var cmd = new NpgsqlCommand("UPDATE users SET password=@p WHERE user_id=@i", conn))
               {
                   cmd.Parameters.AddWithValue("p", newPassword);
                   cmd.Parameters.AddWithValue("i", userId);
                   cmd.ExecuteNonQuery();
               }
               conn.Dispose();
           }

           Console.WriteLine("You successfully changed your password!\n");
       }
   }
}
