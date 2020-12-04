using System;
using System.Threading;

namespace Banks
{
    public class CreditAccount : Account
    {
        private Thread _threadLimit;
        private int leftDays = 30;
        private DateTime creationTimeLimit;
        private double BankCreditLimit;
        private double BankCreditFee;
        
        public CreditAccount(int BIC, bool isDoubtful, double Fee, long numbersAccount, double bankLimitForTransactions, double creditLimit, double creditFee) 
            : base(BIC, isDoubtful, Fee, numbersAccount, bankLimitForTransactions)
        {
            _threadLimit = new Thread(TimeLimit);
            _threadLimit.Start();
            BankCreditLimit = creditLimit;
            BankCreditFee = creditFee;
            creationTimeLimit = DateTime.Now.Date;
        }

        private void TimeLimit()
        {
            while (true)
            {
                if (Date.globalDate - creationTimeLimit == TimeSpan.FromDays(30))
                {
                    if (IsDoubtful)
                    {
                        LimitForTransactionsLeft = BankLimitForTransactions;
                        Console.WriteLine("Ваш кредитный лимит обновился");
                    }

                    if (Balance < (BankCreditLimit / 2))
                    {
                        Balance = Balance + (Balance + BankCreditLimit) * BankCreditFee / 100;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public override void WithDraw(double money)
        {
            if (Balance < BankCreditLimit)
            {
                throw new ExceptionNotEnoughMoney("Недостаточно средств");
            }
            else
            {
                if (IsDoubtful)
                {
                    if (LimitForTransactionsLeft >= money)
                    {
                        LimitForTransactionsLeft -= money;
                        Balance -= money;
                    }
                    else
                    {
                        Console.WriteLine("Ваш лимит превышен за месяц");
                        Console.WriteLine("Ваш оставшийся лимит на транзакции: {0:f2}", LimitForTransactionsLeft);
                        throw new ExceptionCreditLimitExceeded("Первышен лимит");
                    }
                }
                else 
                {
                    if (Balance - money < BankCreditLimit )
                    {
                        Console.WriteLine("Так вы первысите свой кредитный лимит");
                        Console.WriteLine("Кредитного лимита осталось: {0:f2}", Math.Abs(BankCreditLimit - Balance));
                        throw new ExceptionCreditLimitExceeded("Первышен кредитный лимит");
                    }
                    else
                    {
                        Balance -= money;
                    }
                }
            }
        }

    }
}