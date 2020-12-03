using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Exceptions;

namespace Banks
{
    public static class Manager
    {
        private static List<Bank> banks = new List<Bank>(){new TinkoffBank(), new SberBank(), new GazpromBank()};

        public static void CreateClientInBank(string NameBank)
        {
            var bank = FindBank(NameBank);
            
            bank.clients.Add(new Client());
            

            Console.WriteLine();
        }

        public static void CreateDepositAccount(string NameBank, int idClient)
        {
            var bank = FindBank(NameBank);
            var BIC = bank.BIC;
            var condition = bank.conditions;
            var fee = bank.Fee;
            var numbersAccount = bank.CreateRandomAccountNumbers();
            var isDoubrful = CheckForDoubtful(bank, idClient);
            var bankLimitForTransactions = bank.LimitForTransactions;
            
            bank.accounts.Add(new DepositAccount(BIC, isDoubrful, condition, fee, numbersAccount, 30, bankLimitForTransactions));
                
            var client = bank.clients[idClient];
            var account = bank.accounts[bank.accounts.Count - 1];
                
            bank.ConnectClientAndAccount(client,account);
            Console.WriteLine("Для {0} {1} был создан депозитный счёт в банке {2} с номером : {3}", client.Name, client.Surname, bank.Name, account.NumbersAccount);
        }
        
        public static void CreateDebitAccount(string NameBank, int idClient)
        {
            var bank = FindBank(NameBank);
            int BIC = bank.BIC;
            var interest = bank.Interest;
            var fee = bank.Fee;
            var numbersAccount = bank.CreateRandomAccountNumbers();
            var isDoubrful = CheckForDoubtful(bank, idClient);
            var bankLimitForTransactions = bank.LimitForTransactions;
            
            bank.accounts.Add(new DebitAccount(BIC, isDoubrful, interest, fee, numbersAccount, bankLimitForTransactions));
                
            var client = bank.clients[idClient];
            var account = bank.accounts[bank.accounts.Count - 1];
                
            bank.ConnectClientAndAccount(client,account);
            Console.WriteLine("Для {0} {1} был создан дебетовый счёт в банке {2} с номером : {3}", client.Name, client.Surname, bank.Name, account.NumbersAccount);
        }
        
        public static void CreateCreditAccount(string NameBank, int idClient)
        {
            var bank = FindBank(NameBank);
            int BIC = bank.BIC;
            var fee = bank.Fee;
            var numbersAccount = bank.CreateRandomAccountNumbers();
            var isDoubrful = CheckForDoubtful(bank, idClient);
            var bankLimitForTransactions = bank.LimitForTransactions;
            var bankCreditLimit = bank.CreditLimit;
            var bankCreditFee = bank.CreditFee;
            
            bank.accounts.Add(new CreditAccount(BIC, isDoubrful, fee, numbersAccount, bankLimitForTransactions, bankCreditLimit, bankCreditFee ));
                
            var client = bank.clients[idClient];
            var account = bank.accounts[bank.accounts.Count - 1];
                
            bank.ConnectClientAndAccount(client,account);
            Console.WriteLine("Для {0} {1} был создан кредитный счёт в банке {2} с номером : {3}", client.Name, client.Surname, bank.Name, account.NumbersAccount);
        }
        
        

        public static void AddInformationAboutClientInTheBank(string NameBank, int idClient)
        {
            var bank = FindBank(NameBank);
            var client = bank.clients[idClient];
            
            client.AddInformationAboutClient();
            var clientIsDoubtful = CheckForDoubtful(bank, idClient);
            var accounts = bank.clientsAndAccounts
                .FindAll(connection => connection.client == client)
                .Select(connection => connection.account);

            if (!clientIsDoubtful)
            {
                foreach (var account in accounts)
                {
                    account.IsDoubtful = false;
                }
            }
        }
        
        private static bool CheckForDoubtful(Bank bank, int idClient)
        {
            var clientInfo = bank.clients[idClient];
            
            return (clientInfo.Passport is null || clientInfo.Adress is null) ? true : false;
        }

        public static void ShowAllClientsOfBank(string NameBank)
        {
            var bank = FindBank(NameBank);

            int i = 0;
            foreach (var client in bank.clients)
            {
                ++i;
                Console.WriteLine("{0}) {1} {2}", i, client.Name, client.Surname );
            }
        }

        private static Bank FindBank(string NameBank)
        {
            var bank = banks.Find(bank => bank.Name == NameBank);
            if (bank is null)
                throw new ExceptionBankDoesNotExist("Такой банк не существует");
            else
            {
                return bank;
            }
        }

        public static void CheckBalance(string NameBank)
        {
            var bank = FindBank(NameBank);

            foreach (var account in bank.accounts)
            {
                Console.WriteLine("Account: {0}, balance: {1:f2} ", account.NumbersAccount, account.Balance);
            }
        }

        public static void TopUpBalance (string NameBank)
        {
            var bank = FindBank(NameBank);
            Console.WriteLine("Введите номер счёта, который вы хотите пополнить");
            long numbersAccount = long.Parse(Console.ReadLine());

            var bic = numbersAccount / 100000;
            
            var requiredBank = banks.Find(bank => bank.BIC == bic);
            var account = requiredBank.accounts.Find(account => account.NumbersAccount == numbersAccount);

            Console.WriteLine("Сколько денег вы хотите положить на счёт ?");
            var answer = double.Parse(Console.ReadLine());
            
            bank.TopUpAccount(account, answer);
        }

        public static void WithDraw(string NameBank)
        {
            var bank = FindBank(NameBank);
            Console.WriteLine("Введите номер счёта с котого вы хотите списать деньги");
            long numbersAccount = long.Parse(Console.ReadLine());
            
            var bic = numbersAccount / 100000;
            
            var requiredBank = banks.Find(bank => bank.BIC == bic);
            var account = requiredBank.accounts.Find(account => account.NumbersAccount == numbersAccount);
            
            Console.WriteLine("Сколько денег вы хотите списать");
            var answer = double.Parse(Console.ReadLine());
            
            
            bank.WithDrawFromAccount(account, answer);
        }

        public static void Transfer(string NameBank)
        {
            var bank = FindBank(NameBank);
            
            Console.WriteLine("С какого счёта вы хотите перевести деньги");
            int numbersAccountFrom = int.Parse(Console.ReadLine());
            
            Console.WriteLine("На какой счёт вы хотите перевести деньги");
            int numbersAccountTo = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Сколько денег вы хотите перевести на счёт: {0}", numbersAccountTo);
            var money = double.Parse(Console.ReadLine());
            
            var bicFrom = numbersAccountFrom / 100000;
            var requiredBankFrom = banks.Find(bank => bank.BIC == bicFrom);
            var accountFrom = requiredBankFrom.accounts.Find(account => account.NumbersAccount == numbersAccountFrom);
            
            var bicTo = numbersAccountTo / 100000;
            var requiredBankTo = banks.Find(bank => bank.BIC == bicTo);
            var accountTo = requiredBankTo.accounts.Find(account => account.NumbersAccount == numbersAccountTo);
            
            bank.Transfer(accountFrom, accountTo, money);
            
            Console.WriteLine("Трансфер окончен");

        }

        public static void ShowAllAccountsOfClient(string NameBank)
        {
            var bank = FindBank(NameBank);
            Console.WriteLine("Чьи счета показать ?");
            int i = 0;
            foreach (var client in bank.clients)
            {
                ++i;
                Console.WriteLine("{0}) {1} {2}, паспорт: {3}, адресс: {4}", i, client.Name, client.Surname, client.Passport, client.Adress);
            }

            var idClient = int.Parse(Console.ReadLine());

            var theClient = bank.clients[idClient - 1];

            var accounts = bank.clientsAndAccounts.FindAll(connection => connection.client == theClient).Select(connection => connection.account);

            i = 0;
            foreach (var account in accounts)
            {
                ++i;
                Console.WriteLine("{0}) numbers: {1}, balance: {2}", i, account.NumbersAccount, account.Balance);
            }
        }
    }
}