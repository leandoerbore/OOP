namespace Shop_lab2
{
    public class Data
    {
        public int IdProduct { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public Data(int idProduct, int price, int quantity)
        {
            IdProduct = idProduct;
            Price = price;
            Quantity = quantity;
        }
    }
}