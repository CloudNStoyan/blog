using System;
using System.Collections.Generic;
using Npgsql;

namespace Blog
{
    public static class Account
    {
        public static string Name;
        public static string Password;
        public static int Id;
        public static bool Logged;

        public static void LoginInterface()
        {
            Console.Write("Username: ");
            string name = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Thats not valid login information!");
            }
            else
            {
                Login(name, password);

                if (Logged)
                {
                    Console.WriteLine($"Sucesfully logged as {Name}");
                    Console.WriteLine("Type help for more information!\n");
                }
                else
                {
                    Console.WriteLine("Invalid login information!");
                    Console.WriteLine("Try again!");
                }
            }
        }

        public static void CreateInterface()
        {
            Console.WriteLine("You are creating new account please fill the fields!");
            bool creatingAccount = true;
            while (creatingAccount)
            {
                Console.Write("Username: ");
                string name = Console.ReadLine() ?? " ";
                Console.Write("Password: ");
                string password = Console.ReadLine() ?? " ";
                Console.Write("Confirm Password: ");
                string confirmPass = Console.ReadLine() ?? " ";

                if (string.IsNullOrEmpty(confirmPass) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Invalid information!");
                }
                else
                {
                    using (var conn = new NpgsqlConnection(Blog.ConnectionString))
                    {
                        conn.Open();

                        var database = new Database(conn);

                        if (UserExist(name))
                        {
                            Console.WriteLine("This username is already taken try another one!");
                            CreateInterface();
                        }


                        var parametars = new Dictionary<string, object>
                        {
                            {"n", name},
                            {"p", password}
                        };

                        database.ExecuteNonQuery("INSERT INTO users (name,password) VALUES (@n,@p);", parametars);

                        Login(name,password);

                        if (Logged)
                        {
                            Console.WriteLine($"Sucesfully created account {Name} now you are logged!");
                            Console.WriteLine("Type help for more information!\n");
                        }
                        else
                        {
                            Console.WriteLine("Invalid login information!");
                            Console.WriteLine("Try again!");
                        }
                    }

                    creatingAccount = false;
                }
            }
        }

        private static void Login(string name, string password)
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
                }

            }
        }

        public static void AccountOptions()
        {
            Console.WriteLine("Account Options:");
            Console.WriteLine("Choose between renaming your account and changing your password with 'rename' and 'change' or 'return' to return!");
            Console.Write("You choose: ");
            string input = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (input == "rename")
            {
                Console.Write("New name: ");
                string newName = Console.ReadLine();

                if (!UserExist(newName))
                {
                    Rename(Name, newName);
                }
                else
                {
                    Console.WriteLine($"There is already user with this name: {newName}. You are returned to the options menu!");
                    AccountOptions();
                }
            } else if (input == "change")
            {
                Console.Write("Current password: ");
                string currentPassword = Console.ReadLine();
                if (currentPassword == Password)
                {
                    Console.Write("New password: ");
                    string newPassword = Console.ReadLine();
                    Console.Write("Confirm password: ");
                    if (newPassword == Console.ReadLine())
                    {
                        ChangePassword(Id, newPassword);
                    }
                    else
                    {
                        Console.WriteLine("Invalid password confirmation you are now returned to the main menu!");
                    }
                }
            }
        }

        private static void Rename(string oldName, string newName)
        {
            bool namesAreNotEmpty = false;
            if (string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(oldName))
            {
                Console.WriteLine("Invalid name!");
                namesAreNotEmpty = true;
            }

            if (namesAreNotEmpty)
            {
                using (var conn = new NpgsqlConnection(Blog.ConnectionString))
                {
                    conn.Open();

                    var database = new Database(conn);

                    var parametar = new NpgsqlParameter("n", newName);
                    bool accountExistWithThisName = database.Query<UserPoco>("SELECT * FROM users WHERE name=@n;", parametar).Count > 0;

                    if (!accountExistWithThisName)
                    {
                        var parametars = new Dictionary<string, object>()
                        {
                            {"n", newName},
                            {"i", Id}
                        };
                        database.ExecuteNonQuery("UPDATE users SET name=@n WHERE user_id=@i;", parametars);
                        database.ExecuteNonQuery("UPDATE comments SET author_name=@n WHERE user_id=@i;", parametars);

                        Login(newName,Password);
                    }
                }
            }

            Console.WriteLine("You succsefuly renamed your account!\n");
        }

        private static void ChangePassword(int userId, string newPassword)
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


        private static bool UserExist(string name)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                conn.Open();

                var database = new Database(conn);
                var parametar = new NpgsqlParameter("n", name);

                return database.Query<UserPoco>("SELECT * FROM users WHERE name=@n;", parametar).Count > 0;
            }
        }
    }
}