using FlooringMastery.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace FlooringMastery.UI
{
    public class ConsoleIO
    {
        const string constDivLine = "=============================================================";
        const string constMidLine = "|===========================================================|";
        static string DataFileDateParseFormat = ConfigurationManager.AppSettings["DataFileDateParseFormat"].ToString();
        static string UIDateParseFormat = ConfigurationManager.AppSettings["UIDateParseFormat"].ToString();

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

        public static bool GetString(string prompt, int maxLength, out string stringValue)
        {
            stringValue = "";
            do
            {
                Console.WriteLine("(Q to quit at any time)");
                Console.Write(prompt);
                stringValue = Console.ReadLine();
                if (stringValue.ToUpper() == "Q") return false;
                else if (stringValue.Length > maxLength)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The text can only be {maxLength} characters long!");
                    Console.ResetColor();
                }
            } while (stringValue.Length > maxLength);
            return true;
        }

        public static bool GetInt(string prompt, int minInt, int maxInt, out int intNr)
        {
            intNr = 0;
            string input;
            bool validInt, loop;

            if (minInt < -2147483647 || maxInt > 2147483647 || minInt > maxInt)
            {
                //improper use of min/max parameters - ints must be less than +/-2147483647
                return false;
            }

            do
            {
                loop = false;
                Console.WriteLine("(Q to quit at any time)");
                Console.Write(prompt);
                input = Console.ReadLine();
                if (input.ToUpper() == "Q") return false;
                validInt = int.TryParse(input, out intNr);
                if (!validInt)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The number must be a valid integer!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (minInt == maxInt && intNr != minInt)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The only valid number is {minInt})!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (!(intNr >= minInt && intNr <= maxInt))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The number must have between {minInt} and {maxInt}!");
                    Console.ResetColor();
                    loop = true;
                }

            } while (loop);
            return true;
        }

        public static bool GetIntMaxDigits(string prompt, int minDigits, int maxDigits, out int intNr)
        {
            intNr = 0;
            string input;
            bool validInt, loop;

            if (minDigits <= 0 || maxDigits <= 0 || maxDigits > 10 || minDigits > maxDigits)
            {
                //improper use of min/max parameters - ints must be less than +/-2147483647
                return false;
            } 
            
            do
            {
                loop = false;
                Console.WriteLine("(Q to quit at any time)");
                Console.Write(prompt);
                input = Console.ReadLine();
                if (input.ToUpper() == "Q") return false;
                validInt = int.TryParse(input, out intNr);
                if (input.Length > 0 && input.Substring(0, 1) == "0")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The number must not start with 0!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (minDigits == maxDigits && input.Length != minDigits)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The number must consist of {minDigits} digit(s)!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (!(input.Length >= minDigits && input.Length <= maxDigits))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The number must have between {minDigits} and {maxDigits} digits!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (!validInt)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The number must be a valid integer!");
                    Console.ResetColor();
                    loop = true;
                }
            } while (loop);
            return true;
        }

        public static bool GetDecimalValue(string prompt, bool isPositive, decimal minValue, decimal maxValue, out decimal decimalValue)
        {
            decimalValue = 0;
            string input;
            bool loop;
            do
            {
                loop = false;
                Console.WriteLine("(Q to quit at any time)");
                Console.Write(prompt);
                input = Console.ReadLine();
                if (input.ToUpper() == "Q") return false;
                else if (!decimal.TryParse(input, out decimalValue))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must enter a decimal value!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (!((decimalValue > 0 && isPositive) || (decimalValue < 0 && !isPositive)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (isPositive)
                        Console.WriteLine("You must enter a positive value!");
                    else
                        Console.WriteLine("You must enter a negative value!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (decimalValue < minValue || decimalValue > maxValue)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (decimalValue < minValue)
                        Console.WriteLine($"The value must be greater than {minValue}!");
                    else
                        Console.WriteLine($"The value must be less than {maxValue}!");
                    Console.ResetColor();
                    loop = true;
                }
            } while (loop);
            return true;
        }

        public static bool GetDateValue(string prompt, DateTime? dateOnAfter, out DateTime dateValue)
        {
            string[] UIDateParseFormatArray = new string[] { "M/d/yy", "MM/dd/yyyy", "M.d.yy", "MM.dd.yyyy", "M-d-yy", "MM-dd-yyyy" };
            dateValue = default(DateTime);
            string input;
            bool validDate, loop;
            
            do
            {
                if (dateOnAfter == null) dateOnAfter = default(DateTime);
                loop = false;
                Console.WriteLine("(Q to quit at any time)");
                Console.Write(prompt + "({0}): ", UIDateParseFormat);
                input = Console.ReadLine();
                if (input.ToUpper() == "Q") return false;
                validDate = DateTime.TryParseExact(input, UIDateParseFormatArray, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);
                if (!validDate)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must enter a valid date!");
                    Console.ResetColor();
                    loop = true;
                }
                else if (dateValue.Date.CompareTo(dateOnAfter) <0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must enter a date >= {0:"+ UIDateParseFormat + "}!", DateTime.Now.Date);
                    Console.ResetColor();
                    loop = true;
                }
            } while (loop);
            return true;
        }

        public static void DisplayOrders(DisplayMode mode, List<Order> orders, ConsoleColor consoleColor)
        {
            if (orders.Count > 0)
            {
                Console.Clear();
                Console.ForegroundColor = consoleColor;
                Console.WriteLine("===== Orders ================================================");
                foreach (Order o in orders)
                {
                    if (mode == DisplayMode.Normal)
                        WriteOrder(o);
                    else
                        WriteOrderNRH(o);
                }
                Console.ResetColor();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        public static void DisplayOrder(DisplayMode mode, Order order, bool clearScreen, bool wait, ConsoleColor consoleColor)
        {
            if (clearScreen) Console.Clear();
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("===== Order =================================================");
            if (mode == DisplayMode.Normal)
                WriteOrder(order);
            else
                WriteOrderNRH(order);
            Console.ResetColor();
            if (wait)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        public static void DisplayFilenames(List<string> fileNames, bool clearScreen, bool wait, ConsoleColor consoleColor)
        {
            if (clearScreen) Console.Clear();
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("===== ALL Data Files=========================================");
            fileNames.Sort();
            foreach (string fileName in fileNames)
            {
                Console.WriteLine(fileName);
            }
            Console.ResetColor();
            if (wait)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void WriteOrder(Order o)
        {
            string orderNumber = "none (yet)";
            if (o.OrderNumber != 0) orderNumber = o.OrderNumber.ToString();

            Console.WriteLine(constDivLine);
            Console.WriteLine("{0,10} | {1,-20:"+ UIDateParseFormat + "}", orderNumber, o.OrderDate.ToString(UIDateParseFormat));
            Console.WriteLine("   Customer: {0}",o.CustomerName);
            Console.WriteLine("      State: {0} ({1:P})", o.OrderStateTax.StateCode, o.OrderStateTax.TaxRate / 100);
            Console.WriteLine("    Product: {0}",o.OrderProduct.ProductType);
            Console.WriteLine("  Area(ft²): {0:F3}", o.Area);
            Console.WriteLine("  Materials: {0:c}",o.FileMaterialCost);
            Console.WriteLine("      Labor: {0:c}",o.FileLaborCost);
            Console.WriteLine("        Tax: {0:c}",o.FileTax);
            Console.WriteLine(" Total Cost: {0:c}",o.FileTotal);
            Console.WriteLine(constDivLine);
        }


        private static void WriteOrderNRH(Order o)
        {
            string orderNumber = "none (yet)";
            if (o.OrderNumber != 0) orderNumber = o.OrderNumber.ToString();

            Console.WriteLine(constDivLine);
            Console.WriteLine("| Customer: {0,-20} | Order Number: {1,-11}|", o.CustomerName, orderNumber);
            Console.WriteLine("|    State: {0,-20} | Order Date: {1,-13:" + UIDateParseFormat + "}|",  o.OrderStateTax.StateCode + " (" + (o.OrderStateTax.TaxRate/100).ToString("P")+")", o.OrderDate);
            Console.WriteLine(constMidLine);
            Console.WriteLine("|   Product: {0,-19} |     Labor: {1,-14:c}|", o.OrderProduct.ProductType, o.FileLaborCost);
            Console.WriteLine("| Area(ft²): {0,-19:F3} | Materials: {1,-14:c}|", o.Area, o.FileMaterialCost);
            Console.WriteLine("|Total Cost: {0,-19:c} | Tax (L&M): {1,-14:c}|", o.FileTotal, o.FileTax);
            Console.WriteLine(constDivLine);
        }

        public static void DisplayProducts(List<Product> products, bool clearScreen, bool wait, ConsoleColor consoleColor)
        {
            if (products.Count > 0)
            {
                int lineNR = 0;
                if (clearScreen) Console.Clear();
                Console.ForegroundColor = consoleColor;
                Console.WriteLine("===== Products ==============================================");
                Console.WriteLine("|{0,-4}{1,-15}{2,-14}{3,-14}{4,13}", "NR", "Product Type", "Cost/ft²", "LaborCost/ft²", "|");
                foreach (Product p in products)
                {
                    lineNR++;
                    Console.WriteLine("|{0,-4}{1,-15}{2,-14:C}{3,-14:C}{4,13}", lineNR+".", p.ProductType, p.CostPerSquareFoot, p.LaborCostPerSquareFoot, "|");
                }
                Console.WriteLine(constDivLine);
                Console.ResetColor();
                if (wait)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        public static void DisplayTaxes(List<StateTax> taxes, bool clearScreen, bool wait, ConsoleColor consoleColor)
        {
            if (taxes.Count > 0)
            {
                int lineNR = 0;
                if (clearScreen) Console.Clear();
                Console.ForegroundColor = consoleColor;
                Console.WriteLine("===== Taxes =================================================");
                Console.WriteLine("|{0,-4}{1,-13}{2,-20}{3,-8}{4,15}", "NR", "State Code", "State Name", "Tax Rate", "|");
                foreach (StateTax t in taxes)
                {
                    lineNR++;
                    Console.WriteLine("|{0,-4}{1,-13}{2,-20}{3,-8:P}{4,15}", lineNR + ".", t.StateCode, t.StateName, t.TaxRate / 100, "|");
                }
                Console.WriteLine(constDivLine);
                Console.ResetColor();
                if (wait)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        public static void DisplayError(Exception ex)
        {
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
