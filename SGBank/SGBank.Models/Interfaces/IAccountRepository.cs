

namespace SGBank.Models.Interfaces
{
    public interface IAccountRepository
    {
        Account LoadAccount(string accountNumber);
        void SaveAccount(Account account);
    }
}
