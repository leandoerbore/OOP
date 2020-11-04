namespace Lab3.Vehicels.GroundVehicles
{
    public class CamelFastWalker : GroundVehicle
    {
        public override string Name { get; } = "Верблюд быстроход";
        public override double Speed { get; } = 40;
        public override double RestInterval { get; } = 10;
        public override int RestDuration(int itt)
        {
            switch (itt)
            {
               case 1: return 5;
               case 2: return 7;
               default: return 8;
            }
        }
    }
}