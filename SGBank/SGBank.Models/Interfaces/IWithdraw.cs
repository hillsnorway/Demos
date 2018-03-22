using SGBank.Models.Responses;


namespace SGBank.Models.Interfaces
{
    public interface IWithdraw
    {
        AccountWithdrawResponse Withdraw(Account account, decimal amount);
    }
}
