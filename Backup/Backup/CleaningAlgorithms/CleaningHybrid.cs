using System.Collections.Generic;

namespace Backup.CleaningAlgorithms
{
    public class CleaningHybrid : ICleaningAlgorithm
    {

        public CleaningHybrid(List<ICleaningAlgorithm> listOfAlgorithms)
        {
            
        }
        public List<IPoints> Cleaning()
        {
            throw new System.NotImplementedException();
        }
    }
}