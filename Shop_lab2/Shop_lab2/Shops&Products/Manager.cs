﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop_lab2
{
    public class Manager
    {
        private List<Shop> Shops = new List<Shop>();
        private List<Product> Goods = new List<Product>();

        public string GetNameShop(int idShop)
        {
            return Shops[idShop].Name;
        }

        public string GetNameProduct(int idProduct)
        {
            return Goods[idProduct].Name ;
        }

        public void CreateProduct(string name)
        {
            Goods.Add(new Product(Goods.Count, name));
        }
        
        public void CreateShop(string name, string address )
        {
            Shops.Add(new Shop(Shops.Count, name, address));
        }
        
        public void AddGoods(int idShop, int idGoods, int quantity, int price)
        {
            CheckQuantity(quantity);
            CheckProduct(idGoods);
            CheckShop(idShop);

            bool isAdd = false;

            foreach (var i in Shops[idShop].GetStor())
            {
                if (i.IdProduct == idGoods)
                {
                    i.Quantity += quantity;
                    i.Price = price;
                    isAdd = true;
                }
                break;
            }
            if(!isAdd)
                Shops[idShop].GetStor().Add(new Data(idGoods, price, quantity));
        }
        
        public void ShowShops()
        {
            if (Shops.Capacity == 0)
            {
                throw new Exceptions.ExceptionZeroShops("Магазинов еще нет");
            }
            var i = 0;
            foreach (var shop in Shops)
            {
                Console.WriteLine("id " + i + ") " + shop.Name);
                ++i;
            }
        }
        public void ShowAllProducts()
        {
            if (Goods.Capacity == 0)
            {
                throw new Exceptions.ExceptionZeroProducts("Товаров еще нет");
            }
            var i = 0;
            foreach (var product in Goods)
            {
                Console.WriteLine("id " + i + ") " + product.Name);
                i++;
            }
        }
        
        public void ShowProductsOfShop(int idShop)
        {
            CheckShop(idShop);
            if(Shops[idShop].GetStor().Capacity <= 0)
                throw new Exceptions.ExceptionZeroProducts("В магазине еще нет продуктов");
            var i = 1;
            Console.WriteLine("Имя : Кол-во : Цена");
            foreach (var product in Shops[idShop].GetStor())
            {
                Console.WriteLine(i + ") " + Goods[product.IdProduct].Name + " : " + product.Quantity + " : " + product.Price);
                i++;
            }
        }

        public (string? nameShop, int? Price) FindTheCheapestProduct(int idProduct)
        {
            CheckProduct(idProduct);
            int? theCheapestPrice = null;
            string? nameShop = null;
            foreach (var shop in Shops)
            {
                int? price = FindPriceOfProduct(shop.Id, idProduct);
                if (price != null)
                {
                    if (theCheapestPrice == null || theCheapestPrice > price)
                    {
                        theCheapestPrice = (int) price;
                        nameShop = shop.Name;
                    }
                }
            }
            return (nameShop, theCheapestPrice);
        }
        
        private int? FindPriceOfProduct(int idShop, int idProduct)
        {
            CheckShop(idShop);
            CheckProduct(idProduct);
            foreach (var i in Shops[idShop].GetStor())
            {
                if (i.IdProduct == idProduct)
                {
                    return i.Price;
                }
            }
            return null;
        }
        
        public List<(int idProduct, int quantity)> FindProductForSomeCost(int money, int idShop)
        {
            CheckShop(idShop);
            var answer = new List<(int idProduct, int quantity)>();

            for(int i = 0; i < Shops[idShop].GetStor().Count; ++i)
            {
                int quantity = Shops[idShop].GetStor()[i].Quantity;
                int price = Shops[idShop].GetStor()[i].Price;
                int q = money / price;
                if (q >= quantity)
                    answer.Add((Shops[idShop].GetStor()[i].IdProduct, quantity));
                else
                    answer.Add((Shops[idShop].GetStor()[i].IdProduct,q));
            }
            return answer;
        }
        
        public int BuySomeProducts(List<(int idProduct, int quantity)> list, int idShop)
        {
            CheckShop(idShop);
            foreach (var i in list)
            {
                CheckProduct(i.idProduct);
                CheckQuantity(i.quantity);
            }
            int cost = 0;
            int flag = 0;
            var idList = new List<( int idProductShop, int quantity )>();
            for (int i = 0; i < list.Count; ++i)
            {
                for (int n = 0; n < Shops[idShop].GetStor().Count; ++n)
                {
                    if (Shops[idShop].GetStor()[n].IdProduct == list[i].idProduct)
                    {
                        if (list[i].quantity > Shops[idShop].GetStor()[n].Quantity)
                            return -1;
                        flag = 1;
                        cost += Shops[idShop].GetStor()[n].Price * list[i].quantity;
                        idList.Add((n, list[i].quantity));
                        break;
                    }
                }

                if (flag == 0)
                    return -1;
            }

            foreach (var kek in idList)
                Shops[idShop].GetStor()[kek.idProductShop].Quantity -= kek.quantity;

            return cost;
        }
        
        public string? FindTheCheapestGoods(List<(int idProduct, int quantity)> list)
        {
            foreach (var i in list)
            {
                CheckProduct(i.idProduct);
                CheckQuantity(i.quantity);
            }
            int? cost = null;
            int? temp;
            string a = "";
            foreach (var i in Shops)
            {
                temp = BuySomeProducts(list, i.Id);
                if (cost == null || cost > temp)
                {
                    cost = temp;
                    a = i.Name;
                }
            }
            return a;
        }
        private void CheckShop(int idShop)
        {
            if (!(idShop >= 0 && idShop < Shops.Capacity - 1))
            {
                throw new Exceptions.ExceptionShopDoesntExist("Ошибка, такого магазина нет");
            }
        }

        private void CheckProduct(int idGoods)
        {
            if (!(idGoods >= 0 && idGoods < Goods.Capacity - 1))
            {
                throw new Exceptions.ExceptionProductDoesntExist("Ошибка, такого товара нет");
            }
        }
        private void CheckQuantity(int quantity)
        {
            if (quantity < 1)
            {
                throw new Exceptions.ExceptionQuantity("Ошибка, кол-во товаров меньше нуля");
            }
        }
    
    }
}