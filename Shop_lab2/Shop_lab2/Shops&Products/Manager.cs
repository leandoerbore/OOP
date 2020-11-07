using System;
using System.Collections.Generic;

namespace Shop_lab2
{
    public class Manager
    {
        public List<Shop> Shops = new List<Shop>();
        public List<Product> Goods = new List<Product>();

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
        
        public void AddGoods(int idShop, int idGoods, int quantity)
        {
            CheckQuantity(quantity);
            CheckProduct(idGoods);
            CheckShop(idShop);
            foreach (var i in Shops[idShop].GetStor())
            {
                if (i.IdProduct == idGoods)
                {
                    i.Quantity += quantity;
                    i.Price = (int) Goods[idGoods].GetPrice();
                }
                break;
            }
            Shops[idShop].GetStor().Add(new Data(idGoods, (int) Goods[idGoods].GetPrice(), quantity));
        }
        
        public void SetPriceOnProduct(int idProduct, int price)
        {
            CheckProduct(idProduct);
            Goods[idProduct].SetPrice(price);
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
                Console.WriteLine("id " + i + ") " + shop.GetName());
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
                Console.WriteLine("id " + i + ") " + product.GetName());
                i++;
            }
        }
        
        public void ShowProductsOfShop(int idShop)
        {
            CheckShop(idShop);
            if(Shops[idShop].GetStor().Capacity <= 0)
                throw new Exceptions.ExceptionZeroProducts("В магазине еще нет продуктов");
            var i = 1;
            foreach (var product in Shops[idShop].GetStor())
            {
                Console.WriteLine(i + ") " + Goods[product.IdProduct].GetName() + " : " + product.Quantity + " : " + product.Price);
                i++;
            }
        }
        
        public Tuple<int?, int> FindTheCheapestProduct(int idProduct)
        {
            CheckProduct(idProduct);
            int theCheapestPrice = Int32.MaxValue;
            int? idShop = null;
            foreach (var shop in Shops)
            {
                int? price = FindPriceOfProduct(shop.GetId(), idProduct);
                if (price != null)
                {
                    if (theCheapestPrice > price)
                    {
                        theCheapestPrice = (int) price;
                        idShop = shop.GetId();
                    }
                }
            }
            Tuple<int?, int> answer = new Tuple<int?, int>(idShop, theCheapestPrice);
            return answer;
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
        
        public Dictionary<int, int> FindProductForSomeCost(int money, int idShop)
        {
            CheckShop(idShop);
            Dictionary<int, int> answer = new Dictionary<int, int>();
            for(int i = 0; i < Shops[idShop].GetStor().Count; ++i)
            {
                int quantity = Shops[idShop].GetStor()[i].Quantity;
                int price = Shops[idShop].GetStor()[i].Price;
                int q = money / price;
                if (q >= quantity)
                    answer.Add(Shops[idShop].GetStor()[i].IdProduct, quantity);
                else
                {
                    answer.Add(Shops[idShop].GetStor()[i].IdProduct,q);
                }
            }
            return answer;
        }
        
        //Tuple<idProduct, quantity>
        public int BuySomeProducts(List<Tuple<int,int>> list, int idShop)
        {
            CheckShop(idShop);
            foreach (var i in list)
            {
                CheckProduct(i.Item1);
                CheckQuantity(i.Item2);
            }
            int cost = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                for (int n = 0; n < Shops[idShop].GetStor().Count; ++n)
                {
                    if (Shops[idShop].GetStor()[n].IdProduct == list[i].Item1)
                    {
                        if (list[i].Item2 > Shops[idShop].GetStor()[n].Quantity)
                            return -1;
                        cost += Shops[idShop].GetStor()[n].Price * list[i].Item2;
                        break;
                    }
                }
            }
            return cost;
        }
        
        //Tuple<idProduct, quantity>
        public Pair FindTheCheapestGoods(List<Tuple<int,int>> list)
        {
            foreach (var i in list)
            {
                CheckProduct(i.Item1);
                CheckQuantity(i.Item2);
            }
            var a = new Pair();
            int cost = Int32.MaxValue;
            int temp = -1;
            foreach (var i in Shops)
            {
                temp = BuySomeProducts(list, i.GetId());
                if (cost != -1 && cost > temp)
                {
                    cost = temp;
                    a = new Pair(i.GetName(), cost);
                }
            }
            if (cost == Int32.MaxValue)
                return null;
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