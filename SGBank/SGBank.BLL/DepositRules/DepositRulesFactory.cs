using SGBank.Models;
using SGBank.Models.Interfaces;
using System;


namespace SGBank.BLL.DepositRules
{
    public class DepositRulesFactory
    {
        public static IDeposit Create(AccountType type)
        {
            switch (type)
            {
                case AccountType.Free:
                    return new FreeAccountDepositRule();
                case AccountType.Basic:
                    return new NoLimitDepositRule();
                case AccountType.Premium:
                    return new NoLimitDepositRule();
                default:
                    throw new Exception("Account type is not supported!");
            }
        }
    }
}
