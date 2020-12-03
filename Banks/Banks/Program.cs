using System;
using System.Threading;

namespace Banks
{
    class Program
    {
        static void Main(string[] args)
        {
            var Tinkoff = "TinkoffBank";
            var Sberbank = "SberBank";
            var GazpromBank = "GazpromBank";
            
            
            // Case №1 Тест на подсчёт процента на остаток счёта
            {
                Manager.CreateClientInBank(Tinkoff);
                
                Manager.CreateDepositAccount(Tinkoff, 0);
                Manager.TopUpBalance(Tinkoff);
                Manager.CreateDebitAccount(Tinkoff, 0);
                Manager.TopUpBalance(Tinkoff);
                
                
                Manager.CheckBalance(Tinkoff);
                ChangeTime(31);
                
                Thread.Sleep(100);

            }
            
            
            
            
        }

        public static void ChangeTime(int days)
        {
            for (int i = 0; i < days; ++i)
            {
                var dateTest = Date.globalDate + TimeSpan.FromDays(1);
                Date.changeTime(dateTest);
                Thread.Sleep(1000);
            }
        }

    }
}
