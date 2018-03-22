using FlooringMastery.Data;
using System;
using System.Configuration;


namespace FlooringMastery.BLL
{
    public class ProductManagerFactory
    {
        public static ProductManager Create()
        {
            string mode = ConfigurationManager.AppSettings["Mode"].ToString().ToUpper();

            switch (mode)
            {
                case "TEST":
                    return new ProductManager(new ProductTestRepository());
                case "LIVE":
                    return new ProductManager(new ProductFileRepository());
                default:
                    throw new Exception("Mode value in app config is not valid.");
            }
        }
    }
}
