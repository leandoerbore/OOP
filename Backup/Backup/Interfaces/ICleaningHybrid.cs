using System.Collections.Generic;

namespace Backup
{
    public interface ICleaningHybrid
    {
        public List<IPoints> Cleaning(List<ICleaningAlgorithm> hybrid);
    }
}