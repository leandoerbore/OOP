using System.Collections.Generic;
using System.Linq;

namespace Backup.CleaningAlgorithms
{
    public class CleaningBySize : ICleaningAlgorithm
    {
        
        public List<IPoints> Cleaning(FileBackup backup)
        {
            long backupSize = backup.backupSize;
            long maxSize = backup.maxSize;
            List<IPoints> restorePoints = backup.restorePoints;

            List<IPoints> SaveToDelbySize = new List<IPoints>();
            if (backupSize < maxSize)
                return SaveToDelbySize;
            var currentSize = backupSize;

            int countPoints = 0;
            foreach (var point in restorePoints)
            {
                currentSize -= point._size;
                ++countPoints;

                if (currentSize < maxSize)
                    break;
            }

            var FirstStepPointsToDeletebySize =
                from x in restorePoints
                where x is FullPoint && restorePoints.IndexOf(x) < countPoints
                select x;

            {
                var SecondStepPointsToDeletebySize =
                    from x in FirstStepPointsToDeletebySize
                    where x.IndexOfDeltas == 0
                    select x;

                foreach (var point in SecondStepPointsToDeletebySize)
                {
                    SaveToDelbySize.Add(point);
                }
            }

            var ThirdStepPointsToDeletebySize =
                from x in FirstStepPointsToDeletebySize
                where x.IndexOfDeltas > 0
                select x;

            int count = 0;
            currentSize = backupSize;
            foreach (var point in ThirdStepPointsToDeletebySize)
            {
                int indexOfFullPoint = restorePoints.IndexOf(point);
                for (int i = 1; i <= point.IndexOfDeltas; ++i)
                {
                    if (currentSize - restorePoints[indexOfFullPoint + i]._size > backupSize)
                    {
                        SaveToDelbySize.Add(restorePoints[indexOfFullPoint + i]);
                        count++;
                    }
                }

                point.IndexOfDeltas -= count;
                
            }

            return SaveToDelbySize;
        }
    }
}