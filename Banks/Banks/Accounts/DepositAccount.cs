using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Banks
{
    public class DepositAccount : AccountWithInterest
    {
        private bool AccessToWithDraw = false;
        public int DaysForAccess { get; private set; }

        private List<(int firstInterval, int secondInterval, double interest)> _conditions
            = new List<(int firstInterval, int secondInterval, double interest)>();

        private double _interestBalance = 0;
        private int itterationForInterest { get; set; } = 0;
        
        public DepositAccount(int BIC, bool isDoubtful, List<(int firstInterval, int secondInterval, double interest)> conditions, double Fee, long numbersAccount, int DaysForDeposit, double bankLimitForTransactions) 
            : base(BIC, isDoubtful, Fee, numbersAccount, bankLimitForTransactions)
        {
            DaysForAccess = DaysForDeposit; 
            _conditions = conditions;
        }

        protected override void InterestCalc()
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
        }

        protected override void CalcForDay()
        {
            var dif = Date.date().globalDate - lastDateCalcInterest;
            if (dif.Days >= 1)
            {
                var condition = _conditions
                    .Find(condition => Balance >= condition.firstInterval && Balance <= condition.secondInterval);
                InterestProcent = condition.interest;
                if (dif.Days >= 1)
                {
                    _interestBalance += (InterestProcent / 365) * Balance / 100;
                    ++itterationForInterest;
                    lastDateCalcInterest = Date.date().globalDate;

                    if (!AccessToWithDraw && DaysForAccess <= 0)
                    {
                        AccessToWithDraw = true;
                    }
                    else
                    {
                        DaysForAccess -= 1;
                    }
                    //Console.WriteLine("На депозитном счёте {0} InterestBalance: {1:f2}", NumbersAccount, _interestBalance);
                }

            }
        }



        public override void WithDraw(double money)
        {
            if (!AccessToWithDraw)
            {
                throw new ExceptionDepositTime("Срок депозита еще не закончился");
            }
            else
            {
                if (Balance < money)
                {
                    throw new ExceptionNotEnoughMoney("Недостаточно средств");
                }
                else
                {
                    if (IsDoubtful)
                    {
                        if (LimitForTransactionsLeft <= money)
                        {
                            LimitForTransactionsLeft -= money;
                        }
                        else
                        {
                            Console.WriteLine("Ваш лимит превышен за месяц");
                            Console.WriteLine("Ваш оставшийся лимит на транзакции: {0:f2}", LimitForTransactionsLeft);
                            throw new ExceptionLimit("Превышен лимит");
                        }
                        Balance -= money;
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