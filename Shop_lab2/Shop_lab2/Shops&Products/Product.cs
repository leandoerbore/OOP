namespace Shop_lab2
{
    public class Product
    {
        private string _name;
        private int _id;
        private double _price;


        public Product(int id, string name)
        {
            _id = id;
            _name = name;
        }
        public void SetPrice(double price)
        {
            _price = price;
        }
        public double GetPrice()
        {
            return _price;
        }
        public int GetId()
        {
            return _id;
        }
        public string GetName()
        {
            return _name;
        }
    }
}