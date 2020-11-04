using Lab3.Exceptions;

namespace Lab3.Vehicels
{
    public abstract class AirVehicle 
    {
        public abstract string Name { get; }
        public abstract double Speed { get; }
        public abstract double DistanceReducer(double distance);
        
        public double RunTime(double distance)
        {
            if (Speed < 0)
                throw new ExceptionSpeed("Скорость меньше нуля");
            
            double time = DistanceReducer(distance) / Speed;
            
            if (time < 0)
                throw new ExceptionTime("Время меньше нуля");
            return time;
        }
    }
}