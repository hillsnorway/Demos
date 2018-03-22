using SGBank.Models;
using SGBank.Models.Interfaces;
 

namespace SGBank.Data
{
    public class FreeAccountTestRepository : IAccountRepository
    {
        private static Account _account = new Account
        {
            Name = "Free Account",
            Balance = 100M,
            AccountNumber = "12345",
            Type = AccountType.Free
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
