using Lab3.Data;
using Lab3.Exceptions;

namespace Lab3.Races
{
    public abstract class Race
    {
        public double Distance { get; }
        public Race(double distance)
        {
            if (distance < 1000 || distance > 10000000)
                throw new ExceptionDistance("Длинна трассы не соответствует стандарту");
            Distance = distance;
        }
        public abstract Pair Winner();
    }
}