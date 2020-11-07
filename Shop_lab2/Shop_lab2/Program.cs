using System;
using System.Collections.Generic;

namespace Shop_lab2
{
    class Program
    {
        static void Main(string[] args)
        {

            var Manager = new Manager();
            Dictionary<int, int> answer;
            int idShop;
            int money;
            int idProduct;
            int cost;
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
                    var theCheapestPrice = Manager.FindTheCheapestProduct(idProduct);
                    if(theCheapestPrice.Item1 != null)
                        Console.WriteLine("Самая дешевая цена в магазине {0} с ценой {1}", theCheapestPrice.nameShop, theCheapestPrice.Price);
                    // 2 задание
                    Console.WriteLine();
                    Console.WriteLine("Задание 2");
                    money = 500;
                    idShop = 0;
                    answer = Manager.FindProductForSomeCost(money, idShop);
                    Console.WriteLine("В магазине {0} на {1}р можно купить: ", Manager.GetNameShop(idShop), money);
                    foreach (var i in answer)
                    {
                        if (i.Value != 0)
                        {
                            Console.WriteLine("{0} : {1}", Manager.GetNameProduct(i.Key), i.Value);
                        }
                    }

                    // 3 задание
                    Console.WriteLine();
                    Console.WriteLine("Задание 3");
                    idShop = 2;
                    var list3 = new List<(int idProduct,int quantity)>();
                    list3.Add((0, 10));
                    list3.Add((1, 15));
                    list3.Add((4, 15));
                    cost = Manager.BuySomeProducts(list3, idShop);

                    if(cost != -1)
                        Console.WriteLine("Стоимость покупки всех товаров: " + cost);
                    else
                        Console.WriteLine("Ошибка, нет такого кол-ва товара");
                    
                    // 4 Задание
                    Console.WriteLine();
                    Console.WriteLine("Задание 4");
                    var list4 = new List<(int idProduct, int quantity)>();
                    list4.Add((3,10));
                    list4.Add((0,1));
                    list4.Add((6,2));

                    var NameShop4 = Manager.FindTheCheapestGoods(list4);

                    if(NameShop4 != null)
                        Console.WriteLine("Дешевле всех в магазине {0}", NameShop4);
                    else
                        Console.WriteLine("Ошибка, не получилось найти товар");

            
        }
    }
}