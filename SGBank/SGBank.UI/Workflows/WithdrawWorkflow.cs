using Ninject;
using SGBank.BLL;
using SGBank.Models.Responses;
using System;

namespace SGBank.UI.Workflows
{
    class WithdrawWorkflow
    {
        public void Execute()
        {
            string consoleText, accountNumber;
            decimal amount;

            //AccountManager accountManager = AccountManagerFactory.Create();
            AccountManager accountManager = DIContainer.Kernel.Get<AccountManager>();
            consoleText = "Withdraw from an account\n" + "------------------------";
            ConsoleIO.DisplayText(consoleText, true);
            consoleText = "Enter a withdrawl amount (must be negative): ";
            if (!ConsoleIO.GetAccountNumber(out accountNumber)) return;
            if (!ConsoleIO.GetDecimalValue(consoleText, false, out amount)) return;
            AccountWithdrawResponse response = accountManager.Withdraw(accountNumber, amount);

            if (response.Success)
            {
                ConsoleIO.DisplayWithdrawlDetails(response);
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
