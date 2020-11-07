using System;
using System.Collections.Generic;

namespace Shop_lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            var Manager = new Manager();
            Console.WriteLine("1) Для отчета");
            Console.WriteLine("2) Для теста");
            int variable = Convert.ToInt32(Console.ReadLine());
            Tuple<int?, int> theCheapestPrice;
            Dictionary<int, int> answer;
            int idShop;
            int money;
            int idProduct;
            List<Tuple<int, int>> list;
            int cost;
            Pair a;
            switch (variable)
            {
                case 1:
                    Manager.CreateShop("Дикси", "Сенная"); // id 0
                    Manager.CreateShop("Пятерочка", "Лесная"); // id 1
                    Manager.CreateShop("Икея", "Победы"); // id 2

                    Manager.CreateProduct("Пиво"); // id 0
                    Manager.CreateProduct("Чипсы"); // id 1
                    Manager.CreateProduct("Сок"); // id 2
                    Manager.CreateProduct("Сыр"); // id 3
                    Manager.CreateProduct("Молоко"); // id 4
                    Manager.CreateProduct("Кетчуп"); // id 5
                    Manager.CreateProduct("Курица"); // id 6
                    Manager.CreateProduct("Пельмени"); // id 7
                    Manager.CreateProduct("Авокадо"); // id 8
                    Manager.CreateProduct("Манго"); // id 9

                    Manager.AddGoods(0, 0,50, 40);
                    Manager.AddGoods(0, 1,50, 60);
                    Manager.AddGoods(0, 4,50, 75);
                    Manager.AddGoods(0, 6,50, 250);
                    Manager.AddGoods(0, 7,50, 115);
                    Manager.AddGoods(0, 5,50, 60);

                    Manager.AddGoods(1, 0,50, 50);
                    Manager.AddGoods(1, 9,50, 80);
                    Manager.AddGoods(1, 8,50, 50);
                    Manager.AddGoods(1, 7,50, 100);
                    Manager.AddGoods(1, 2,50, 50);
                    Manager.AddGoods(1, 1,50, 50);

                    Manager.AddGoods(2, 2,50, 40);
                    Manager.AddGoods(2, 3,50, 65);
                    Manager.AddGoods(2, 4,50, 15);
                    Manager.AddGoods(2, 5,50, 88);
                    Manager.AddGoods(2, 9,50, 70);
                    Manager.AddGoods(2, 8,50, 40);

                    // 1 задание
                    Console.WriteLine();
                    Console.WriteLine("Задание 1");
                    idProduct = 4;
                    theCheapestPrice = Manager.FindTheCheapestProduct(idProduct);
                    if(theCheapestPrice.Item1 != null && theCheapestPrice.Item2 != Int32.MaxValue)
                        Console.WriteLine("В " + Manager.Shops[(int) theCheapestPrice.Item1].GetName() + " самая маленькая цена продукта " +
                                          Manager.Goods[idProduct].GetName() + ": " + theCheapestPrice.Item2);
                    // 2 задание
                    Console.WriteLine();
                    Console.WriteLine("Задание 2");
                    money = 500;
                    idShop = 0;
                    answer = Manager.FindProductForSomeCost(money, idShop);
                    Console.WriteLine("В магазине " + Manager.Shops[idShop].GetName() + " на " + money + "р" + " можно купить: ");
                    foreach (var i in answer)
                    {
                        if (i.Value != 0)
                        {
                            Console.WriteLine(Manager.Goods[i.Key].GetName() + " : " + i.Value);
                        }
                    }

                    // 3 задание
                    Console.WriteLine();
                    Console.WriteLine("Задание 3");
                    idShop = 2;
                    list = new List<Tuple<int, int>>();
                    list.Add(new Tuple<int, int>(0, 10));
                    list.Add(new Tuple<int, int>(1, 15));
                    list.Add(new Tuple<int, int>(4, 15));
                    cost = Manager.BuySomeProducts(list, idShop);

                    if(cost != -1)
                        Console.WriteLine("Стоимость покупки всех товаров: " + cost);
                    else
                    {
                        Console.WriteLine("Ошибка, нет такого кол-ва товара");
                    }
                    // 4 Задание
                    Console.WriteLine();
                    Console.WriteLine("Задание 4");
                    list.Add(new Tuple<int, int>(3,10));
                    list.Add(new Tuple<int, int>(0,1));
                    list.Add(new Tuple<int, int>(6,2));

                    a = Manager.FindTheCheapestGoods(list);

                    if(a != null && a.Item2 != -1)
                        Console.WriteLine("Дешевле всех в магазине " + a.Item1 + " по цене: " + a.Item2);
                    else
                    {
                        Console.WriteLine("Ошибка, не получилось найти товар");
                    }


                    break;
                case 2:
                    Console.WriteLine("_________Лабораторная работа №2 Магазин_________");
                    Console.WriteLine("1) Создать магазин");
                    Console.WriteLine("2) Создать продукт");
                    Console.WriteLine("3) Показать все магазины");
                    Console.WriteLine("4) Показать все продукты");
                    Console.WriteLine("5) Показать все продукты магазина");
                    Console.WriteLine("6) Добавить продукты в магазин");
                    Console.WriteLine("7) Найти магазин, в котором товар самый дешевый");
                    Console.WriteLine("8) Выбрать товары на определенную сумму");
                    Console.WriteLine("9) Купить партию товаров");
                    Console.WriteLine("0) Последнее задание еп");
                    while (true)
                    {

                        string num = Console.ReadLine();
                        string line, line1;
                        list = new List<Tuple<int,int>>();
                        switch (num)
                        {
                            case "1":
                                Console.WriteLine("Введите название магазина и адресс");
                                string nameShop = Console.ReadLine();
                                string addressShop = Console.ReadLine();
                                Manager.CreateShop(nameShop, addressShop);
                                break;
                            case "2" :
                                Console.WriteLine("Введите название продукта");
                                string nameProduct = Console.ReadLine();
                                Manager.CreateProduct(nameProduct);
                                break;
                            case "3":
                                Manager.ShowShops();
                                break;
                            case "4":
                                Manager.ShowAllProducts();
                                break;
                            case "5":
                                Console.WriteLine("Введите уникальный код магазина");
                                idShop = Convert.ToInt32(Console.ReadLine());
                                Manager.ShowProductsOfShop(idShop);
                                break;
                            case "6":
                                Console.WriteLine("Введите уникальный код магазина, уникальный код продукта, кол-во, цену");
                                idShop = Convert.ToInt32(Console.ReadLine());
                                idProduct = Convert.ToInt32(Console.ReadLine());
                                int quantity = Convert.ToInt32(Console.ReadLine());
                                int price = Convert.ToInt32(Console.ReadLine());
                                Manager.AddGoods(idShop, idProduct, quantity, price);
                                break;
                            case "7":
                                Console.WriteLine("Введите id продукта");
                                idProduct = Convert.ToInt32(Console.ReadLine());
                                theCheapestPrice = Manager.FindTheCheapestProduct(idProduct);

                                if(theCheapestPrice.Item1 != null && theCheapestPrice.Item2 != Int32.MaxValue)
                                    Console.WriteLine("В " + Manager.Shops[(int) theCheapestPrice.Item1].GetName() + " самая маленькая цена продукта "
                                                      +
                                                      Manager.Goods[idProduct].GetName() + ": " + theCheapestPrice.Item2);
                                break;
                            case "8":
                                Console.WriteLine("Введите кол-во денег и id магазина");
                                money = Convert.ToInt32(Console.ReadLine());
                                idShop = Convert.ToInt32(Console.ReadLine());
                                answer = Manager.FindProductForSomeCost(money, idShop);
                                Console.WriteLine("В магазине " + Manager.Shops[idShop].GetName() + " на " + money + "р" + " можно купить: ");
                                foreach (var i in answer)
                                {
                                    if (i.Value != 0)
                                    {
                                        Console.WriteLine(Manager.Goods[i.Key].GetName() + " : " + i.Value);
                                    }
                                }
                                break;
                            case "9":
                                Console.WriteLine("Введите id магазина");
                                idShop = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Введите id товара и кол-во, для завершения введите -1");
                                while (true)
                                {
                                    line = Console.ReadLine();
                                    if (line == "-1")
                                        break;
                                    line1 = Console.ReadLine();
                                    int first = Convert.ToInt32(line);
                                    int second = Convert.ToInt32(line1);
                                    list.Add(new Tuple<int, int>(first,second));
                                }

                                cost = Manager.BuySomeProducts(list, idShop);

                                if(cost != -1)
                                    Console.WriteLine("Стоимость покупки всех товаров: " + cost);
                                else
                                {
                                    Console.WriteLine("Ошибка, нет такого кол-ва товара");
                                }
                                break;
                            case "0":
                                Console.WriteLine("Введите id товара и кол-во, для завершения введите -1");

                                while (true)
                                {
                                    line = Console.ReadLine();
                                    if (line == "-1")
                                        break;
                                    line1 = Console.ReadLine();
                                    int first = Convert.ToInt32(line);
                                    int second = Convert.ToInt32(line1);
                                    list.Add(new Tuple<int, int>(first,second));
                                }

                                a = Manager.FindTheCheapestGoods(list);

                                if(a != null && a.Item2 != -1)
                                    Console.WriteLine("Дешевле всех в магазине " + a.Item1 + " по цене: " + a.Item2);
                                else
                                {
                                    Console.WriteLine("Ошибка, не получилось найти товар");
                                }
                                break;
                        }

                    }
                    break;
            }
        }
    }
}