using FlooringMastery.UI.WorkFlows;
using System;


namespace FlooringMastery.UI
{
    public class Menu
    {

        public static void Start()
        {
            try
            {
                const string divLine = "=============================================================";

                //Default Display Mode
                DisplayMode displayMode = DisplayMode.Condensed;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine(divLine);
                    Console.WriteLine("|   SWC CORP. - FLOTSam  (Flooring Order Tracking System)   {0}", "|");
                    Console.WriteLine(divLine);

                    Console.WriteLine("|0. Toggle display mode (currently: {0}) {1," + (23 - displayMode.ToString().Length) + "}", displayMode.ToString(), "|");
                    Console.WriteLine("|1. List ALL Files      (given: nothing)                    {0}", "|");
                    Console.WriteLine("|2. List ALL Orders     (given: order date)                 {0}", "|");
                    Console.WriteLine("|3. Add an Order        (given: order date)                 {0}", "|");
                    Console.WriteLine("|4. Show an Order       (given: order date & order number)  {0}", "|");
                    Console.WriteLine("|5. Edit an Order       (given: order date & order number)  {0}", "|");
                    Console.WriteLine("|6. Remove an Order     (given: order date & order number)  {0}", "|");
                    Console.WriteLine("|7. (Q) to quit                                             {0}", "|");
                    Console.WriteLine(divLine);

                    Console.Write("\nEnter selection: ");

                    string userInput = Console.ReadLine().ToUpper();

                    switch (userInput)
                    {
                        case "0":
                            if (displayMode == DisplayMode.Normal)
                                displayMode = DisplayMode.Condensed;
                            else
                                displayMode = DisplayMode.Normal;
                            break;
                        case "1":
                            ListAllFilesWorkFlow.Execute();
                            break;
                        case "2":
                            DisplayOrdersWorkFlow.Execute(displayMode);
                            break;
                        case "3":
                            AddOrderWorkFlow.Execute(displayMode);
                            break;
                        case "4":
                            DisplayOrderWorkFlow.Execute(displayMode);
                            break;
                        case "5":
                            EditOrderWorkFlow.Execute(displayMode);
                            break;
                        case "6":
                            DeleteOrderWorkFlow.Execute(displayMode);
                            break;
                        case "7":
                        case "Q":
                            return;
                        default:
                            //Invalid Input
                            ConsoleIO.DisplayText($"Invalid entry! Please enter a number between 1 & 7 \nPress any key to try again...",false,true, ConsoleColor.Red);
                            continue;
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleIO.DisplayError(ex);
            }
        }
    }

    public enum DisplayMode
    {
        Normal,
        Condensed
    }
}
