using FlooringMastery.Data;
using System;
using System.Configuration;


namespace FlooringMastery.BLL
{
    public class OrderManagerFactory
    {
        public static OrderManager Create(DateTime orderDate)
        {
            string mode = ConfigurationManager.AppSettings["Mode"].ToString().ToUpper();

            switch (mode)
            {
                case "TEST":
                    return new OrderManager(new OrderTestRepository(orderDate));
                case "LIVE":
                    return new OrderManager(new OrderFileRepository(orderDate));
                default:
                    throw new Exception("Mode value in app config is not valid.");
            }
        }
    }
}
