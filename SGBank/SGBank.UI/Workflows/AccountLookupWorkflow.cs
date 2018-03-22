using Ninject;
using SGBank.BLL;
using SGBank.Models.Responses;
using System;


namespace SGBank.UI.Workflows
{
    public class AccountLookupWorkflow
    {
        public void Execute()
        {
            string consoleText, accountNumber;

            //NRH NINJECT
            //AccountManager accountManager = AccountManagerFactory.Create();
            AccountManager accountManager = DIContainer.Kernel.Get<AccountManager>();
            consoleText = "Lookup an account\n" + "-----------------";
            ConsoleIO.DisplayText(consoleText, true);
            if (!ConsoleIO.GetAccountNumber(out accountNumber)) return;
            AccountLookupResponse response = accountManager.LookupAccount(accountNumber);

            if (response.Success)
            {
                ConsoleIO.DisplayAccoutDetails(response.Account);
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
