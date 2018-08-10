﻿using System;
using System.Collections.Generic;
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
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
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

            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password))
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
            string name = Console.ReadLine() ?? " ";
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Thats not valid login information!");
                return;
            }

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? " ";
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Thats not valid login information!");
                return;
            }

            Login(name, password);
        }

        public static void Create()
        {
            Console.WriteLine("You are creating new account please fill the fields!");
            string name;
            string password;
            while (true)
            {
                Console.Write("Username: ");
                name = Console.ReadLine() ?? " ";
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Invalid username!");
                    continue;
                }

                Console.Write("Password: ");
                password = Console.ReadLine() ?? " ";
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Invalid password!");
                    continue;
                }

                Console.Write("Confirm Password: ");
                string confirmPass = Console.ReadLine() ?? " ";
                if (string.IsNullOrEmpty(confirmPass))
                {
                    Console.WriteLine("Invalid confirm password!");
                    continue;
                }

                if (confirmPass == password)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Your password confirmation was incorrect!");
                }
            }

            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO users (name,password) VALUES (@n,@p)", conn);
                cmd.Parameters.AddWithValue("n", name);
                cmd.Parameters.AddWithValue("p", password);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Dispose();
            }


            Login(name, password);
        }

        public static void AccountOptions()
        {
            Console.WriteLine("Account Options:");
            Console.Write("Do you want to rename your account? (Y/N): ");
            while (true)
            {
                string renameInput = Console.ReadLine() ?? " ";
                if (string.IsNullOrEmpty(renameInput))
                {
                    Console.WriteLine("Invalid input choose between 'Y' for yes and 'N' for no!");
                    continue;

                }

                if (renameInput.ToLowerInvariant().Trim() == "y")
                {
                    Console.Write("New name: ");
                    string newName = Console.ReadLine() ?? " ";
                    Rename(Name, newName);
                    Console.WriteLine("Do you want to change another thing? (Y/N): ");
                    while (true)
                    {
                        string option = Console.ReadLine() ?? " ";
                        if (option.ToLowerInvariant().Trim() == "n")
                        {
                            return;
                        }
                        else if (option.ToLowerInvariant().Trim() == "y")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Choose between Y for yes and N for no!");
                        }
                    }

                    break;
                }
            }

            while (true)
            {
                Console.Write("Do you want to change your account password? (Y/N): ");
                string passwordChangeInput = Console.ReadLine() ?? " ";
                if (!string.IsNullOrEmpty(passwordChangeInput) && passwordChangeInput.ToLowerInvariant().Trim() == "y")
                {
                    while (true)
                    {
                        Console.Write("Current password: ");
                        string currentPassword = Console.ReadLine() ?? " ";
                        if (currentPassword == Password)
                        {
                            Console.Write("New password: ");
                            string newPassword = Console.ReadLine() ?? " ";
                            Console.Write("Confirm new password: ");
                            string confirmedNewPassword = Console.ReadLine() ?? " ";
                            if (newPassword == confirmedNewPassword)
                            {
                                ChangePassword(Id, newPassword);
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid password!");
                        }
                    }

                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input you must type 'Y' for yes and 'N' for no!");
                }
            }
        }

        public static void Rename(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                Console.WriteLine("Invalid new name!");
                return;
            }

            if (string.IsNullOrEmpty(oldName))
            {
                Console.WriteLine("Invalid old name!");
                return;
            }

            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                try
                {
                    using (var cmd = new NpgsqlCommand("UPDATE users SET name=@n WHERE user_id=@i", conn))
                    {
                        cmd.Parameters.AddWithValue("n", newName);
                        cmd.Parameters.AddWithValue("i", Id);
                        cmd.ExecuteNonQuery();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid name. This name is already being used!");
                    return;
                }

                conn.Dispose();
            }

            RenameComments(Id, newName);
        }

        public static void RenameComments(int userId, string newName)
        {

            var commentIds = new List<int>();

            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM comments WHERE user_id=@i", conn))
                {
                    cmd.Parameters.AddWithValue("i", userId);
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
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
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