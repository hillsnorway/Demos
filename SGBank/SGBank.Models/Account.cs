

using System;

namespace SGBank.Models
{
    public class Account
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountType Type { get; set; }

        public Nullable<AccountType> ParseAccountType(char accountChar)
        {
            switch (accountChar)
            {
                case 'F':
                    return AccountType.Free;
                case 'B':
                    return AccountType.Basic;
                case 'P':
                    return AccountType.Premium;
                default:
                    return null;
            }
        }
    }


}
