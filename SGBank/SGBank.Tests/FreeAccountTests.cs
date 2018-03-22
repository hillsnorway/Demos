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
    public class FreeAccountTests
    {
        [Test]
        public void CanLoadFreeAccountTestData()
        {
            AccountManager manager = AccountManagerFactory.Create();
            AccountLookupResponse response = manager.LookupAccount("");
            Assert.IsNull(response.Account);

            response = manager.LookupAccount("0");
            Assert.IsNull(response.Account);

            response = manager.LookupAccount("11223");
            Assert.IsNull(response.Account);

            response = manager.LookupAccount("-12345");
            Assert.IsNull(response.Account);

            response = manager.LookupAccount("12345");
            Assert.IsNotNull(response.Account);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("12345", response.Account.AccountNumber);
        }

        [TestCase ("12345","Free Account",100, AccountType.Free,250,false)] //fail, too much deposited
        [TestCase("12345", "Free Account", 100, AccountType.Free, -100, false)] //fail, negative number deposited
        [TestCase("12345", "Free Account", 100, AccountType.Basic, 50, false)] //fail, not a free account type
        [TestCase("12345", "Free Account", 100, AccountType.Free, 50, true)] //success
        public void FreeAccountDepositRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, bool expectedResult)
        {
            IDeposit myDepositRule = new FreeAccountDepositRule();
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

        [TestCase("12345", "Free Account", 100, AccountType.Free, 0, false)] //fail, positive (or 0) amount withdrawn
        [TestCase("12345", "Free Account", 100, AccountType.Free, 15, false)] //fail, positive (or 0) amount withdrawn
        [TestCase("12345", "Free Account", 500, AccountType.Free, -101, false)] //fail, negative withdrawl over limit
        [TestCase("12345", "Free Account", 49, AccountType.Free, -50, false)] //fail, overdraft
        [TestCase("12345", "Free Account", 100, AccountType.Basic, -50, false)] //fail, not a free account type
        [TestCase("12345", "Free Account", 100, AccountType.Free, -50, true)] //success
        public void FreeAccountWithdrawRuleTest(string accountNumber, string name, decimal balance, AccountType accountType, decimal amount, bool expectedResult)
        {
            IWithdraw myWithdrawRule = new FreeAccountWithdrawRule();
            Account myAccount = new Account()
            {
                AccountNumber = accountNumber,
                Name = name,
                Balance = balance,
                Type = accountType
            };
            AccountWithdrawResponse response = myWithdrawRule.Withdraw(myAccount, amount);
            Assert.AreEqual(expectedResult, response.Success);
        }
    }
}
