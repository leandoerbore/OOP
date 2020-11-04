namespace Lab3.Data
{
    public class Pair
    {
        public string Name { get; }
        public double Time { get; }

        public Pair(string name, double time)
        {
            Name = name;
            Time = time;
        }
    }
}