using System;
using System.Collections.Generic;

namespace Shop_lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new Manager();
            int idShop;
            int money;
            int idProduct;
            manager.CreateShop("Дикси", "Сенная"); // id 0
            manager.CreateShop("Пятерочка", "Лесная"); // id 1
            manager.CreateShop("Икея", "Победы"); // id 2

            manager.CreateProduct("Пиво"); // id 0
            manager.CreateProduct("Чипсы"); // id 1
            manager.CreateProduct("Сок"); // id 2
            manager.CreateProduct("Сыр"); // id 3
            manager.CreateProduct("Молоко"); // id 4
            manager.CreateProduct("Кетчуп"); // id 5
            manager.CreateProduct("Курица"); // id 6
            manager.CreateProduct("Пельмени"); // id 7
            manager.CreateProduct("Авокадо"); // id 8
            manager.CreateProduct("Манго"); // id 9

            manager.AddGoods(0, 0,50, 50);
            manager.AddGoods(0, 1,50, 60);
            manager.AddGoods(0, 4,50, 75);
            manager.AddGoods(0, 6,50, 250);
            manager.AddGoods(0, 7,50, 115);
            manager.AddGoods(0, 5,50, 60);

            manager.AddGoods(1, 0,50, 40);
            manager.AddGoods(1, 9,50, 80);
            manager.AddGoods(1, 8,50, 50);
            manager.AddGoods(1, 7,50, 100);
            manager.AddGoods(1, 2,50, 50);
            manager.AddGoods(1, 1,50, 50);

            manager.AddGoods(2, 0,50, 30);
            manager.AddGoods(2, 2,50, 40);
            manager.AddGoods(2, 3,50, 65);
            manager.AddGoods(2, 4,50, 15);
            manager.AddGoods(2, 5,50, 88); 
            manager.AddGoods(2, 9,50, 70);
            manager.AddGoods(2, 8,50, 40);
            
            // 1 задание
            Console.WriteLine();
            Console.WriteLine("Задание 1");
            idProduct = 4;
            var theCheapestPrice = manager.FindTheCheapestProduct(idProduct);
            if (theCheapestPrice.nameShop != null)
            {
                Console.WriteLine("Самая дешевая цена в магазине {0} с ценой {1}", theCheapestPrice.nameShop, theCheapestPrice.Price);
            }
            else
            {
                Console.WriteLine("Не найден такой магазин");
            }
            // 2 задание
            Console.WriteLine();
            Console.WriteLine("Задание 2");
            money = 500;
            idShop = 0;
            var answer = manager.FindProductForSomeCost(money, idShop);
            Console.WriteLine("В магазине {0} на {1}р можно купить: ", manager.GetNameShop(idShop), money);
            foreach (var i in answer)
            {
                if (i.quantity > 0)
                    Console.WriteLine("{0} : {1}", manager.GetNameProduct(i.idProduct), i.quantity);
            }

            // 3 задание
            Console.WriteLine();
            Console.WriteLine("Задание 3");
            idShop = 2;
            var list3 = new List<(int idProduct,int quantity)>();
            list3.Add((2, 10));
            list3.Add((3, 15));
            list3.Add((4, 15));
            int? costOfProducts = manager.BuySomeProducts(list3, idShop);
                    
            if (costOfProducts != null)
                Console.WriteLine("Стоимость покупки всех товаров: {0}",costOfProducts);
            else
                Console.WriteLine("Ошибка, нет такого кол-ва товара");

            // 4 Задание
            Console.WriteLine();
            Console.WriteLine("Задание 4");
            var list4 = new List<(int idProduct, int quantity)>();
            list4.Add((0,1));

            var nameShop4 = manager.FindTheCheapestGoods(list4);

            if (nameShop4 != null)
                Console.WriteLine("Дешевле всех в магазине {0}", nameShop4);
            else
                Console.WriteLine("Ошибка, не получилось найти товар");
        }
    }
}