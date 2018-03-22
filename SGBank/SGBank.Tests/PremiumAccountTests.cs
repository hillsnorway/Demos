using NUnit.Framework;
using SGBank.BLL.DepositRules;
using SGBank.BLL.WithdrawlRules;
using SGBank.Models;
using SGBank.Models.Interfaces;
using SGBank.Models.Responses;


namespace SGBank.Tests
{
    [TestFixture]
    class PremiumAccountTests
    {
        [TestCase("99999", "Premium Account", 100, AccountType.Free, 250, false)] //Fail, wrong account type - Free
        [TestCase("99999", "Premium Account", 100, AccountType.Premium, -100, false)] //Fail, negative number deposited
        [TestCase("99999", "Premium Account", 100, AccountType.Premium, 5000, true)] //Success!!!
        public void PremiumAccountDepositRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, bool expectedResult)
        {
            IDeposit myDepositRule = new NoLimitDepositRule();
            Account myAccount = new Account()
            {
                AccountNumber = accountNumber,
                Name = name,
                Balance = balance,
                Type = accountType
            };
            AccountDepositResponse response = myDepositRule.Deposit(myAccount, amount);
            Assert.AreEqual(expectedResult, response.Success);
        }

        [TestCase("99999", "Basic Account", 100, AccountType.Free, -100, 100, false)] //Fail, not a premium account type - Free
        [TestCase("99999", "Basic Account", 100, AccountType.Basic, -100, 100, false)] //Fail, not a premium account type - Basic
        [TestCase("99999", "Basic Account", 100, AccountType.Premium, 100, 100, false)] //Fail, positive number withdrawn
        [TestCase("99999", "Basic Account", 100, AccountType.Premium, -601, 100, false)] //Fail, can not overdraft for more then $500...
        [TestCase("99999", "Basic Account", 150, AccountType.Premium, -50, 100, true)] //Success!!!
        [TestCase("99999", "Basic Account", 100, AccountType.Premium, -600, -500, true)] //Success!!!  Also verify that there is NO overdraft fee...

        public void PremiumAccountWithdrawRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, decimal newBalance, bool expectedResult)
        {
            IWithdraw myWithdrawRule = new PremiumAccountWithdrawRule();
            Account myAccount = new Account()
            {
                AccountNumber = accountNumber,
                Name = name,
                Balance = balance,
                Type = accountType
            };
            AccountWithdrawResponse response = myWithdrawRule.Withdraw(myAccount, amount);
            Assert.AreEqual(expectedResult, response.Success);
            if (response.Success)
            {
                Assert.AreEqual(newBalance, response.Account.Balance);
            };
        }
    }
}
