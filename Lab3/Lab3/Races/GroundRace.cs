using System.Collections.Generic;
using Lab3.Data;
using Lab3.Vehicels;

namespace Lab3.Races
{
    public class GroundRace : Race
    {
        public GroundRace(double distance) : base(distance)
        {
        }
        private List<GroundVehicle> _GroundVehicle = new List<GroundVehicle>();
        
        public void CheckIn(GroundVehicle vehicle)
        {
            _GroundVehicle.Add(vehicle);
        }

        public override Pair Winner()
        {
            Pair GWinnerVehicle;
            
            GWinnerVehicle = new Pair(_GroundVehicle[0].Name, _GroundVehicle[0].RunTime(Distance));
            for (int i = 1; i < _GroundVehicle.Count; ++i)
            {
                if (_GroundVehicle[i].RunTime(Distance) < GWinnerVehicle.Time)
                    GWinnerVehicle = new Pair(_GroundVehicle[i].Name, _GroundVehicle[i].RunTime(Distance));
            }

            return GWinnerVehicle;
        }
    }
}