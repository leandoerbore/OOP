using System.Runtime.ConstrainedExecution;
using System;

namespace Banks
{
    public class Client
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string? Adress { get; private set; }
        public long? Passport { get; private set; }

        public Client()
        {
            Console.WriteLine("Введите имя клиента");
            string name = Console.ReadLine();
            Name = name;
            
            Console.WriteLine("Введите фамилию клиента");
            string surname = Console.ReadLine();
            Surname = surname;
            
            Console.WriteLine("Добавляем номер паспорта? Y : N");
            var answer = Console.ReadLine();
            switch (answer)
            {
                case "Y":
                    Console.WriteLine("Введите номер паспорта клиента");
                    int? passport = int.Parse(Console.ReadLine());
                    if (!(passport is null))
                    {
                        Passport = passport;
                    }
                    break;
            }
            Console.WriteLine("Добавляем адресс? Y : N");
            answer = Console.ReadLine();
            switch (answer)
            {
                case "Y":
                    Console.WriteLine("Введите адресс клиента");
                    string adress = Console.ReadLine();
                    if (!(adress is null))
                    {
                        Adress = adress;
                    }
                    break;
            }
        }

        public void AddInformationAboutClient()
        {
            int answer;

            while (true)
            {
                if (Adress is null && Passport is null)
                {
                    Console.WriteLine("Какую информацию вы хотите добавить ?");
                    Console.WriteLine("1) Адресс проживания");
                    Console.WriteLine("2) Номер паспорта");
                    Console.WriteLine("0) Выйти");
                    answer = int.Parse(Console.ReadLine());
                    switch (answer)
                    {
                        case 1:
                            Console.WriteLine("Введите адресс");
                            Adress = Console.ReadLine();

                            break;
                        case 2:
                            Console.WriteLine("Введите номер паспорта");
                            Passport = long.Parse(Console.ReadLine());
                        
                            break;
                        case 3:

                            return;
                    }
                }
                else if (Adress is null)
                {
                    Console.WriteLine("Какую информацию вы хотите добавить ?");
                    Console.WriteLine("1) Адресс проживания");
                    Console.WriteLine("0) Выйти");
                    answer = int.Parse(Console.ReadLine());

                    switch (answer)
                    {
                        case 1:
                            Console.WriteLine("Введите адресс");
                            Adress = Console.ReadLine();
                            break;
                        case 0:
                            
                            return;
                    }
                }
                else if (Passport is null)
                {
                    Console.WriteLine("Какую информацию вы хотите добавить ?");
                    Console.WriteLine("1) Номер паспорта");
                    Console.WriteLine("0) Выйти");
                    answer = int.Parse(Console.ReadLine());

                    switch (answer)
                    {
                        case 1:
                            Console.WriteLine("Введите номер паспорта");
                            Passport = long.Parse(Console.ReadLine());
                            break;
                        case 0:
                            
                            return;
                    }
                }
                else
                {
                    Console.WriteLine("Вся информация о клиенте заполнена");
                    break;
                }
            }
        }
    }
}