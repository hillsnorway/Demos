using FlooringMastery.BLL;
using FlooringMastery.Models.Responses;
using System;


namespace FlooringMastery.UI.WorkFlows
{
    public class DisplayOrderWorkFlow
    {
        internal static void Execute(DisplayMode mode)
        {
            Execute(mode, true, true);
        }

        internal static void Execute(DisplayMode mode, bool clearScreen, bool wait)
        {
            Execute(mode, clearScreen, wait, out OrderResponse or, out OrderManager om);
        }

        internal static void Execute(DisplayMode mode, bool clearScreen, bool wait, out OrderResponse response, out OrderManager myOM)
        {
            response = null;
            myOM = null;
            Console.Clear();
            if (!ConsoleIO.GetDateValue("Enter the date for your order ", null, out DateTime orderDate)) return;
            if (!ConsoleIO.GetInt("Enter the order number: ", 1, 999999, out int orderNumber)) return;
            myOM = OrderManagerFactory.Create(orderDate);
            response = myOM.GetOrder(orderNumber);
            if (response.Success) ConsoleIO.DisplayOrder(mode, response.Order, clearScreen, wait, ConsoleColor.Blue);
            else
            {
                ConsoleIO.DisplayText(response.Message + "\nPress any key to continue...", false, true, ConsoleColor.Red);
            }
        }

    }
}
