#nullable enable
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

            var desiredData = Shops[idShop].GetStor().Find(x => x.IdProduct == idGoods);
            if (desiredData is null)
            {
                Shops[idShop].GetStor().Add(new Data(idGoods, price, quantity));
            }
            else
            {
                desiredData.Quantity += quantity;
                desiredData.Price = price;
            }
            
        }
        
        public void ShowShops()
        {
            if (Shops.Count == 0)
            {
                throw new ExceptionZeroShops("Магазинов еще нет");
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
            if (Goods.Count == 0)
            {
                throw new ExceptionZeroProducts("Товаров еще нет");
            }
            var i = 0;
            foreach (var product in Goods)
            {
                Console.WriteLine("id {0}) {1} ) ", i, product.Name);
                ++i;
            }
        }
        
        public void ShowProductsOfShop(int idShop)
        {
            CheckShop(idShop);
            if(Shops[idShop].GetStor().Count <= 0)
                throw new ExceptionZeroProducts("В магазине еще нет продуктов");
            var i = 1;
            Console.WriteLine("Имя : Кол-во : Цена");
            foreach (var product in Shops[idShop].GetStor())
            {
                Console.WriteLine("{0}) {1} : {2} : {3}", i, Goods[product.IdProduct].Name, product.Quantity, product.Price );
                ++i;
            }
        }

        public (string? nameShop, int? Price) FindTheCheapestProduct(int idProduct)
        {
            CheckProduct(idProduct);

            IEnumerable<(string name, int? price)> theCheapestProductWithPrice = Shops
                .Select(shop => (shop.Name, FindPriceOfProduct(shop.Id, idProduct)))
                .Where(shop => shop.Item2 != null)
                .OrderBy(shop => shop.Item2)
                .Take(1);
            
            return (theCheapestProductWithPrice.FirstOrDefault().name, theCheapestProductWithPrice.FirstOrDefault().price);
        }
        
        private int? FindPriceOfProduct(int idShop, int idProduct)
        {
            CheckShop(idShop);
            CheckProduct(idProduct);

            var priceOfProduct = Shops[idShop].GetStor()
                .Find(productData => productData.IdProduct == idProduct);

            return priceOfProduct?.Price;
        }
        
        public IEnumerable<(int idProduct, int quantity)> FindProductForSomeCost(int money, int idShop)
        {
            CheckShop(idShop);

            IEnumerable<(int idProduct, int quantity)> productsWeCanBuyAll = Shops[idShop]
                .GetStor()
                .FindAll(productData => productData.Quantity <= money / productData.Price)
                .Select(productData => (productData.IdProduct, productData.Quantity) );

            IEnumerable<(int idProduct, int quantity)> productsWeCanBuyPartly = Shops[idShop]
                .GetStor()
                .FindAll(productData => productData.Quantity > money / productData.Price)
                .Select(productData => (productData.IdProduct, money / productData.Price));


            var allProductsToBuy =
                productsWeCanBuyAll.Select(x => x)
                    .Concat(productsWeCanBuyPartly.Select(d => d))
                    .OrderBy(x => x.quantity);

            return allProductsToBuy;
        }
        
        public int? BuySomeProducts(List<(int idProduct, int quantity)> productSheet, int idShop)
        {
            CheckShop(idShop);
            foreach (var product in productSheet)
            {
                CheckProduct(product.idProduct);
                CheckQuantity(product.quantity);
            }
            int? cost = 0;
            
            var idList = new List<( int idProduct, int quantity )>();
            var shopStore = Shops[idShop].GetStor();

            foreach (var product in productSheet)
            {
                var desiredProduct = shopStore.Find(productData => productData.IdProduct == product.idProduct );
                if (desiredProduct != null && desiredProduct.Quantity > product.quantity)
                {
                    cost += desiredProduct.Price * product.quantity;
                    idList.Add((product.idProduct, product.quantity));
                }
            }

            foreach (var check in idList)
                shopStore[check.idProduct].Quantity -= check.quantity;

            return cost;
        }
        
        public string? FindTheCheapestGoods(List<(int idProduct, int quantity)> list)
        {
            foreach (var idProductAndQuantity in list)
            {
                CheckProduct(idProductAndQuantity.idProduct);
                CheckQuantity(idProductAndQuantity.quantity);
            }

            IEnumerable<(string name, int? cost)> desiredShop = Shops
                .Select(shop => (shop.Name, BuySomeProducts(list, shop.Id)))
                .OrderBy(shop => shop.Item2)
                .Take(1);

            return desiredShop.FirstOrDefault().name;
        }
        private void CheckShop(int idShop)
        {
            if (!(idShop >= 0 && idShop < Shops.Count))
            {
                throw new ExceptionShopDoesntExist("Ошибка, такого магазина нет");
            }
        }

        private void CheckProduct(int idGoods)
        {
            if (!(idGoods >= 0 && idGoods < Goods.Count))
            {
                throw new ExceptionProductDoesntExist("Ошибка, такого товара нет");
            }
        }
        private void CheckQuantity(int quantity)
        {
            if (quantity < 1)
            {
                throw new ExceptionQuantity("Ошибка, кол-во товаров меньше нуля");
            }
        }
    
    }
}