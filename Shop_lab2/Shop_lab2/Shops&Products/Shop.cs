using System.Collections.Generic;

namespace Shop_lab2
{
    public class Shop
    {
        private string _name;
        private string _address;
        private int _id;
        public Shop(int id,string name, string address)
        {
            _id = id;
            _name = name;
            _address = address;
        }


        private List<Data> _stor = new List<Data>();
        public List<Data> GetStor()
        {
            return _stor;
        }
        public string GetName()
        {
            return _name;
        }
        public int GetId()
        {
            return _id;
        }
    }
}