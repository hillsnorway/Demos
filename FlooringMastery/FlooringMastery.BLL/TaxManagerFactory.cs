using FlooringMastery.Data;
using System;
using System.Configuration;


namespace FlooringMastery.BLL
{
    public class TaxManagerFactory
    {
        public static TaxManager Create()
        {
            string mode = ConfigurationManager.AppSettings["Mode"].ToString().ToUpper();

            switch (mode)
            {
                case "TEST":
                    return new TaxManager(new TaxTestRepository());
                case "LIVE":
                    return new TaxManager(new TaxFileRepository());
                default:
                    throw new Exception("Mode value in app config is not valid.");
            }
        }
    }
}
