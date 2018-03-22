using SGBank.Models;
using SGBank.Models.Interfaces;
using SGBank.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBank.BLL.WithdrawlRules
{
    public class BasicAccountWithdrawRule : IWithdraw
    {
        public AccountWithdrawResponse Withdraw(Account account, decimal amount)
        {
            AccountWithdrawResponse response = new AccountWithdrawResponse();
            if (account.Type != AccountType.Basic)
            {
                response.Success = false;
                response.Message = "Error: a non-basic account hit the Basic Withdraw Rule. \nContact IT!";
                return response;
            }

            if (amount >= 0M)
            {
                response.Success = false;
                response.Message = "Withdrawl amounts must be negativ!";
                return response;

            }

            if (amount < -500M)
            {
                response.Success = false;
                response.Message = "Basic accounts cannot withdraw more than $500 at a time!";
                return response;
            }

            if (account.Balance + amount < -100)
            {
                response.Success = false;
                response.Message = "Basic accounts have a $100 overdraft limit!";
                return response;
            }

            response.OldBalance = account.Balance;
            account.Balance += amount;
            if (account.Balance < 0) account.Balance -= 10M; //Basic accounts have a $10 fee for overdrafts
            response.Account = account;
            response.Amount = amount;
            response.Success = true;

            return response;
        }
    }
}
