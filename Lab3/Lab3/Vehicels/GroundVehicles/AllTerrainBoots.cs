namespace Lab3.Vehicels.GroundVehicles
{
    public class AllTerrainBoots : GroundVehicle
    {
        public override string Name { get; } = "Ботинки-вездеходы";
        public override double Speed { get; } = 10;
        public override double RestInterval { get; } = 60;
        public override int RestDuration(int itt)
        {
            switch (itt)
            {
                case 1: return 10;
                default: return 5;
            }
        }
    }
}