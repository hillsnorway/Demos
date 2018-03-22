using FlooringMastery.BLL;
using System;
using System.Collections.Generic;


namespace FlooringMastery.UI.WorkFlows
{
    public class ListAllFilesWorkFlow
    {
        public static void Execute()
        {
            OrderManager myOM = OrderManagerFactory.Create(default(DateTime).Date); //Just accessing myOM to get a list of filenames, therefore using default system date
            List<string> fileNames = myOM.GetAllFileNames(out string dataPath);
            ConsoleIO.DisplayFilenames(fileNames, true, true, ConsoleColor.Blue);
        }
    }
}
