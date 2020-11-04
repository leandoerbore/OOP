namespace Lab3.Vehicels.GroundVehicles
{
    public class Centaur : GroundVehicle
    {
        public override string Name { get; } = "Кентавр";
        public override double Speed { get; } = 15;
        public override double RestInterval { get; } = 8;
        public override int RestDuration(int itt)
        {
            switch (itt)
            {
               default: return 2; 
            }
        }
    }
}