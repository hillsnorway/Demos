using Ninject;
using SGBank.BLL;
using SGBank.Models.Responses;
using System;


namespace SGBank.UI.Workflows
{
    public class DepositWorkFlow
    {
        public void Execute()
        {
            string consoleText, accountNumber;
            decimal amount;

            //AccountManager accountManager = AccountManagerFactory.Create();
            AccountManager accountManager = DIContainer.Kernel.Get<AccountManager>();
            consoleText = "Deposit to an account\n" + "---------------------";
            ConsoleIO.DisplayText(consoleText, true);
            consoleText = "Enter a deposit amount: ";
            if (!ConsoleIO.GetAccountNumber(out accountNumber)) return;
            if (!ConsoleIO.GetDecimalValue(consoleText, true, out amount)) return;
            AccountDepositResponse response = accountManager.Deposit(accountNumber, amount);

            if (response.Success)
            {
                ConsoleIO.DisplayDepositDetails(response);
            }
            else
            {
                consoleText = "An error occurred:\n" + response.Message;
                ConsoleIO.DisplayText(consoleText, false, false, ConsoleColor.Red);
            }
            ConsoleIO.DisplayText("Press any key to continue...", false, true);
        }
    }
}
