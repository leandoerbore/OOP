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
                var idClient1 = Manager.CreateClientInBank(Tinkoff, "Женя", "Жиборкин", "Сенная");

                var numbers1 = Manager.CreateDepositAccount(Tinkoff, 0);
                Manager.TopUpBalance(numbers1, 2000);
                var numbers2 = Manager.CreateDebitAccount(Tinkoff, 0);
                Manager.TopUpBalance(numbers2, 3000);
                
                Manager.CheckBalance(Tinkoff);
                Manager.ChangeTime(31);
                
                Manager.CheckBalance(Tinkoff);
                
            }
            
            
            // Case №2 Тест на сомнительный счёт 
            /*{
                Manager.CreateClientInBank(Tinkoff);
                Manager.CreateDebitAccount(Tinkoff, 0);
                Manager.TopUpBalance(Tinkoff);
                
                Manager.WithDraw(Tinkoff);
            }*/
            
            // Case №3 Тест на коммисию при пополнении, переводе или снятия со счетов в других банках и отмену транзакций
            /*{
                Manager.CreateClientInBank(Tinkoff);
                Manager.CreateDebitAccount(Tinkoff, 0);
                Manager.TopUpBalance(Sberbank);
                Manager.CheckBalance(Tinkoff);
                
                Manager.CreateClientInBank(Sberbank);
                Manager.CreateDebitAccount(Sberbank, 0);
                Manager.Transfer(Sberbank);
                
                Manager.CheckBalance(Tinkoff);
                Manager.CheckBalance(Sberbank);
                
                Manager.CanceTransaction(Sberbank);
                
                Manager.CheckBalance(Tinkoff);
                Manager.CheckBalance(Sberbank);
            }*/
        }
    }
}
