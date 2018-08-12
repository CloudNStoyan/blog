using System;
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

                var database = new Database(conn);
                var parametars = new Dictionary<string, object>
                {
                    {"n", name},
                    {"p", password}
                };
                var user = database.QueryOne<UserPoco>("SELECT * FROM users WHERE name=@n AND password=@p;", parametars);
                if (user != null)
                {
                    Name = user.Name;
                    Password = user.Password;
                    Id = user.UserId;
                    Logged = true;
                    Console.WriteLine($"Sucesfully logged as {Name}");
                    Console.WriteLine("Type help for more information!\n");
                }
                else
                {
                    Console.WriteLine("Invalid login information!\n");
                }

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

                var database = new Database(conn);
                var parametars = new Dictionary<string,object>
                {
                    {"n", name},
                    {"p", password}
                };

                database.ExecuteNonQuery("INSERT INTO users (name,password) VALUES (@n,@p);",parametars);
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

                    var database = new Database(conn);
                    var parametars = new Dictionary<string,object>()
                    {
                        {"n", newName},
                        {"i", Id}
                    };
                    database.ExecuteNonQuery("UPDATE users SET name=@n WHERE user_id=@i;",parametars);
                    database.ExecuteNonQuery("UPDATE comments SET author_name=@n WHERE user_id=@i;", parametars);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid name. This name is already being used!");
                    return;
                }
            }
        }

        public static void ChangePassword(int userId, string newPassword)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                var parametars = new Dictionary<string,object>()
                {
                    {"p" , newPassword},
                    {"i", userId}
                };

                database.ExecuteNonQuery("UPDATE users SET password=@p WHERE user_id=@i;", parametars);
            }

            Console.WriteLine("You successfully changed your password!\n");
        }
    }
}