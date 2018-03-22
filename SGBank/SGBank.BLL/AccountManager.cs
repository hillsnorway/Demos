using SGBank.BLL.WithdrawlRules;
using SGBank.BLL.DepositRules;
using SGBank.Models.Interfaces;
using SGBank.Models.Responses;


namespace SGBank.BLL
{
    public class AccountManager
    {
        private IAccountRepository _accountRepository;

        public AccountManager(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public AccountLookupResponse LookupAccount(string accountNumber)
        {
            AccountLookupResponse response = new AccountLookupResponse();
            response.Account = _accountRepository.LoadAccount(accountNumber);
            if(response.Account == null)
            {
                response.Success = false;
                response.Message = $"{accountNumber} is not a valid account.";
            }
            else
            {
                response.Success = true;
            }
            return response;
        }

        public AccountDepositResponse Deposit (string accountNumber, decimal amount)
        {
            AccountDepositResponse response = new AccountDepositResponse();
            response.Account = _accountRepository.LoadAccount(accountNumber);
            if (response.Account == null)
            {
                response.Success = false;
                response.Message = $"{accountNumber} is not a valid account.";
                return response;
            }

            //NRH NINJECT
            //IDeposit depositRule = DepositRulesFactory.Create(response.Account.Type);
            IDeposit depositRule = DIDepositRules.Create(response.Account.Type);
            response = depositRule.Deposit(response.Account, amount);

            if (response.Success)
            {
                _accountRepository.SaveAccount(response.Account);
            }

            return response;
            
        }

        public AccountWithdrawResponse Withdraw(string accountNumber, decimal amount)
        {
            AccountWithdrawResponse response = new AccountWithdrawResponse();
            response.Account = _accountRepository.LoadAccount(accountNumber);
            if (response.Account == null)
            {
                response.Success = false;
                response.Message = $"{accountNumber} is not a valid account.";
                return response;
            }

            //NRH NINJECT
            //IWithdraw withdrawRule = WithdrawRulesFactory.Create(response.Account.Type);
            IWithdraw withdrawRule = DIWithdrawRules.Create(response.Account.Type); 
            response = withdrawRule.Withdraw(response.Account, amount);

            if (response.Success)
            {
                _accountRepository.SaveAccount(response.Account);
            }

            return response;

        }
    }
}
