using System.Collections.Generic;

namespace Backup
{
    public interface ICleaningAlgorithm
    {
        public List<IPoints> Cleaning();
    }
}