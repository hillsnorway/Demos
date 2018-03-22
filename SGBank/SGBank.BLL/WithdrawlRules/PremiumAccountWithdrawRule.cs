﻿using SGBank.Models;
using SGBank.Models.Interfaces;
using SGBank.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBank.BLL.WithdrawlRules
{
    public class PremiumAccountWithdrawRule : IWithdraw
    {
        public AccountWithdrawResponse Withdraw(Account account, decimal amount)
        {
            AccountWithdrawResponse response = new AccountWithdrawResponse();
            if (account.Type != AccountType.Premium)
            {
                response.Success = false;
                response.Message = "Error: a non-premium account hit the Premium Withdraw Rule. \nContact IT!";
                return response;
            }

            if (amount >= 0M)
            {
                response.Success = false;
                response.Message = "Withdrawl amounts must be negativ!";
                return response;

            }

            if (account.Balance + amount < -500)
            {
                response.Success = false;
                response.Message = "Premium accounts have a $500 overdraft limit!";
                return response;
            }

            response.OldBalance = account.Balance;
            account.Balance += amount;
            response.Account = account;
            response.Amount = amount;
            response.Success = true;

            return response;
        }
    }
}