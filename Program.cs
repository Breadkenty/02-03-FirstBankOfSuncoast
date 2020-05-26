using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace _02_03_FirstBankOfSuncoast
{
    partial class Program
    {
        static void ShowBalance(User user)
        {
            Console.WriteLine($"Accounts: \n");
            foreach (var account in user.Accounts)
            {
                Console.WriteLine($"{account.Name}: ${account.Balance}");
            }
        }

        static void Greeting(string prompt)
        {
            Console.WriteLine($"-------------------------------");
            Console.WriteLine(prompt);
            Console.WriteLine($"-------------------------------");
        }

        static string AskForString(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        static decimal AskForDecimal(string prompt)
        {
            Console.WriteLine(prompt);

            decimal result;
            bool goodResponse = false;

            var input = Console.ReadLine();
            goodResponse = decimal.TryParse(input, out result);

            while (!goodResponse)
            {
                Console.WriteLine("Invalid Input! Enter an amount in two decimal places! eg. 10.00");
                input = Console.ReadLine();

                goodResponse = decimal.TryParse(input, out result);
            }

            return Math.Abs(result);
        }

        static char PressAKeyPrompt(string prompt)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadKey().KeyChar;
            return input;
        }

        public static void PressAnyKeyPrompt(string prompt)
        {
            Console.WriteLine(prompt);
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            TextReader reader;

            if (File.Exists("Users.csv"))
            {
                reader = new StreamReader("Users.csv");
            }
            else
            {
                reader = new StringReader("");
            }

            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var users = csvReader.GetRecords<User>().ToList();

            var userInUse = new User();
            reader.Close();
            userInUse.LoadAccount();

            // Log in phase ---------------------------------------------------------------------------

            // Ask for username and password, create an account if you don't have one
            bool accessGranted = false;

            while (!accessGranted)
            {
                var input = PressAKeyPrompt("L to login to your account | C to create a new account ");
                bool goodInput = "lc".Contains(input);
                User selectedUser;

                // User Login
                if (input == 'l')
                {
                    bool enteringUserID = true;
                    while (enteringUserID)
                    {
                        string userIDEntered = AskForString($"Enter your user name: ");
                        var userExists = users.Any(user => user.UserID == userIDEntered);

                        // If the user exists enter the password
                        if (userExists)
                        {
                            selectedUser = users.First(user => user.UserID == userIDEntered);
                            bool correctPassword = false;
                            while (!correctPassword)
                            {
                                var userPasswordEntered = AskForString($"Enter your password: ");

                                // Correct password
                                if (selectedUser.UserPassword == userPasswordEntered)
                                {
                                    PressAnyKeyPrompt($"Welcome {selectedUser.FirstName}! Press any key see your account status...");
                                    userInUse = selectedUser;
                                    correctPassword = true;
                                    enteringUserID = false;
                                    accessGranted = true;
                                }

                                // Invalid password
                                if (selectedUser.UserPassword != userPasswordEntered)
                                {
                                    PressAnyKeyPrompt("Invalid password. Press any key to try again...");
                                }
                            }
                        }

                        // If the user doesn't exist, try again
                        if (!userExists)
                        {
                            PressAnyKeyPrompt("Invalid user ID. Press any key to try again...");
                        }
                    }
                }

                // Create new user
                if (input == 'c')
                {
                    Console.WriteLine("Enter the following information:");

                    var newUser = new User()
                    {
                        UserID = AskForString("User Name: "),
                        UserPassword = AskForString("Password: "),
                        FirstName = AskForString("First Name: "),
                        LastName = AskForString("Last Name: ")
                    };

                    userInUse = newUser;

                    PressAnyKeyPrompt($"Welcome {newUser.FirstName}! Press any key see your account status...");
                    accessGranted = true;
                }

                // Invalid input
                if (!goodInput)
                {
                    Console.WriteLine($"Invalid input");
                }
            }

            // Bank transaction page ---------------------------------------------------------------------------
            bool mainMenuInUse = true;

            while (mainMenuInUse)
            {
                Console.Clear();
                Greeting($"{userInUse.FirstName}, welcome to the First Bank of Suncoast!");

                // Show Balance
                ShowBalance(userInUse);

                var input = PressAKeyPrompt("N to create a new account | B to browse your accounts | X to Exit");
                var goodInput = "nbx".Contains(input);

                // Invalid input
                if (!goodInput)
                {
                    PressAnyKeyPrompt("Invalid input!");
                }

                // N to create a new account
                if (input == 'n' || userInUse.Accounts.Count() == 0)
                {
                    PressAnyKeyPrompt("Let's set you up with a new account! Press any key to continue...");

                    var newAccount = new Account()
                    {
                        Name = AskForString("What type of account is this?"),
                        Balance = AskForDecimal("How much are you depositing to this account?")
                    };

                    userInUse.Accounts.Add(newAccount);
                    userInUse.SaveAccount();

                    PressAnyKeyPrompt($"Great we've created your {newAccount.Name} account! \nWe've deposited your check. Your current balance is ${newAccount.Balance}. \nPress any key to continue...");
                }

                // B to browse accounts
                if (input == 'b' && userInUse.Accounts.Count() > 0)
                {
                    Console.WriteLine($"\nHere are all of your accounts");
                    foreach (var account in userInUse.Accounts)
                    {
                        Console.WriteLine(account.Name);
                    }

                    bool accountMenuInUse = true;
                    while (accountMenuInUse)
                    {
                        var askedAccountString = AskForString("Which account would you to browse?");
                        var askedAccount = userInUse.Accounts.FirstOrDefault(account => account.Name == askedAccountString);
                        bool validAccount = userInUse.Accounts.Any(account => account.Name == askedAccountString);

                        // Valid account input
                        if (validAccount)
                        {
                            var accountBrowsingMenuInput = PressAKeyPrompt("D - Deposit | W - Withdraw | T - Transfer | S - Show balance | M - Main Menu ");
                            goodInput = "dwtsm".Contains(accountBrowsingMenuInput);

                            // Invalid input
                            if (!goodInput)
                            {
                                PressAnyKeyPrompt("Invalid input!");
                            }

                            // Deposit money
                            if (accountBrowsingMenuInput == 'd')
                            {
                                var depositAmount = AskForDecimal("How much would you like to deposit?");
                                askedAccount.Balance += depositAmount;
                                userInUse.SaveAccount();

                                Console.WriteLine($"${depositAmount} has been deposited to the account: {askedAccount.Name}.\nYour current balance is ${askedAccount.Balance}");
                                PressAnyKeyPrompt($"Press any key to continue...");
                            }

                            // Withdraw money
                            if (accountBrowsingMenuInput == 'w' && askedAccount.Balance > 0)
                            {
                                var withdrawAmount = AskForDecimal("How much would you like to withdraw?");

                                if (withdrawAmount > askedAccount.Balance)
                                {
                                    PressAnyKeyPrompt("Unavailable to withdraw, insufficient funds. Press any key to continue...");
                                }

                                if (withdrawAmount <= askedAccount.Balance)
                                {
                                    askedAccount.Balance -= withdrawAmount;
                                    userInUse.SaveAccount();

                                    Console.WriteLine($"${withdrawAmount} has been deposited to the account: {askedAccount.Name}.\nYour current balance is ${askedAccount.Balance}");
                                    PressAnyKeyPrompt($"Press any key to continue...");
                                }
                            }

                            if (accountBrowsingMenuInput == 'w' && askedAccount.Balance == 0)
                            {
                                PressAnyKeyPrompt("You do not have any funds to withdraw. Deposit some money first! Press any key to continue...");
                            }

                            // Transfer money to another account
                            if (accountBrowsingMenuInput == 't' && userInUse.Accounts.Count() > 1)
                            {
                                Console.WriteLine($"-----------------------");
                                Console.WriteLine($"Your accounts: \n");
                                foreach (var account in userInUse.Accounts)
                                {
                                    Console.WriteLine(account.Name);
                                }
                                Console.WriteLine($"-----------------------");

                                var transferToString = AskForString("Which account did you want to transfer to?");
                                var transferTo = userInUse.Accounts.FirstOrDefault(account => account.Name == transferToString);

                                var transferAmount = AskForDecimal("How much would you like to transfer?");

                                if (transferAmount > askedAccount.Balance)
                                {
                                    PressAnyKeyPrompt("Unavailable to transfer, insufficient funds. Press any key to continue...");
                                }

                                if (transferAmount < askedAccount.Balance)
                                {
                                    askedAccount.Balance -= transferAmount;
                                    transferTo.Balance += transferAmount;
                                    userInUse.SaveAccount();

                                    Console.WriteLine($"${transferAmount} has been deposited to the account: {askedAccount.Name}.\nYour current balance is ${askedAccount.Balance}");
                                    PressAnyKeyPrompt($"Press any key to continue...");
                                }
                            }

                            // No account to transfer to
                            if (accountBrowsingMenuInput == 't' && userInUse.Accounts.Count() == 1)
                            {
                                PressAnyKeyPrompt("You don't have another account to transfer to. Please create another account. Press any key to continue...");
                            }

                            // Show balance
                            if (accountBrowsingMenuInput == 's')
                            {
                                Console.WriteLine($"{askedAccount.Name}: ${askedAccount.Balance}");
                                PressAnyKeyPrompt($"\nPress any key to continue...");
                            }

                            // Main Menu
                            if (accountBrowsingMenuInput == 'm')
                            {
                                accountMenuInUse = false;
                            }
                        }

                        if (!validAccount)
                        {
                            PressAnyKeyPrompt("Invalid account name! Please enter the account name. Press any key to continue...");
                        }
                    }
                }

                if (input == 'x')
                {
                    PressAnyKeyPrompt("Thank you for banking with the First Bank of Suncoast! Press any key to exit...");
                    mainMenuInUse = false;
                }
            }

            foreach (var user in users)
            {
                Console.WriteLine($"{userInUse.FirstName}: ");

                foreach (var account in userInUse.Accounts)
                {
                    Console.WriteLine($"{account.Name}: ${account.Balance}");

                    PressAnyKeyPrompt("Press any key to continue");
                }
            }

            users.Add(userInUse);
            var fileWriter = new StreamWriter("Users.csv");
            var csvWriter = new CsvWriter(fileWriter, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(users);
            fileWriter.Close();
        }
    }
}
