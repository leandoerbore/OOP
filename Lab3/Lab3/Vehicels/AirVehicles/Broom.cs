namespace Lab3.Vehicels.AirVehicles
{
    public class Broom : AirVehicle
    {
        public override string Name { get; } = "Метла";
        public override double Speed { get; } = 20;
        public override double DistanceReducer(double distance)
        {
            return distance * (distance / 100000);
        }
    }
}