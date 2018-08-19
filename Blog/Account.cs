using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
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
                if (Login(name,password))
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
                using (var conn = new NpgsqlConnection(Blog.ConnectionString))
                {
                    var database = new Database(conn);
                    var service = new Service(database);
                    if (!service.UserExist(name))
                    {
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
                            var user = service.RegisterUser(name, password);
                            if (Login(user.Name, user.Password))
                            {
                                Console.WriteLine($"Sucesfully created account {Name} now you are logged!");
                                Console.WriteLine("Type help for more information!\n");
                            }
                            creatingAccount = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("This name is already taken try another one!");
                    }
                }
            }
        }

        private static bool Login(string name, string password)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                var user = service.Login(name, password);

                if (user != null)
                {
                    Name = user.Name;
                    Password = user.Password;
                    Id = user.UserId;
                    Logged = true;
                    return true;
                }
            }

            return false;
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
                string newName = Console.ReadLine()?.Trim();
                using (var conn = new NpgsqlConnection(Blog.ConnectionString))
                {
                    var database = new Database(conn);
                    var service = new Service(database);
                    if (!service.UserExist(newName))
                    {
                        if (!string.IsNullOrEmpty(newName) && !string.IsNullOrWhiteSpace(newName))
                        {
                            Rename(newName);
                            Console.WriteLine(newName);
                        }
                        else
                        {
                            Console.WriteLine("New name must not be whitespace or empty!");
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            $"There is already user with this name: {newName}. You are returned to the options menu!");
                        AccountOptions();
                    }
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

        private static void Rename(string newName)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                using (var conn = new NpgsqlConnection(Blog.ConnectionString))
                {
                    var database = new Database(conn);
                    var service = new Service(database);
                    service.Rename(Id, Password, newName);
                }

                Login(newName, Password);
                Console.WriteLine("You succsefuly renamed your account!\n");
            }
            else
            {
                Console.WriteLine("Invalid name!");
            }
        }

        private static void ChangePassword(int userId, string newPassword)
        {
            using (var conn = new NpgsqlConnection(Blog.ConnectionString))
            {
                var database = new Database(conn);
                var service = new Service(database);
                service.ChangePassword(userId, newPassword, Name);
            }

            Console.WriteLine("You successfully changed your password!\n");
        }

        public static UserPoco AccountToUserPoco()
        {
            var userPoco = new UserPoco {Name=Account.Name,Password = Account.Password,UserId = Account.Id};
            return userPoco;
        }
    }
}