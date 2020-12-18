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
                var money1 = 2000;
                var money2 = 3000;
                
                var idClient1 = Manager.CreateClientInBank(Tinkoff, "Женя", "Жиборкин", "Сенная");

                var numbers1 = Manager.CreateDepositAccount(Tinkoff, idClient1);
                Manager.TopUpBalance(Tinkoff, numbers1, money1);
                var numbers2 = Manager.CreateDebitAccount(Tinkoff, idClient1);
                Manager.TopUpBalance(Tinkoff, numbers1, money2);
                
                var balance1 = Manager.GetAccountBalance(numbers1);
                var balance2 = Manager.GetAccountBalance(numbers2);
                
                Manager.ChangeTime(32);

                var accountsBalance = Manager.GetBalanceAllAccounts(Tinkoff);
                if (accountsBalance[0].balance != 2000 && accountsBalance[1].balance !=3000)
                    Console.WriteLine("Case 1 успешно пройден");
                else
                    Console.WriteLine("Case 1 не пройден");
                
            }
            
            
            // Case №2 Тест на сомнительный счёт 
            {
                try
                {
                    var idClient1 = Manager.CreateClientInBank(Tinkoff, "Алексей", "Навальный", "фсб");
                    var numbers1 = Manager.CreateDebitAccount(Tinkoff, idClient1);
                    Manager.TopUpBalance(Tinkoff, numbers1, 1000000);
                
                    Manager.WithDraw(Tinkoff, numbers1, 60000);
                    Console.WriteLine("Case 2 не пройден");
                }
                catch (ExceptionLimit e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Case 2 успешно пройден");
                }
                
            }
            
            // Case №3 Тест на коммисию при пополнении, переводе или снятия со счетов в других банках и отмену транзакций
            {
                var money1 = 40000;
                var money2 = 20000;
                
                var casePoints = 0;
                
                var idClient1 = Manager.CreateClientInBank(Tinkoff, "Алексей", "Навальный", "фсб");
                long numbers1 = Manager.CreateDebitAccount(Tinkoff, idClient1);
                Manager.TopUpBalance(Sberbank, numbers1, money1);
                var accountsBalance = Manager.GetBalanceAllAccounts(Tinkoff);
                
                var idClient2 = Manager.CreateClientInBank(Sberbank,"Женя", "Жиборкин", "Сенная", 2200220);
                var numbers2 = Manager.CreateDebitAccount(Sberbank, idClient2);
                Manager.Transfer(numbers1, numbers2, money2 );
                if (Manager.GetAccountBalance(numbers2) == money2)
                    casePoints++;
                
                Manager.CancleTransaction(numbers1, 0);
                
                if (Manager.GetAccountBalance(numbers2) == 0 && Manager.GetAccountBalance(numbers1) == money1)
                    casePoints++;
                
                Console.WriteLine(casePoints == 2 ? "Case 3 успешно пройден" : "Case 3 не пройден");
            }
        }
    }
}
