using System;
using Lab3.Exceptions;
using Lab3.Races;
using Lab3.Vehicels.GroundVehicles;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var race = new GroundRace(1000);
                race.CheckIn(new Centaur());
                race.CheckIn(new CamelFastWalker());
                //race.CheckIn(new Papasha());
                //race.CheckIn(new Broom());
                // Кулити

                var winner = race.Winner();
                Console.WriteLine("Победитель - {0} с временем {1:f2}", winner.Name, winner.Time);
            }
            catch (ExceptionDistance e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ExceptionTime e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ExceptionSpeed e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}