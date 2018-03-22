using SGBank.Models;
using SGBank.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SGBank.UI
{
    public class ConsoleIO
    {
        public static void DisplayText(string text, bool clearScreen, bool awaitKeypress, ConsoleColor consoleForeColor)
        {
            if (clearScreen) Console.Clear();
            Console.ForegroundColor = consoleForeColor;
            Console.WriteLine(text);
            Console.ResetColor();
            if (awaitKeypress) Console.ReadKey();
        }

        public static void DisplayText(string text, bool clearScreen, bool awaitKeypress)
        {
            DisplayText(text, clearScreen, awaitKeypress, ConsoleColor.White);
        }

        public static void DisplayText(string text, bool clearScreen)
        {
            DisplayText(text, clearScreen, false);
        }

        public static void DisplayText(string text)
        {
            DisplayText(text, false, false);
        }

        public static void DisplayAccoutDetails(Account account)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Account Number: {account.AccountNumber}");
            Console.WriteLine($"Name: {account.Name}");
            Console.WriteLine($"Balance {account.Balance:c}");
            Console.ResetColor();
        }

        public static void DisplayWithdrawlDetails(AccountWithdrawResponse response)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Withdrawl Completed!");
            Console.WriteLine($"Account Number: {response.Account.AccountNumber}");
            Console.WriteLine($"Old Balance: {response.OldBalance:c}");
            Console.WriteLine($"Amount Withdrawn: {response.Amount}");
            Console.WriteLine($"New Balance: {response.Account.Balance:c}");
            Console.ResetColor();
        }

        public static void DisplayDepositDetails(AccountDepositResponse response)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Deposit Completed!");
            Console.WriteLine($"Account Number: {response.Account.AccountNumber}");
            Console.WriteLine($"Old Balance: {response.OldBalance:c}");
            Console.WriteLine($"Amount Deposited: {response.Amount}");
            Console.WriteLine($"New Balance: {response.Account.Balance:c}");
            Console.ResetColor();
        }

        public static bool GetAccountNumber(out string accountNumber)
        {
            accountNumber = "";
            bool validAcct;
            do
            {
                Console.WriteLine("(Q to quit at any time)");
                Console.Write("Enter an account number (5 digits): ");
                accountNumber = Console.ReadLine();
                if (accountNumber.ToUpper() == "Q") return false;
                validAcct = int.TryParse(accountNumber, out int intAcctNr);
                if (!(accountNumber.Length == 5 && validAcct))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must enter a valid account number!");
                    Console.ResetColor();
                };
            } while (!(accountNumber.Length == 5 && validAcct));
            return true;
        }

        public static bool GetDecimalValue(string prompt, bool isPositive, out decimal decimalValue)
        {
            decimalValue = 0;
            string input;
            do
            {
                Console.WriteLine("(Q to quit at any time)");
                Console.Write(prompt);
                input = Console.ReadLine();
                if (input.ToUpper() == "Q") return false;
                else if (!decimal.TryParse(input, out decimalValue))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must enter a valid number!");
                    Console.ResetColor();
                }
                else if (!((decimalValue > 0 && isPositive) || (decimalValue < 0 && !isPositive)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (isPositive)
                        Console.WriteLine("You must enter a positive number!");
                    else
                        Console.WriteLine("You must enter a negative number!");
                    Console.ResetColor();
                }
            } while (!((decimalValue > 0 && isPositive) || (decimalValue < 0 && !isPositive)));
            return true;
        }
    }
}
