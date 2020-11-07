namespace Shop_lab2
{
    public class Product
    {
        public string Name { get; }
        public int Id { get; }
        public double Price { get; set; }


        public Product(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}