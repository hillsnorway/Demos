using FlooringMastery.BLL;
using FlooringMastery.Models.Responses;
using System;


namespace FlooringMastery.UI.WorkFlows
{
    public class DisplayOrdersWorkFlow
    {
        internal static void Execute(DisplayMode mode)
        {
            Console.Clear();
            if (!ConsoleIO.GetDateValue("Enter the date you wish to display orders for: ", null, out DateTime orderDate)) return;
            OrderManager myOM = OrderManagerFactory.Create(orderDate);
            OrdersResponse responseOs = myOM.GetOrders();
            if (responseOs.Success)
                ConsoleIO.DisplayOrders(mode, responseOs.Orders, ConsoleColor.Blue);
            else
            {
                ConsoleIO.DisplayText(responseOs.Message, false, false, ConsoleColor.Red);
                if (responseOs.Orders.Count > 2) //Must have more than just header = 1 order to have other lines to display.
                {
                    if (!ConsoleIO.GetString($"Press (Y/y) if you wish to display other orders despite this error.", 1, out string input)) return;
                    if (input.ToUpper() == "Y") ConsoleIO.DisplayOrders(mode, responseOs.Orders, ConsoleColor.Blue);
                }
                else
                {
                    ConsoleIO.DisplayText("Press any key to continue...", false, true);
                }
            }
        }
    }
}
