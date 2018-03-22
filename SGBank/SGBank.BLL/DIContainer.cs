using Ninject;
using SGBank.Data;
using SGBank.Models.Interfaces;
using System;
using System.Configuration;


namespace SGBank.BLL
{
    public static class DIContainer
    {
        public static IKernel Kernel = new StandardKernel();

        static DIContainer()
        {
            string mode = ConfigurationManager.AppSettings["Mode"].ToString();

            switch (mode)
            {
                case "FreeTest":
                    Kernel.Bind<IAccountRepository>().To<FreeAccountTestRepository>();
                    break;
                case "BasicTest":
                    Kernel.Bind<IAccountRepository>().To<BasicAccountTestRepository>();
                    break;
                case "PremiumTest":
                    Kernel.Bind<IAccountRepository>().To<PremiumAccountTestRepository>();
                    break;
                case "FileTest":
                    Kernel.Bind<IAccountRepository>().To<FileAccountRepository>();
                    break;
                default:
                    throw new Exception("Mode value in app config is not valid.");
            }
        }
    }

}
