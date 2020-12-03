using System;
using System.Threading;

namespace Banks
{
    public class DebitAccount : AccountWithInterest
    {
        public DebitAccount(int BIC, bool isDoubtful, double interest, double Fee, long numbersAccount, double bankLimitForTransactions) 
            : base(BIC, isDoubtful, Fee, numbersAccount, bankLimitForTransactions)
        {
            InterestProcent = interest;
            DateForInterest = Date.globalDate;
            lastDateCalcInterest = Date.globalDate;
        }

        protected override void InterestCalc()
        {
            while (true)
            {
                CalcForDay();
                if (itterationForInterest >= 30)
                {
                    Balance += _interestBalance;
                    _interestBalance = 0;
                    itterationForInterest = 0;
                    //Console.WriteLine("Баланс на счете {0} пополнился: {1:f2}", NumbersAccount, Balance);
                    if (IsDoubtful)
                    {
                        LimitForTransactionsLeft = BankLimitForTransactions;
                    }
                }
                
                Thread.Sleep(10);
            }
        }

        protected override void CalcForDay()
        {
            var dif = Date.globalDate - lastDateCalcInterest;
            if (dif.Days >= 1)
            {
                _interestBalance += (InterestProcent / 365) * Balance / 100;
                ++itterationForInterest;
                lastDateCalcInterest = Date.globalDate;
                Console.WriteLine("На дебетовом счёте {0} InterestBalance: {1:f2}", NumbersAccount, _interestBalance);
            }
        }
        
        

        public override void WithDraw(double money)
        {
            if (Balance < money)
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
                        throw new ExceptionNotEnoughMoney("Недостаточно средств");
                    }
                }
                else
                {
                    Balance -= money;   
                }
            }
        }
    }
}