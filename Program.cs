using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace _02_03_FirstBankOfSuncoast
{

    class Transaction
    {
        public string Type { get; set; }
        public string Account { get; set; }
        public decimal Amount { get; set; }
    }


    partial class Program
    {
        private static decimal TransferSum(List<Transaction> transactions, string accountTransferOut)
        {
            var transferTransactions = transactions.Where(transaction => transaction.Account == accountTransferOut && transaction.Type == "Transfer");
            var transferTransactionSum = transferTransactions.Sum(transaction => transaction.Amount);

            return transferTransactionSum;
        }
        private static decimal CalculateAccountBalance(List<Transaction> transactions, string account, string otherAccount)
        {
            var accountDepositTransactions = transactions.Where(transaction => transaction.Account == account && transaction.Type == "Deposit");
            var accountDepositTransactionSum = accountDepositTransactions.Sum(transaction => transaction.Amount);

            var accountWithdrawTransactions = transactions.Where(transaction => transaction.Account == account && transaction.Type == "Withdraw");
            var accountWithdrawTransactionSum = accountWithdrawTransactions.Sum(transaction => transaction.Amount);

            var balance = accountDepositTransactionSum - accountWithdrawTransactionSum;
            balance = balance - TransferSum(transactions, otherAccount);

            return balance - TransferSum(transactions, account);
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
            decimal result = 0.00m;
            bool goodInput = false;

            while (!goodInput)
            {
                Console.WriteLine(prompt);

                var input = Console.ReadLine();
                goodInput = decimal.TryParse(input, out result);

                // Invalid input: not a number
                if (!goodInput)
                {
                    PressAnyKeyPrompt("Invalid input! Not a number. Press any key to continue...");
                }

                // Invalid input: not a positive number
                if (goodInput && result < 0)
                {
                    PressAnyKeyPrompt("Invalid input! Not a positive number. Press any key to continue...");
                    goodInput = false;
                }
            }

            return result;
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

            if (File.Exists("Transactions.csv"))
            {
                reader = new StreamReader("Transactions.csv");
            }
            else
            {
                reader = new StringReader("");
            }

            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var transactions = csvReader.GetRecords<Transaction>().ToList();

            reader.Close();

            var users = new List<User>();
            var userInUse = new User();

            var user1 = new User()
            {
                UserID = "Kento726",
                UserPassword = "1234",
                FirstName = "Kento",
                LastName = "Kawakami"
            };

            var user2 = new User()
            {
                UserID = "Kodak51",
                UserPassword = "12345",
                FirstName = "Kodak",
                LastName = "Kawakami"
            };

            var user3 = new User()
            {
                UserID = "Breadkenty",
                UserPassword = "123456",
                FirstName = "Bread",
                LastName = "Kenty"
            };

            var user4 = new User()
            {
                UserID = "CodeMaster1337",
                UserPassword = "1337",
                FirstName = "Gavin",
                LastName = "Stark"
            };
            users.Add(user1);
            users.Add(user2);
            users.Add(user3);
            users.Add(user4);

            // var transactions = new List<Transaction>();

            var deposit1 = new Transaction()
            {
                Type = "Deposit",
                Amount = 10000.00m,
                Account = "Checking",
            };
            var deposit2 = new Transaction()
            {
                Type = "Deposit",
                Amount = 4869.00m,
                Account = "Saving",
            };
            var withdraw1 = new Transaction()
            {
                Type = "Withdraw",
                Amount = 1234m,
                Account = "Checking",
            };
            var withdraw2 = new Transaction()
            {
                Type = "Withdraw",
                Amount = 1234m,
                Account = "Saving",
            };

            transactions.Add(deposit1);
            transactions.Add(deposit2);
            transactions.Add(withdraw1);
            transactions.Add(withdraw2);

            // // Log in phase ---------------------------------------------------------------------------

            // Ask for username and password, create an account if you don't have one
            bool accessGranted = false;
            User userInOperation = null;

            while (!accessGranted)
            {
                var input = PressAKeyPrompt("L to login to your account | C to create a new account ");
                bool goodInput = "lc".Contains(input);

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
                            userInOperation = users.First(user => user.UserID == userIDEntered);

                            bool correctPassword = false;

                            while (!correctPassword)
                            {
                                var userPasswordEntered = AskForString($"Enter your password: ");

                                // Correct password
                                if (userInOperation.UserPassword == userPasswordEntered)
                                {
                                    PressAnyKeyPrompt($"Welcome {userInOperation.FirstName}! Press any key see your account status...");

                                    correctPassword = true;
                                    enteringUserID = false;
                                    accessGranted = true;
                                }

                                // Invalid password
                                if (userInOperation.UserPassword != userPasswordEntered)
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

                    userInOperation = newUser;

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
                Greeting($"{userInOperation.FirstName}, welcome to the First Bank of Suncoast!");

                var checkingAccount = transactions.Where(transaction => transaction.Account == "Checking");
                var savingAccount = transactions.Where(transaction => transaction.Account == "Saving");

                var mainMenuInput = PressAKeyPrompt("D - Deposit | W - Withdraw | T - Transfer | S - Show balance | X - Exit");
                var goodInput = "dwtsx".Contains(mainMenuInput);

                // Invalid input
                if (!goodInput)
                {
                    PressAnyKeyPrompt("Invalid input!");
                }

                // Deposit money
                if (mainMenuInput == 'd')
                {
                    bool depositInUse = true;

                    while (depositInUse)
                    {
                        var depositMenuInput = PressAKeyPrompt("Deposit to:\nC - Checking | S - Saving | X - Finish Deposit");
                        var goodDepositInput = "csx".Contains(depositMenuInput);

                        if (depositMenuInput == 'c')
                        {
                            var depositAmount = AskForDecimal("How much do you want to deposit into your checking account?");

                            var newDepositTransaction = new Transaction()
                            {
                                Type = "Deposit",
                                Account = "Checking",
                                Amount = depositAmount
                            };

                            transactions.Add(newDepositTransaction);

                            PressAnyKeyPrompt($"You deposited ${depositAmount} to your Checking account.\nPress any key to continue...");
                        }

                        if (depositMenuInput == 's')
                        {
                            var depositAmount = AskForDecimal("How much do you want to deposit into your saving account?");

                            var newDepositTransaction = new Transaction()
                            {
                                Type = "Deposit",
                                Account = "Saving",
                                Amount = depositAmount
                            };

                            transactions.Add(newDepositTransaction);

                            PressAnyKeyPrompt($"You deposited ${depositAmount} to your Saving account.\nPress any key to continue...");
                        }

                        if (depositMenuInput == 'x')
                        {
                            PressAnyKeyPrompt("Entering Main Menu.\nPress any key to continue...");
                            depositInUse = false;
                        }

                        if (!goodDepositInput)
                        {
                            PressAnyKeyPrompt("Invalid input!");
                        }
                    }
                }

                // Withdraw money
                if (mainMenuInput == 'w')
                {
                    bool withdrawInUse = true;

                    while (withdrawInUse)
                    {
                        var withdrawMenuInput = PressAKeyPrompt("Withdraw from:\nC - Checking | S - Saving | X - Finish Withdraw");
                        var goodWithdrawInput = "csx".Contains(withdrawMenuInput);

                        if (withdrawMenuInput == 'c')
                        {
                            var withdrawAmount = AskForDecimal("How much do you want to withdraw from your checking account?");

                            var newWithdrawTransaction = new Transaction()
                            {
                                Type = "Withdraw",
                                Account = "Checking",
                                Amount = withdrawAmount
                            };

                            transactions.Add(newWithdrawTransaction);

                            PressAnyKeyPrompt($"You've withdrawn ${withdrawAmount} from your Checking account.\nPress any key to continue...");
                        }

                        if (withdrawMenuInput == 's')
                        {
                            var withdrawAmount = AskForDecimal("How much do you want to withdraw into your saving account?");

                            var newWithdrawTransaction = new Transaction()
                            {
                                Type = "Withdraw",
                                Account = "Saving",
                                Amount = withdrawAmount
                            };

                            transactions.Add(newWithdrawTransaction);

                            PressAnyKeyPrompt($"You've withdrawn ${withdrawAmount} from your Saving account.\nPress any key to continue...");
                        }

                        if (withdrawMenuInput == 'x')
                        {
                            PressAnyKeyPrompt("Entering Main Menu.\nPress any key to continue...");
                            withdrawInUse = false;
                        }

                        if (!goodWithdrawInput)
                        {
                            PressAnyKeyPrompt("Invalid input!");
                        }
                    }
                }

                // Transfer money to another account
                if (mainMenuInput == 't')
                {
                    bool transferInUse = true;

                    while (transferInUse)
                    {
                        var transferMenuInput = PressAKeyPrompt("Transfer from:\nC - Checking | S - Saving | X - Finish transfer");
                        var goodTransferInput = "csx".Contains(transferMenuInput);

                        if (transferMenuInput == 'c')
                        {
                            var transferAmount = AskForDecimal("How much do you want to transfer from your checking account to your savings account?");

                            var newTransferTransaction = new Transaction()
                            {
                                Type = "Transfer",
                                Account = "Checking",
                                Amount = transferAmount
                            };

                            transactions.Add(newTransferTransaction);
                            PressAnyKeyPrompt($"You've transferred ${transferAmount} from your Checking account.\nPress any key to continue...");
                        }

                        if (transferMenuInput == 's')
                        {
                            var transferAmount = AskForDecimal("How much do you want to transfer from your savings account to your checking account?");

                            var newTransferTransaction = new Transaction()
                            {
                                Type = "Transfer",
                                Account = "Saving",
                                Amount = transferAmount
                            };

                            transactions.Add(newTransferTransaction);
                            PressAnyKeyPrompt($"You've transferred ${transferAmount} from your Checking account.\nPress any key to continue...");
                        }

                        if (transferMenuInput == 'x')
                        {
                            PressAnyKeyPrompt("Entering main menu.\nPress any key to continue...");

                            transferInUse = false;
                        }

                        if (!goodTransferInput)
                        {
                            PressAnyKeyPrompt("Invalid input!");
                        }
                    }
                }

                // Show balance
                if (mainMenuInput == 's')
                {
                    var checkingBalance = CalculateAccountBalance(transactions, "Checking", "Saving");
                    var savingBalance = CalculateAccountBalance(transactions, "Saving", "Checking");

                    Console.WriteLine($"Checking: ${checkingBalance}");

                    Console.WriteLine($"Saving: ${savingBalance}");

                    PressAnyKeyPrompt($"\nHere are your account balances.\nPress any key to continue...");
                }

                if (mainMenuInput == 'x')
                {
                    PressAnyKeyPrompt("Thank you for banking with the First Bank of Suncoast! Press any key to exit...");
                    mainMenuInUse = false;
                }
            }

            var fileWriter = new StreamWriter("Transactions.csv");
            var csvWriter = new CsvWriter(fileWriter, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(transactions);
            fileWriter.Close();
        }
    }
}
