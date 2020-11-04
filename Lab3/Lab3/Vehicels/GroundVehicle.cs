using Lab3.Exceptions;

namespace Lab3.Vehicels
{
    public abstract class GroundVehicle 
    {
        public abstract string Name { get; }
        public abstract double Speed { get; }
        public abstract double RestInterval { get; }
        public abstract int RestDuration(int itt);

        public double RunTime(double distance)
        {
            if (Speed < 0)
                throw new ExceptionSpeed("Скорость меньше нуля");
            double time = distance / Speed;
            int countRest = (int) (distance / RestInterval);
            for (int i = 1; i <= countRest; ++i)
            {
                time += RestDuration(i);
            }

            if (time < 0)
                throw new ExceptionTime("Время меньше нуля");
            return time;
        }
    }
}