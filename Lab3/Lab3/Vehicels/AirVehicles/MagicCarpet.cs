namespace Lab3.Vehicels.AirVehicles
{
    public class MagicCarpet : AirVehicle
    {
        public override string Name { get; } = "Ковер самолет";
        public override double Speed { get; } = 10;
        public override double DistanceReducer(double distance)
        {
            if (distance < 1000)
                return distance;
            if (distance < 5000)
                return distance * 0.97;
            if (distance < 10000)
                return distance * 0.95;
            return distance * 0.80;
        }
    }
}