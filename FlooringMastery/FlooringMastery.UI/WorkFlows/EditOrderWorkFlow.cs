using FlooringMastery.BLL;
using FlooringMastery.Models;
using FlooringMastery.Models.Responses;
using System;
using System.Configuration;
using System.Linq;


namespace FlooringMastery.UI.WorkFlows
{
    public class EditOrderWorkFlow
    {
        private static int _minArea = int.Parse(ConfigurationManager.AppSettings["MinArea"]);
        private static int _maxArea = int.Parse(ConfigurationManager.AppSettings["MaxArea"]);
        private static int _maxCustNameLength = int.Parse(ConfigurationManager.AppSettings["MaxCustNameLength"]);

        internal static void Execute(DisplayMode mode)
        {
            DisplayOrderWorkFlow.Execute(mode, true, false, out OrderResponse response, out OrderManager myOM);

            if (!response.Success)
            {
                return;
            }
            string input;
            if (!ConsoleIO.GetString("Type (E/e) to edit this order: ", 1, out input)) return;
            if (input.ToUpper() == "E")
            {
                Order editedOrder = new Order()
                {
                    OrderDate = response.Order.OrderDate,
                    OrderNumber = response.Order.OrderNumber,
                    OrderStateTax = response.Order.OrderStateTax,
                    OrderProduct = response.Order.OrderProduct,
                    CustomerName = response.Order.CustomerName,
                    Area = response.Order.Area,
                    FileTotal = response.Order.FileTotal,
                    FileTax = response.Order.FileTax,
                    FileMaterialCost = response.Order.FileMaterialCost,
                    FileLaborCost = response.Order.FileLaborCost,
                };

                ConsoleIO.DisplayText("OK, you are now in Edit mode.", false, false, ConsoleColor.DarkYellow);
                //Customer Name
                if (!ConsoleIO.GetString($"Enter a new customer name, or press Enter to keep ({editedOrder.CustomerName}): ", _maxCustNameLength, out input)) return;
                if (input == "")
                {
                    //User pressed Enter
                }
                else
                {
                    if (input.Length > _maxCustNameLength || !(input.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.' || c == ',')))
                    {
                        bool loop;
                        do
                        {
                            loop = false;
                            ConsoleIO.DisplayText("That name was invalid. \nOnly Characters [A-Z][a-z][0-9][,][.][ ] are allowed.", false, false, ConsoleColor.Red);
                            if (!ConsoleIO.GetString($"Enter the customer name (max {_maxCustNameLength} characters): ", _maxCustNameLength, out input)) return;
                            if (input == "" || input.Length > _maxCustNameLength || !(input.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.' || c == ','))) loop = true;
                        } while (loop);
                    }
                    else
                    {
                        editedOrder.CustomerName = input;
                    }
                }

                //State
                TaxManager myTM = TaxManagerFactory.Create();
                TaxesResponse responseTs = myTM.GetTaxes();
                if (!ConsoleIO.GetString($"Enter a new state (f.ex. {responseTs.Taxes[0].StateCode}), or press Enter to keep ({editedOrder.OrderStateTax.StateCode}): ", 2, out input)) return;
                if (input == "")
                {
                    //User pressed Enter
                }
                else
                {
                    TaxResponse responseT = myTM.GetTaxByState(input.ToUpper ());
                    if (!responseT.Success)
                    {
                        ConsoleIO.DisplayText("That was not a valid state.", false, false, ConsoleColor.Red);
                        ConsoleIO.DisplayTaxes(responseTs.Taxes, false, false, ConsoleColor.Blue);
                        bool loop;
                        do
                        {
                            loop = false;
                            if (!ConsoleIO.GetString($"Enter a state (f.ex. {responseTs.Taxes[0].StateCode}) from the list: ", 2, out input)) return;
                            responseT = myTM.GetTaxByState(input.ToUpper());
                            if (!responseT.Success)
                            {
                                ConsoleIO.DisplayText("That was not a valid state.", false, false, ConsoleColor.Red);
                                loop = true;
                            }
                        } while (loop);
                        editedOrder.OrderStateTax = responseT.StateTax;
                    }
                    else editedOrder.OrderStateTax = responseT.StateTax;
                }

                //Product
                if (!ConsoleIO.GetString($"Enter a new product type, or press Enter to keep the current one ({editedOrder.OrderProduct.ProductType}): ", 255, out input)) return;
                if (input == "")
                {
                    //User pressed Enter
                }
                else
                {
                    ProductManager myPM = ProductManagerFactory.Create();
                    ProductResponse responseP = myPM.GetProductByType(input.ToUpper());
                    if (!responseP.Success)
                    {
                        ConsoleIO.DisplayText("That was not a valid product type.", false, false, ConsoleColor.Red);
                        ProductsResponse responsePs = myPM.GetProducts();
                        ConsoleIO.DisplayProducts(responsePs.Products, false, false, ConsoleColor.Blue);
                        bool loop;
                        do
                        {
                            loop = false;
                            if (!ConsoleIO.GetString("Enter the product type from the list wish to select: ", 255, out input)) return;
                            responseP = myPM.GetProductByType(input.ToUpper());
                            if (!responseP.Success)
                            {
                                ConsoleIO.DisplayText("That was not a valid product type.",false,false,ConsoleColor.Red);
                                loop = true;
                            }
                        } while (loop);
                        editedOrder.OrderProduct = responseP.Product;
                    }
                    else editedOrder.OrderProduct = responseP.Product;
                }

                //Area
                decimal decimalArea = editedOrder.Area;
                if (!ConsoleIO.GetString($"Enter a new area (>={_minArea} ft²), or press Enter to keep the current area ({editedOrder.Area}): ", 255, out input)) return;
                if (input == "")
                {
                    //User pressed Enter
                }
                else
                {
                    if (!(decimal.TryParse(input, out decimalArea)) || decimalArea < _minArea || decimalArea > _maxArea)
                    {
                        if (!ConsoleIO.GetDecimalValue($"Enter a valid Area (>={_minArea} ft²) : ", true, _minArea, _maxArea, out decimalArea)) return;
                        editedOrder.Area = decimalArea;
                    }
                    else
                    {
                        editedOrder.Area = decimalArea;
                    }
                }

                editedOrder.Recalculate();

                //Display Confirmation of Order Changes
                Console.Clear();
                ConsoleIO.DisplayText("Original order:", false, false, ConsoleColor.Blue);
                ConsoleIO.DisplayOrder(mode, response.Order, false, false, ConsoleColor.Blue);
                ConsoleIO.DisplayText("Edited order:", false, false, ConsoleColor.DarkYellow);
                ConsoleIO.DisplayOrder(mode, editedOrder, false, false, ConsoleColor.DarkYellow);

                //Prompt for Saving
                if (!ConsoleIO.GetString("Press (Y/y) if you wish to save the changes: ", 1, out input)) return;
                if (input.ToUpper() == "Y")
                {
                    OrderResponse editResponse = new OrderResponse();
                    editResponse = myOM.EditOrder(editedOrder);
                    if (editResponse.Success)
                        ConsoleIO.DisplayText("Changes were saved. \nPress any key to continue...", false, true);
                    else
                        ConsoleIO.DisplayText(editResponse.Message + "\nPress any key to continue...", false, true, ConsoleColor.Red);
                }
                else
                {
                    ConsoleIO.DisplayText("Changes were NOT saved. \nPress any key to continue...", false, true);
                }
            }
        }
    }
}
