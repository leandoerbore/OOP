namespace Lab3.Vehicels.GroundVehicles
{
    public class Papasha : GroundVehicle
    {
        public override string Name { get; } = "Папаша с кабриолетом черного цвета";
        public override double Speed { get; } = 1000;
        public override double RestInterval { get; } = 10000;
        public override int RestDuration(int itt)
        {
            return 1;
        }
    }
}