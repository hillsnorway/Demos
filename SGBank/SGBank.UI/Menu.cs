using SGBank.UI.Workflows;
using System;
 

namespace SGBank.UI
{
    public class Menu
    {
        public static void Start()
        {
            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("SG Bank Application");
                    Console.WriteLine("-----------------------");
                    Console.WriteLine("1. Lookup an Account.");
                    Console.WriteLine("2. Deposit");
                    Console.WriteLine("3. Withdraw");

                    Console.WriteLine("\nQ to quit");
                    Console.WriteLine("\nEnter selection: ");

                    string userInput = Console.ReadLine().ToUpper();

                    switch (userInput)
                    {
                        case "1":
                            AccountLookupWorkflow lookupWorkflow = new AccountLookupWorkflow();
                            lookupWorkflow.Execute();
                            break;
                        case "2":
                            DepositWorkFlow depositWorkflow = new DepositWorkFlow();
                            depositWorkflow.Execute();
                            break;
                        case "3": //WithdrawWorkflow
                            WithdrawWorkflow withdrawWorkflow = new WithdrawWorkflow();
                            withdrawWorkflow.Execute();
                            break;
                        case "Q":
                            return;
                        default:
                            //Invalid Input
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                //Just being funny...
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n========== Oppsie, I tooted! ==========\n");
                //Print out the exception info to the user
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("=======================================================================================================================");
                Console.WriteLine($"The error happened:\n{ex.StackTrace}");
                Console.WriteLine("=======================================================================================================================");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
