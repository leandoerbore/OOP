namespace Lab3.Vehicels.AirVehicles
{
    public class Mortar : AirVehicle
    {
        public override string Name { get; } = "Ступа";
        public override double Speed { get; } = 8;
        public override double DistanceReducer(double distance)
        {
            return distance * 0.94;
        }
    }
}