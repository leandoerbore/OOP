using System.Collections.Generic;
using Lab3.Data;
using Lab3.Vehicels;

namespace Lab3.Races
{
    public class AirRace : Race
    {
        public AirRace(double distance) : base(distance)
        {
        }
        
        private List<AirVehicle> _AirVehicles = new List<AirVehicle>();
        
        public void CheckIn(AirVehicle vehicle)
        {
            _AirVehicles.Add(vehicle);
        }

        public override Pair Winner()
        {
           
            Pair AWinnerVehicle;

            AWinnerVehicle = new Pair(_AirVehicles[0].Name, _AirVehicles[0].RunTime(Distance));
            for (int i = 1; i < _AirVehicles.Count; ++i)
            {
                if (_AirVehicles[i].RunTime(Distance) < AWinnerVehicle.Time)
                    AWinnerVehicle = new Pair(_AirVehicles[i].Name, _AirVehicles[i].RunTime(Distance));
            }

            return AWinnerVehicle;
        }
    }
}