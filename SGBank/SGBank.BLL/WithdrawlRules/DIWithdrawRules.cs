using Ninject;
using SGBank.Models;
using SGBank.Models.Interfaces;
using System;


namespace SGBank.BLL.WithdrawlRules
{
    public class DIWithdrawRules 
    {
        public static IWithdraw Create(AccountType type)
        {
            switch (type)
            {
                case AccountType.Free:
                    DIContainer.Kernel.Unbind<IWithdraw>();
                    DIContainer.Kernel.Bind<IWithdraw>().To<FreeAccountWithdrawRule>();
                    return DIContainer.Kernel.Get<IWithdraw>();
                case AccountType.Basic:
                    DIContainer.Kernel.Unbind<IWithdraw>();
                    DIContainer.Kernel.Bind<IWithdraw>().To<BasicAccountWithdrawRule>();
                    return DIContainer.Kernel.Get<IWithdraw>();
                case AccountType.Premium:
                    DIContainer.Kernel.Unbind<IWithdraw>();
                    DIContainer.Kernel.Bind<IWithdraw>().To<PremiumAccountWithdrawRule>();
                    return DIContainer.Kernel.Get<IWithdraw>();
                default:
                    throw new Exception("Account type is not supported!");
            }
        }
    }
}
