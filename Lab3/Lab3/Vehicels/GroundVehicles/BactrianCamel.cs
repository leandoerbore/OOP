namespace Lab3.Vehicels.GroundVehicles
{
    public class BactrianCamel : GroundVehicle
    {
        public override string Name { get; } = "Двугорбый верблюд";
        public override double Speed { get; } = 10;
        public override double RestInterval { get; } = 30;
        public override int RestDuration(int itt)
        {
            switch (itt)
            {
                case 1: return 5;
                default: return 8;
            }
        }
        
        
        
    }
}