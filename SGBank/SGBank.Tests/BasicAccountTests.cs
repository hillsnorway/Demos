using NUnit.Framework;
using SGBank.BLL;
using SGBank.BLL.DepositRules;
using SGBank.BLL.WithdrawlRules;
using SGBank.Models;
using SGBank.Models.Interfaces;
using SGBank.Models.Responses;


namespace SGBank.Tests
{
    [TestFixture]
    public class BasicAccountTests
    {
        [TestCase("33333", "Basic Account", 100, AccountType.Free, 250, false)] //Fail, wrong account type
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, -100, false)] //Fail, negative number deposited
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, 250, true)] //Success!!!
        public void BasicAccountDepositRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, bool expectedResult)
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

        [TestCase("33333", "Basic Account", 1500, AccountType.Basic, -1000, 1500, false)] //Fail, too much withdrawn
        [TestCase("33333", "Basic Account", 100, AccountType.Free, -100, 100, false)] //Fail, not a basic account type
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, 100, 100, false)] //Fail, positive number withdrawn
        [TestCase("33333", "Basic Account", 150, AccountType.Basic, -50, 100,  true)] //Success!!!
        [TestCase("33333", "Basic Account", 100, AccountType.Basic, -150, -60, true)] //Success!!!  Also verify that there is NO overdraft fee...
        public void BasicAccountWithdrawRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, decimal newBalance, bool expectedResult)
        {
            IWithdraw myWithdrawRule = new BasicAccountWithdrawRule();
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
