using FlooringMastery.BLL;
using FlooringMastery.Models;
using FlooringMastery.Models.Responses;
using System;
using System.Configuration;
using System.Linq;


namespace FlooringMastery.UI.WorkFlows
{
    public class AddOrderWorkFlow
    {
        private static int _minArea = int.Parse(ConfigurationManager.AppSettings["MinArea"]);
        private static int _maxArea = int.Parse(ConfigurationManager.AppSettings["MaxArea"]);
        private static int _maxCustNameLength = int.Parse(ConfigurationManager.AppSettings["MaxCustNameLength"]);

        internal static void Execute(DisplayMode mode)
        {
            Order newOrder = new Order();

            ConsoleIO.DisplayText("===== Add Order =============================================", true, false);
            if (!ConsoleIO.GetDateValue("Enter the date you wish create an order for: ", DateTime.Now.Date, out DateTime orderDate)) return;
            newOrder.OrderDate = orderDate;

            //Customer Name
            string customerName;
            bool loop;
            do
            {
                loop = false;
                if (!ConsoleIO.GetString($"Enter the customer name (max {_maxCustNameLength} characters): ", _maxCustNameLength, out customerName)) return;
                //if (!(customerName.All(c => char.IsLetterOrDigit(c) || c == '.' || c == ',')))
                if (customerName == "" || customerName.Length > _maxCustNameLength || !(customerName.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.' || c == ',')))
                {
                    ConsoleIO.DisplayText("The name entered contains invalid characters. \nOnly Characters [A-Z][a-z][0-9][,][.][ ] are allowed!", false, false, ConsoleColor.Red);
                    loop = true;
                }
            } while (loop);
            newOrder.CustomerName = customerName;

            //State
            String input;
            TaxManager myTM = TaxManagerFactory.Create();
            TaxesResponse responseTs = myTM.GetTaxes();
            ConsoleIO.DisplayTaxes(responseTs.Taxes, false, false, ConsoleColor.Blue);
            TaxResponse responseT = new TaxResponse();
            do
            {
                loop = false;
                if (!ConsoleIO.GetString($"Enter a state (f.ex. {responseTs.Taxes[0].StateCode}) from the list: ", 2, out input)) return;
                responseT = myTM.GetTaxByState(input);
                if (!responseT.Success)
                {
                    ConsoleIO.DisplayText("That was not a valid state.", false, false, ConsoleColor.Red);
                    loop = true;
                }
            } while (loop);
            newOrder.OrderStateTax = responseT.StateTax;

            //Product
            ProductManager myPM = ProductManagerFactory.Create();
            ProductsResponse responsePs = myPM.GetProducts();
            ConsoleIO.DisplayProducts(responsePs.Products, false, false, ConsoleColor.Blue);
            ProductResponse responseP = new ProductResponse();
            do
            {
                loop = false;
                if (!ConsoleIO.GetString("Enter the product type from the list wish to select: ", 255, out input)) return;
                responseP = myPM.GetProductByType(input);
                if (!responseP.Success)
                {
                    ConsoleIO.DisplayText("That was not a valid product type.", false, false, ConsoleColor.Red);
                    loop = true;
                }
            } while (loop);
            newOrder.OrderProduct = responseP.Product;

            //Area
            if (!ConsoleIO.GetDecimalValue($"Enter a valid Area (>={_minArea} ft²) : ", true, _minArea, _maxArea, out decimal decimalArea)) return;
            newOrder.Area = decimalArea;

            newOrder.Recalculate();

            //Display Confirmation of Order Changes
            Console.Clear();
            ConsoleIO.DisplayText("New order:", false, false, ConsoleColor.DarkYellow);
            ConsoleIO.DisplayOrder(mode, newOrder, false, false, ConsoleColor.DarkYellow);

            //Prompt for Saving
            if (!ConsoleIO.GetString($"Press (Y/y) if you wish to save this new order.", 1, out input)) return;
            if (input.ToUpper() == "Y")
            {
                OrderManager myOM = OrderManagerFactory.Create(orderDate);
                OrderResponse response = new OrderResponse();
                response = myOM.AddOrder(newOrder);
                if (response.Success)
                {
                    ConsoleIO.DisplayText("The order was saved. \nPress any key to continue...", false, true);
                }
                else
                {
                    ConsoleIO.DisplayText(response.Message + "\nPress any key to continue...", false, true, ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleIO.DisplayText("The order was NOT saved. \nPress any key to continue...", false, true);
            }
        }
    }
}
