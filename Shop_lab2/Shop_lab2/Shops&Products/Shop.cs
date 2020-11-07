using System.Collections.Generic;

namespace Shop_lab2
{
    public class Shop
    {
        public string Name { get; }
        public string Address { get; }
        public int Id { get; }
        public Shop(int id,string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }


        private List<Data> _stor = new List<Data>();
        public List<Data> GetStor()
        {
            return _stor;
        }
        
    }
}