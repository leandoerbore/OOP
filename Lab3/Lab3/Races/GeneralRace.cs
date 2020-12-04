using System;
using System.Collections.Generic;
using Lab3.Data;
using Lab3.Vehicels;

namespace Lab3.Races
{
    public class GeneralRace : Race
    {
        public GeneralRace(double distance) : base(distance)
        {
        }
        
        private List<AirVehicle> _AirVehicles = new List<AirVehicle>();
        private List<GroundVehicle> _GroundVehicle = new List<GroundVehicle>();
        

        public void CheckIn(GroundVehicle vehicle)
        {
            _GroundVehicle.Add(vehicle);
        }

        public void CheckIn(AirVehicle vehicle)
        {
            _AirVehicles.Add(vehicle);
        }
        
        public override Pair Winner()
        {
            int flag;
            if (_AirVehicles.Capacity != 0 && _GroundVehicle.Capacity != 0)
                flag = 1;
            else if (_AirVehicles.Capacity != 0)
                flag = 2;
            else if (_GroundVehicle.Capacity != 0)
                flag = 3;
            else
                throw new Exception("Никто не записался на гонку");

            Pair AWinnerVehicle;
            Pair GWinnerVehicle;
            switch (flag)
            {
                case 1:
                    AWinnerVehicle = new Pair(_AirVehicles[0].Name, _AirVehicles[0].RunTime(Distance));
                    GWinnerVehicle = new Pair(_GroundVehicle[0].Name, _GroundVehicle[0].RunTime(Distance));

                    for (int i = 1; i < _AirVehicles.Count; ++i)
                    {
                        if (_AirVehicles[i].RunTime(Distance) < AWinnerVehicle.Time)
                            AWinnerVehicle = new Pair(_AirVehicles[i].Name, _AirVehicles[i].RunTime(Distance));
                    }
                    
                    for (int i = 1; i < _GroundVehicle.Count; ++i)
                    {
                        if (_GroundVehicle[i].RunTime(Distance) < GWinnerVehicle.Time)
                            GWinnerVehicle = new Pair(_GroundVehicle[i].Name, _GroundVehicle[i].RunTime(Distance));
                    }

                    var WinnerVehicle = AWinnerVehicle.Time <= GWinnerVehicle.Time ? AWinnerVehicle : GWinnerVehicle;
                    return WinnerVehicle;
                case 2:
                    AWinnerVehicle = new Pair(_AirVehicles[0].Name, _AirVehicles[0].RunTime(Distance));
                    for (int i = 1; i < _AirVehicles.Count; ++i)
                    {
                        if (_AirVehicles[i].RunTime(Distance) < AWinnerVehicle.Time)
                            AWinnerVehicle = new Pair(_AirVehicles[i].Name, _AirVehicles[i].RunTime(Distance));
                    }

                    return AWinnerVehicle;
                case 3:
                    GWinnerVehicle = new Pair(_GroundVehicle[0].Name, _GroundVehicle[0].RunTime(Distance));
                    for (int i = 1; i < _GroundVehicle.Count; ++i)
                    {
                        if (_GroundVehicle[i].RunTime(Distance) < GWinnerVehicle.Time)
                            GWinnerVehicle = new Pair(_GroundVehicle[i].Name, _GroundVehicle[i].RunTime(Distance));
                    }

                    return GWinnerVehicle;
            }

            return null;
        }
    }
}