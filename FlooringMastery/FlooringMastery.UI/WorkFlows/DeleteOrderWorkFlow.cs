using FlooringMastery.BLL;
using FlooringMastery.Models.Responses;
using System;


namespace FlooringMastery.UI.WorkFlows
{
    public class DeleteOrderWorkFlow
    {
        internal static void Execute(DisplayMode mode)
        {
            int orderNumberDeleted;

            DisplayOrderWorkFlow.Execute(mode, true, false, out OrderResponse response, out OrderManager myOM);
            orderNumberDeleted = response.Order.OrderNumber;
            if (!ConsoleIO.GetString("Type (D/d) to delete this order: ", 1, out string input)) return;
            if (input.ToUpper() == "D")
            {
                response = myOM.DeleteOrder(response.Order.OrderNumber);
                if (response.Success)
                {
                    ConsoleIO.DisplayText("Order deleted! \nPress any key to continue...", false, true);
                }
                else
                {
                    ConsoleIO.DisplayText(response.Message + "\nPress any key to continue...", false, true, ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleIO.DisplayText("Order NOT deleted! \nPress any key to continue...", false, true);
            }
        }
    }
}
