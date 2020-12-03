using System.Collections.Generic;

namespace Banks
{
    public class ConditionInterest
    {
        public double Interest;
        
        public List<(int firstInterval, int secondInterval, double interest)> Conditions 
            = new List<(int firstInterval, int secondInterval, double interest)>();

        public ConditionInterest(List<(int firstInterval, int secondInterval, double interest)> newConditions)
        {
            Conditions = newConditions;
        }
        
    }
}