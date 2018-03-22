using Ninject;
using SGBank.Models;
using SGBank.Models.Interfaces;
using System;


namespace SGBank.BLL.DepositRules
{
    public class DIDepositRules
    {
        public static IDeposit Create(AccountType type)
        {
            switch (type)
            {
                case AccountType.Free:
                    DIContainer.Kernel.Unbind<IDeposit>();
                    DIContainer.Kernel.Bind<IDeposit>().To<FreeAccountDepositRule>();
                    return DIContainer.Kernel.Get<IDeposit>();
                case AccountType.Basic:
                case AccountType.Premium:
                    DIContainer.Kernel.Unbind<IDeposit>();
                    DIContainer.Kernel.Bind<IDeposit>().To<NoLimitDepositRule>();
                    return DIContainer.Kernel.Get<IDeposit>();
                default:
                    throw new Exception("Account type is not supported!");
            }
        }
    }
}
