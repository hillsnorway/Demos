using SGBank.Models;
using SGBank.Models.Interfaces;
 

namespace SGBank.Data
{
    public class BasicAccountTestRepository : IAccountRepository
    {
        private static Account _account = new Account
        {
            Name = "Basic Account",
            Balance =100M,
            AccountNumber = "33333",
            Type = AccountType.Basic
        };

        public Account LoadAccount(string accountNumber)
        {
            if (accountNumber == _account.AccountNumber)
            {
                return _account;
            }
            else return null;
        }

        public void SaveAccount(Account account)
        {
            _account = account;
        }
    }
}
